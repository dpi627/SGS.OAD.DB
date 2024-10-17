using SGS.OAD.DB.API.Repositories.DTOs;

namespace SGS.OAD.DB.API.Repositories.Interfaces
{
    public interface ISqlEncryptPasswordRepository
    {
        public SqlEncryptPasswordDataModel? Query(SqlEncryptPasswordCondition condition);
    }
}
