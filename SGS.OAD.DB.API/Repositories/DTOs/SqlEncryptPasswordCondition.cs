#nullable disable

namespace SGS.OAD.DB.API.Repositories.DTOs
{
    public class SqlEncryptPasswordCondition
    {
        public ProgramLanguage PLanguage { get; set; } = ProgramLanguage.Csharp;

        public string ServerName { get; set; }

        public string DatabaseName { get; set; }

        public DatabaseRole DatabaseRole { get; set; } = DatabaseRole.db_datawriter;
    }
}
