using SGS.OAD.DB.Models;

namespace SGS.OAD.DB.Services.Interfaces
{
    public interface IUserInfoService
    {
        Task<UserInfo> GetEncryptedUserInfoAsync(string url);
    }
}
