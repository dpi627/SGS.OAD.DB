using SGS.OAD.DB.Enums;
using SGS.OAD.DB.Models;
using SGS.OAD.DB.Utilities;

namespace SGS.OAD.DB.Builders
{
    public class ApiUrlBuilder
    {
        private string _endpoint = "http://twws006/Decrypt_Server/ServiceDecryptPW.svc/Get_Decrypt4"; //"https://twws006.sgs.net/Decrypt_Server/ServiceDecryptPW.svc/Get_Decrypt4";
        private string _server = string.Empty;
        private string _database = string.Empty;
        private ProgramLanguage _language = ProgramLanguage.Csharp; // "CSharp";
        private DatabaseRole _role = DatabaseRole.db_datawriter; // "db_datawriter";
        private readonly string _pattern = "{endpoint}?servername={server}&databasename={database}&planguage={language}&databaserole={role}";

        // 如果要初始化 field 也許寫在 constructor
        private ApiUrlBuilder() { }

        public static ApiUrlBuilder Empty() => new();

        public ApiUrlBuilder SetEndpoint(string endpoint)
        {
            _endpoint = endpoint;
            return this;
        }
        public ApiUrlBuilder SetServer(string server)
        {
            _server = server;
            return this;
        }
        public ApiUrlBuilder SetDatabase(string database)
        {
            _database = database;
            return this;
        }
        public ApiUrlBuilder SetLanguage(ProgramLanguage language)
        {
            _language = language;
            return this;
        }
        public ApiUrlBuilder SetDatabaseRole(DatabaseRole role)
        {
            _role = role;
            return this;
        }

        public ApiUrlInfo Build()
        {
            return new ApiUrlInfo()
            {
                Endpoint = _endpoint,
                Server = _server,
                Database = _database,
                Language = _language,
                Role = _role,
                Url = Util.ReplaceVariables(
                    _pattern,
                    ("endpoint", _endpoint),
                    ("server", _server),
                    ("database", _database),
                    ("language", _language),
                    ("role", _role)
                )
            };
        }
    }
}
