namespace SGS.OAD.DB.Services.Implements
{
    public class FileLogger : ILogger
    {
        private readonly string _logDirectory;
        private readonly object _lock = new object();

        public FileLogger(string logDirectory)
        {
            _logDirectory = logDirectory;

            // 確保目錄存在
            if (!Directory.Exists(_logDirectory))
                Directory.CreateDirectory(_logDirectory);
        }

        public void LogInformation(string message)
        {
            LogToFile(message, LogLevel.INF);
        }

        public void LogWarning(string message)
        {
            LogToFile(message, LogLevel.WAR);
        }

        public void LogError(string message, Exception exception = null)
        {
            LogToFile(message, LogLevel.ERR);

            if (exception != null)
                LogToFile(exception.ToString(), LogLevel.ERR);
        }

        private void LogToFile(string message, LogLevel logLevel)
        {
            string msg = $"[{logLevel}] {DateTime.Now:yyyy/MM/dd HH:mm:ss}: {message}";

            // 取得今天的檔案名稱
            string filePath = GetLogFilePath();

            // 確保同時只有一個執行緒可以寫入檔案，避免產生 race condition
            lock (_lock)
            {
                File.AppendAllText(filePath, msg + Environment.NewLine);
            }
        }

        private string GetLogFilePath()
        {
            // 以 yyyy-MM-dd 的格式產生檔名，確保每天都有一個新檔案
            string date = DateTime.Now.ToString("yyyyMMdd");
            return Path.Combine(_logDirectory, $"db_{date}.log");
        }
    }
}
