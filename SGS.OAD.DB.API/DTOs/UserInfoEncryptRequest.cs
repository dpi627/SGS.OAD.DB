#nullable disable

namespace SGS.OAD.DB.API.DTOs
{
    public class UserInfoEncryptRequest
    {
        public ProgramLanguage PLanguage { get; set; } = ProgramLanguage.Csharp;

        public string ServerName { get; set; }

        public string DatabaseName { get; set; }

        public DatabaseRole DatabaseRole { get; set; } = DatabaseRole.db_datawriter;

        public string AppName { get; set; } = "SYSOP";

        public ApiAlgorithm Algorithm { get; set; } = ApiAlgorithm.DES;
    }
}
