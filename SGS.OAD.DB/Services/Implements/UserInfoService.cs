using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using SGS.OAD.DB.Services.Interfaces;
using SGS.OAD.DB.Models;

namespace SGS.OAD.DB.Services.Implements
{
    public class UserInfoService : IUserInfoService
    {
        private readonly ILogger _logger;
        private static readonly HttpClient _client = new HttpClient();

        public UserInfoService(ILogger logger)
        {
            _logger = logger;
        }

        // 同步方法
        public UserInfo GetEncryptedUserInfo(string url)
        {
            return GetEncryptedUserInfoAsync(url).GetAwaiter().GetResult();
        }

        // 异步方法，使用 Task + ContinueWith
        public async Task<UserInfo> GetEncryptedUserInfoAsync(string url)
        {
            _logger.LogInformation($"Fetch url: {url}");

            // setting request's header here

            var response = await _client.GetAsync(url).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                // Deserialize JSON without Newtonsoft.Json
                var userInfoEncrypt = DeserializeJson<UserInfoJson>(json);

                return new UserInfo
                {
                    UserId = userInfoEncrypt.ID,
                    Password = userInfoEncrypt.PW
                };
            }
            else
            {
                throw new HttpRequestException($"Can't fetch UserInfo from {url} , status code: {response.StatusCode}");
            }
        }

        // Deserialize JSON using DataContractJsonSerializer
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
