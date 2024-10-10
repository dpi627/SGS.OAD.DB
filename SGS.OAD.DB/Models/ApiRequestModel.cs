using SGS.OAD.DB.Enums;

namespace SGS.OAD.DB.Models
{
    public class ApiRequestModel
    {
        public string ServerName { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public ProgramLanguage ProgramLanguage { get; set; } = ProgramLanguage.Csharp;
        public DatabaseRole DatabaseRole { get; set; } = DatabaseRole.db_owner;
    }
}
