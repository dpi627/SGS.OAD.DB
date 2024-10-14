using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using SGS.OAD.DB.Services.Interfaces;
using SGS.OAD.DB.Models;

namespace SGS.OAD.DB.Services.Implements
{
    public class UserInfoService : IUserInfoService
    {
        private static readonly HttpClient _client = new HttpClient();

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
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                // setting request's header here
                // request.Headers.Add("Custom-Header", "HeaderValue");

                var response = await _client
                    .SendAsync(request, cancellationToken)
                    .ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    //var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    string json = await ReadResponseContentAsync(response, cancellationToken).ConfigureAwait(false);
                    var userInfoEncrypt = DeserializeJson<UserInfoJson>(json);

                    return new UserInfo
                    {
                        UserId = userInfoEncrypt.ID,
                        Password = userInfoEncrypt.PW
                    };
                }
                else
                {
                    throw new HttpRequestException($"Can't fetch UserInfo from {url}, status code: {response.StatusCode}");
                }
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
