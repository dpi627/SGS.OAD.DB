namespace SGS.OAD.DB
{
    /// <summary>
    /// API URL 建構器
    /// </summary>
    public class ApiUrlBuilder
    {
        private ApiProtocal _protocal = ApiProtocal.HTTPS;
        private ApiType _type = ApiType.WebAPI;
        private ApiAlgorithm _algorithm = ApiAlgorithm.DES;
        private string _endpoint;
        private string _server;
        private string _database;
        private string _appName;
        private ProgramLanguage _language = ProgramLanguage.Csharp;
        private DatabaseRole _role = DatabaseRole.db_datawriter;
        private readonly string _pattern;

        private ApiUrlBuilder() {
            // 取得預設應用程式名稱
            _appName = ConfigHelper.GetValue("APP_NAME");
            // 先設定 Http 或 Https 以及 WCF 或 WebAPI
            _protocal = ConfigHelper.GetValue("API_PROTOCAL", _protocal);
            _type = ConfigHelper.GetValue("API_TYPE", _type);
            // 才能取得 API 端點
            _endpoint = GetEndpoint(_protocal, _type);
            _pattern = ConfigHelper.GetValue("API_URL_PATTERN");
            _algorithm = ConfigHelper.GetValue("API_ALGORITHM", _algorithm);
        }

        public static ApiUrlBuilder Empty() => new();

        /// <summary>
        /// 設定 API 端點
        /// 預設透過 _protocal 和 _type 取得，但也開放自訂
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

        public ApiUrlBuilder SetAppName(string appName)
        {
            _appName = appName;
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

        /// <summary>
        /// 設定加密演算法
        /// </summary>
        /// <param name="algorithm"></param>
        /// <returns></returns>
        public ApiUrlBuilder SetAlgorithm(ApiAlgorithm algorithm)
        {
            _algorithm = algorithm;
            return this;
        }

        public ApiUrlInfo Build()
        {
            return new ApiUrlInfo()
            {
                Endpoint = _endpoint,
                Server = _server,
                Database = _database,
                AppName = _appName,
                Language = _language,
                Role = _role,
                Url = Util.ReplaceVariables(
                    _pattern,
                    ("endpoint", _endpoint),
                    ("server", _server),
                    ("database", _database),
                    ("app", _appName),
                    ("language", _language),
                    ("role", _role),
                    ("algorithm", _algorithm)
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
