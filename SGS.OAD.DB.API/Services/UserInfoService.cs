using MapsterMapper;
using SGS.OAD.DB.API.Repositories.DTOs;
using SGS.OAD.DB.API.Repositories.Interfaces;
using SGS.OAD.DB.API.Services.DTOs;

namespace SGS.OAD.DB.API.Services;

public class UserInfoService(
    ISqlEncryptPasswordRepository repo,
    IMapper mapper,
    ILogger<UserInfoService> logger
) : Interfaces.IUserInfoService
{
    public UserInfoResultModel? Get(UserInfoInfo info)
    {
        logger.LogInformation("Get UserInfo with {@info}", info);
        var condition = mapper.Map<SqlEncryptPasswordCondition>(info);
        var data = repo.Query(condition);

        if (data == null)
        {
            logger.LogWarning("No UserInfo found for {@info}", info);
            return null;
        }

        // 這邊使用 Tuple 來傳遞 data 和 info，並於 Mapster Config 判斷映射欄位
        // 映射規則請參考 SGS.OAD.DB.API/MapsterConfig.cs
        var result = mapper.Map<UserInfoResultModel>((data, info));

        logger.LogInformation("Return UserInfoResultModel with {@result}", result);
        return result;
    }
}
