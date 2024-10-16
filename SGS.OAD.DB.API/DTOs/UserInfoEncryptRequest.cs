#nullable disable

namespace SGS.OAD.DB.API.DTOs
{
    public class UserInfoEncryptRequest
    {
        public string PLanguage { get; set; } = "Csharp";

        public string ServerName { get; set; }

        public string DatabaseName { get; set; }

        public string DatabaseRole { get; set; } = "db_datawriter";

        public string AppName { get; set; } = "SYSOP";
    }
}
