using SGS.OAD.DB.Enums;
using SGS.OAD.DB.Services.Interfaces;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using static SGS.OAD.DB.Services.Implements.Logger;

namespace SGS.OAD.DB.Services.Implements
{
    public class Logger : ILogger
    {
        private readonly ConcurrentQueue<LogEntry> _logQueue = new();
        private readonly CancellationTokenSource _cts = new();
        private readonly Task _backgroundTask;
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly string _seqUrl = "http://twtpeoad002:5341";
        private readonly string _seqApiKey = "pyvd2FERKUkIw2xUJVyi";
        private bool _isFlushing;

        public Logger()
        {
            _backgroundTask = Task.Run(ProcessLogQueue, _cts.Token);
        }

        public void Log(LogLevel level, string message)
        {
            _logQueue.Enqueue(new LogEntry { Level = level, Message = message });
        }

        public void LogError(string message, Exception ex = null)
        {
            _logQueue.Enqueue(new LogEntry { Level = LogLevel.Error, Message = $"{message} {ex?.ToString()}" });
        }

        public void LogInformation(string message)
        {
            _logQueue.Enqueue(new LogEntry { Level = LogLevel.Information, Message = message });
        }

        public void LogWarning(string message)
        {
            _logQueue.Enqueue(new LogEntry { Level = LogLevel.Warning, Message = message });
        }


        private async Task ProcessLogQueue()
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                if (_logQueue.TryDequeue(out var logEntry))
                {
                    Console.WriteLine($"[{logEntry.Level}] {logEntry.Message}");
                    await SendLogToSeq(logEntry.Message, logEntry.Level);
                }
                else
                {
                    // 如果佇列是空的，暫停一段時間
                    await Task.Delay(100);
                }
            }
        }

        private async Task SendLogToSeq(string log, LogLevel level)
        {
            var seqEvent = new SeqEvent
            {
                Timestamp = DateTime.UtcNow,
                Level = MapLogLevel(level),
                MessageTemplate = log
            };

            var payload = new SeqPayload
            {
                Events = new[] { seqEvent }
            };

            var settings = new DataContractJsonSerializerSettings
            {
                DateTimeFormat = new DateTimeFormat("o") // ISO 8601 格式
            };
            var serializer = new DataContractJsonSerializer(typeof(SeqPayload), settings);

            using var memoryStream = new MemoryStream();
            serializer.WriteObject(memoryStream, payload);
            var jsonPayload = Encoding.UTF8.GetString(memoryStream.ToArray());
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync($"{_seqUrl}/api/events/raw?apiKey={_seqApiKey}", content);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to send log to Seq: {response.StatusCode} - {response.ReasonPhrase}");
                    Console.WriteLine($"Payload: {jsonPayload}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while sending log to Seq: {ex.Message}");
            }
        }

        public class LogEntry
        {
            public LogLevel Level { get; set; }
            public string Message { get; set; }
        }

        private string MapLogLevel(LogLevel level)
        {
            return level switch
            {
                LogLevel.Information => "Information",
                LogLevel.Warning => "Warning",
                LogLevel.Error => "Error",
                _ => "Information",
            };
        }




        [DataContract]
        public class SeqPayload
        {
            [DataMember(Name = "Events")]
            public SeqEvent[] Events { get; set; }
        }

        [DataContract]
        public class SeqEvent
        {
            [DataMember(Name = "Timestamp")]
            public DateTime Timestamp { get; set; }

            [DataMember(Name = "Level")]
            public string Level { get; set; }

            [DataMember(Name = "MessageTemplate")]
            public string MessageTemplate { get; set; }
        }


        public void Dispose()
        {
            _cts.Cancel();
            _backgroundTask.Wait();
        }

        public void FlushAndDispose()
        {
            _isFlushing = true;

            // 等待佇列處理完成
            while (!_logQueue.IsEmpty)
            {
                Thread.Sleep(100); // 確保不會消耗過多 CPU
            }

            _isFlushing = false;
            _cts.Cancel();
            _backgroundTask.Wait(); // 等待背景工作完成
            Dispose();
        }
    }
}
