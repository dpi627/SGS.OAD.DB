using SGS.OAD.DB.Models;

namespace SGS.OAD.DB.Services.Interfaces
{
    public interface IDecryptService
    {
        UserInfo DecryptUserInfo(UserInfo encryptedUserInfo);
    }
}
