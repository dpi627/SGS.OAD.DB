using SGS.OAD.DB.API.Services.DTOs;

namespace SGS.OAD.DB.API.Services.Interfaces
{
    public interface IUserInfoService
    {
        public UserInfoResultModel? Get(UserInfoInfo info);
    }
}
