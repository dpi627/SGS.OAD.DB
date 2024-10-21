using MapsterMapper;
using SGS.OAD.DB.API.Models;
using SGS.OAD.DB.API.Repositories.DTOs;
using SGS.OAD.DB.API.Repositories.Interfaces;

namespace SGS.OAD.DB.API.Repositories
{
    public class SqlEncryptPasswordRepository(
        SGSLims_chemContext context,
        IMapper mapper,
        ILogger<SqlEncryptPasswordRepository> logger
    ) : ISqlEncryptPasswordRepository
    {
        public SqlEncryptPasswordDataModel? Query(SqlEncryptPasswordCondition condition)
        {
            logger.LogInformation("Select SqlEncryptPassword with {@condition}", condition);

            var data = context.SQLEncryptPasswords
                .Where(p =>
                        p.PLanguage == condition.PLanguage.ToString() &&
                        p.ServerName == condition.ServerName &&
                        p.DatabaseName == condition.DatabaseName &&
                        p.DatabaseRole == condition.DatabaseRole.ToString()
                ).FirstOrDefault();

            if (data == null)
                return null;

            var result = mapper.Map<SqlEncryptPasswordDataModel>(data);

            logger.LogInformation("Return SqlEncryptPasswordDataModel with {@result}", result);

            return result;
        }
    }
}
