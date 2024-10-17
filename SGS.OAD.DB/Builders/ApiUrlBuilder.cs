namespace SGS.OAD.DB
{
    /// <summary>
    /// API URL 建構器
    /// </summary>
    public class ApiUrlBuilder
    {
        private ApiProtocal _protocal = ApiProtocal.HTTP;
        private ApiType _type = ApiType.WCF;
        private string _endpoint;
        private string _server;
        private string _database;
        private ProgramLanguage _language;
        private DatabaseRole _role;
        private readonly string _pattern;

        private ApiUrlBuilder() {
            if (Enum.TryParse(ConfigHelper.GetValue("API_PROTOCAL"), true, out ApiProtocal protocal))
                _protocal = protocal;
            else
                Console.WriteLine($"Can't parse protocal, keep default setting: {_protocal}");

            if (Enum.TryParse(ConfigHelper.GetValue("API_TYPE"), true, out ApiType type))
                _type = type;
            else
                Console.WriteLine($"Can't parse API type, keep default setting: {_type}");

            _endpoint = GetEndpoint(_protocal, _type);
            _pattern = ConfigHelper.GetValue("API_URL_PATTERN");
            _language = ProgramLanguage.Csharp;
            _role = DatabaseRole.db_datawriter;
        }

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

        /// <summary>
        /// 設定 HTTP 或 HTTPS
        /// </summary>
        /// <param name="protocal"></param>
        /// <returns></returns>
        public ApiUrlBuilder SetProtocal(ApiProtocal protocal)
        {
            _protocal = protocal;
            return this;
        }

        /// <summary>
        /// 設定 API 類型為 WCF 或 WebAPI
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ApiUrlBuilder SetType(ApiType type)
        {
            _type = type;
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

        /// <summary>
        /// 取得指定 API 端點
        /// </summary>
        /// <param name="protocal">HTTP or HTTPS</param>
        /// <param name="type">WCF or WebAPI</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private static string GetEndpoint(ApiProtocal protocal, ApiType type)
        {
            return type switch
            {
                ApiType.WCF => protocal switch
                {
                    ApiProtocal.HTTP => ConfigHelper.GetValue("API_ENDPOINT_WCF_HTTP"),
                    ApiProtocal.HTTPS => ConfigHelper.GetValue("API_ENDPOINT_WCF_HTTPS"),
                    _ => throw new NotImplementedException()
                },
                ApiType.WebAPI => protocal switch
                {
                    ApiProtocal.HTTP => ConfigHelper.GetValue("API_ENDPOINT_WEBAPI_HTTP"),
                    ApiProtocal.HTTPS => ConfigHelper.GetValue("API_ENDPOINT_WEBAPI_HTTPS"),
                    _ => throw new NotImplementedException()
                },
                _ => throw new NotImplementedException()
            };
        }
    }
}
