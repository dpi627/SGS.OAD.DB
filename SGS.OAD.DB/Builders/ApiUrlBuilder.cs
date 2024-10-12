using SGS.OAD.DB.Enums;
using SGS.OAD.DB.Models;
using SGS.OAD.DB.Utilities;

namespace SGS.OAD.DB.Builders
{
    /// <summary>
    /// API URL 建構器
    /// </summary>
    public class ApiUrlBuilder
    {
        private string _endpoint = "http://twws006/Decrypt_Server/ServiceDecryptPW.svc/Get_Decrypt4"; //"https://twws006.sgs.net/Decrypt_Server/ServiceDecryptPW.svc/Get_Decrypt4";
        private string _server = string.Empty;
        private string _database = string.Empty;
        private ProgramLanguage _language = ProgramLanguage.Csharp;
        private DatabaseRole _role = DatabaseRole.db_datawriter;
        private readonly string _pattern = "{endpoint}?servername={server}&databasename={database}&planguage={language}&databaserole={role}";

        // 如果要初始化 field 也許寫在 constructor
        private ApiUrlBuilder() { }

        public static ApiUrlBuilder Empty() => new();

        /// <summary>
        /// 設定 API 端點
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        public ApiUrlBuilder SetEndpoint(string endpoint)
        {
            _endpoint = endpoint;
            return this;
        }

        /// <summary>
        /// 設定資料庫主機名稱
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public ApiUrlBuilder SetServer(string server)
        {
            _server = server;
            return this;
        }

        /// <summary>
        /// 設定資料庫名稱
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        public ApiUrlBuilder SetDatabase(string database)
        {
            _database = database;
            return this;
        }

        /// <summary>
        /// 設定程式語言
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public ApiUrlBuilder SetLanguage(ProgramLanguage language)
        {
            _language = language;
            return this;
        }

        /// <summary>
        /// 設定資料庫角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
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
