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
        /// <param name="url"></param>
        /// <returns></returns>
        public UserInfo GetEncryptedUserInfo(string url)
        {
            return GetEncryptedUserInfoAsync(url).GetAwaiter().GetResult();
        }

        /// <summary>
        /// 取得加密的使用者資料
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<UserInfo> GetEncryptedUserInfoAsync(string url)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                // setting request's header here
                // request.Headers.Add("Custom-Header", "HeaderValue");

                var response = await _client.SendAsync(request).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
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
