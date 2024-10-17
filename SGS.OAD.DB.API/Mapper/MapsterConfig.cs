using Mapster;
using SGS.OAD.DB.API.Repositories.DTOs;
using SGS.OAD.DB.API.Services.DTOs;

namespace SGS.OAD.DB.API.Mapper
{
    public class MapsterConfig
    {
        public static void Configure()
        {
            TypeAdapterConfig<(SqlEncryptPasswordDataModel Data, UserInfoInfo Info), UserInfoResultModel>
                .NewConfig()
                .Map(dest => dest.ID, src =>
                    src.Info.Algorithm == ApiAlgorithm.AES ? src.Data.EncrptedId : src.Data.ID)
                .Map(dest => dest.PW, src =>
                    src.Info.Algorithm == ApiAlgorithm.AES ? src.Data.EncrptedPassword : src.Data.PW)
                ;
        }
    }
}
