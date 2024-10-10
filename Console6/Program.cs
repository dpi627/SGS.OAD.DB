using SGS.OAD.DB.Builders;

namespace Console6
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var resutl = ConnectionStringBuilder.Init()
                .SetSever("TWDB009")
                .SetDatabase("SGSLims_Chem")
                .SetApi(api => api.SetParameter(
                    new SGS.OAD.DB.Models.ApiRequestModel() {
                        ServerName = "TWDB009",
                        DatabaseName = "SGSLims_Chem",
                        ProgramLanguage = SGS.OAD.DB.Enums.ProgramLanguage.Csharp,
                        DatabaseRole = SGS.OAD.DB.Enums.DatabaseRole.db_owner
                    }
                    ))
                .Build();
            Console.WriteLine(resutl);
        }
    }
}
