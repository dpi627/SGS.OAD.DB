using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace SGS.OAD.DB
{
    public class UserInfoService : IUserInfoService
    {
        private static readonly HttpClient _client;

        static UserInfoService()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = ValidateServerCertificate
            };
            _client = new HttpClient(handler);
        }

        /// <summary>
        /// 檢驗 HTTPS 憑證狀態，無論結果都給過，異常則顯示警告
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        private static bool ValidateServerCertificate(HttpRequestMessage requestMessage, X509Certificate2 certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine($"SSL Certificate Error: {sslPolicyErrors}");
            Console.WriteLine("Warning: Ignoring SSL certificate error. This may pose security risks.");
            return true;
        }

        /// <summary>
        /// 取得加密的使用者資料
        /// </summary>
        /// <param name="url">Web API Endpoint</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public UserInfo GetEncryptedUserInfo(string url, CancellationToken cancellationToken = default)
        {
            return GetEncryptedUserInfoAsync(url, cancellationToken).GetAwaiter().GetResult();
        }

        /// <summary>
        /// 取得加密的使用者資料
        /// </summary>
        /// <param name="url">Web API Endpoint</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<UserInfo> GetEncryptedUserInfoAsync(string url, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    // setting request's header here
                    // request.Headers.Add("Custom-Header", "HeaderValue");

                    var response = await _client
                        .SendAsync(request, cancellationToken)
                        .ConfigureAwait(false);

                    response.EnsureSuccessStatusCode();

                    string json = await ReadResponseContentAsync(response, cancellationToken).ConfigureAwait(false);
                    var userInfoEncrypt = DeserializeJson<UserInfoJson>(json);

                    return new UserInfo
                    {
                        UserId = userInfoEncrypt.ID,
                        Password = userInfoEncrypt.PW
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException($"Can't fetch UserInfo from {url}. Error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 解決版本相容性問題，確保 CancellationToken 被正確處理
        /// </summary>
        /// <param name="response"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<string> ReadResponseContentAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            #if NET5_0_OR_GREATER
                return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            #else
                var readTask = response.Content.ReadAsStringAsync();
                if (readTask.IsCompleted)
                {
                    return readTask.Result;
                }

                using (cancellationToken.Register(() => response.Content.Dispose()))
                {
                    return await readTask.ConfigureAwait(false);
                }
            #endif
        }

        /// <summary>
        /// Deserialize JSON string to object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        private static T DeserializeJson<T>(string json)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                return (T)serializer.ReadObject(ms);
            }
        }
    }
}
