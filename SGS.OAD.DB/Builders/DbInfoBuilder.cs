﻿using SGS.OAD.DB.Services.Implements;
using System.Collections.Concurrent;

namespace SGS.OAD.DB
{
    /// <summary>
    /// 資料庫資訊建構器
    /// </summary>
    public class DbInfoBuilder
    {
        private string _server;
        private string _database;
        private string _uid;
        private string _pwd;
        private string _appName;
        private int _timeout;
        private bool _trustServerCertificate;
        private string _pattern;
        private ProgramLanguage _language = ProgramLanguage.Csharp;
        private DatabaseRole _databeseRole = DatabaseRole.db_datawriter;
        private UserInfo _userInfo;
        private readonly ApiUrlBuilder _apiUrlBuilder;
        private ApiProtocal _protocal;
        private ApiType _type;
        private ApiMethod _method;
        private ApiAlgorithm _algorithm;
        private readonly bool _isEnableLog;

        // 使用執行緒安全的集合
        private static ConcurrentBag<DbInfo> _dbList = new ConcurrentBag<DbInfo>();

        private readonly IUserInfoService _userInfoService;
        private readonly IDecryptService _decryptService;
        private ILogger _logger;

        private DbInfoBuilder(
            IUserInfoService userInfoService = null,
            IDecryptService decryptService = null,
            ILogger logger = null
            )
        {
            // 注入外部服務，如果沒有使用內建服務
            _userInfoService = userInfoService ?? new UserInfoService();
            _decryptService = decryptService ?? new DecryptService();
            _isEnableLog = ConfigHelper.GetValue<bool>("ENABLE_LOG");
            _logger = logger ?? (_isEnableLog ? LoggerBuilder() : new NullLogger());
            // 初始化預設值，讀取設定檔
            _apiUrlBuilder = ApiUrlBuilder.Empty();
            _appName = ConfigHelper.GetValue("APP_NAME");
            _timeout = ConfigHelper.GetValue<int>("DB_TIMEOUT");
            _trustServerCertificate = ConfigHelper.GetValue<bool>("DB_TRUST_SERVER_CERTIFICATE");
            _pattern = ConfigHelper.GetValue("DB_CONNECTION_STRING_PATTERN");
            _protocal = ConfigHelper.GetValue("API_PROTOCAL", _protocal);
            _type = ConfigHelper.GetValue("API_TYPE", _type);
            _method = ConfigHelper.GetValue("API_METHOD", _method);
            _algorithm = ConfigHelper.GetValue("API_ALGORITHM", _algorithm);
        }

        public static DbInfoBuilder Init(
            IUserInfoService userInfoService = null,
            IDecryptService decryptService = null,
            ILogger logger = null
            ) => new(userInfoService, decryptService, logger);

        /// <summary>
        /// 設定伺服器名稱
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        public DbInfoBuilder SetServer(string serverName)
        {
            _server = serverName;
            _apiUrlBuilder.SetServer(serverName);
            return this;
        }

        /// <summary>
        /// 設定資料庫名稱
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public DbInfoBuilder SetDatabase(string databaseName)
        {
            _database = databaseName;
            _apiUrlBuilder.SetDatabase(databaseName);
            return this;
        }

        /// <summary>
        /// 設定程式語言
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public DbInfoBuilder SetLanguage(ProgramLanguage language)
        {
            _language = language;
            _apiUrlBuilder.SetLanguage(language);
            return this;
        }

        /// <summary>
        /// 設定資料庫角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public DbInfoBuilder SetDatabaseRole(DatabaseRole role)
        {
            _databeseRole = role;
            _apiUrlBuilder.SetDatabaseRole(role);
            return this;
        }

        /// <summary>
        /// 設定 API 端點
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public DbInfoBuilder SetApiUrl(Action<ApiUrlBuilder> action)
        {
            action(_apiUrlBuilder);
            return this;
        }

        /// <summary>
        /// 設定應用程式名稱
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        public DbInfoBuilder SetAppName(string appName)
        {
            _appName = appName;
            _apiUrlBuilder.SetAppName(appName);
            return this;
        }

        /// <summary>
        /// 設定連線逾時時間
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public DbInfoBuilder SetTimeout(int timeout)
        {
            _timeout = timeout;
            return this;
        }

        /// <summary>
        /// 設定是否信任伺服器憑證
        /// </summary>
        /// <param name="trustServerCertificate"></param>
        /// <returns></returns>
        public DbInfoBuilder SetTrustServerCertificate(bool trustServerCertificate)
        {
            _trustServerCertificate = trustServerCertificate;
            return this;
        }

        /// <summary>
        /// 設定使用者資訊(帳密)
        /// </summary>
        /// <returns></returns>
        private async Task SetUserInfoAsync(CancellationToken cancellationToken = default)
        {
            // 取得 API 端點
            var apiUrlInfo = _apiUrlBuilder.Build();
            var url = apiUrlInfo.Url;
            // 取得加密的使用者資料
            var userInfoService = new UserInfoService(_logger);
            var encryptedUserInfo = await userInfoService
                .GetEncryptedUserInfoAsync(url, cancellationToken)
                .ConfigureAwait(false);
            // 解密使用者資料
            var decryptService = new DecryptService();
            _userInfo = decryptService.DecryptUserInfo(encryptedUserInfo);
            _uid = _userInfo.UserId;
            _pwd = _userInfo.Password;
        }

        public void ClearCache()
        {
            _logger.LogInformation("Clear Cache");
            _dbList = new ConcurrentBag<DbInfo>();
        }

        public DbInfo Build()
        {
            return BuildAsync().GetAwaiter().GetResult();
        }
        public async Task<DbInfo> BuildAsync(CancellationToken cancellationToken = default)
        {
            // Check if a matching DbInfo already exists
            foreach (var dbInfo in _dbList)
            {
                if (dbInfo.Server == _server && dbInfo.Database == _database)
                {
                    _logger.LogInformation($"DbInfo Found in Cache {Util.SerializeJson(dbInfo)}");
                    return dbInfo;
                }
            }

            // 驗證必要欄位 server name & database name
            ValidateRequiredFields();
            // 取得使用者資訊
            await SetUserInfoAsync(cancellationToken).ConfigureAwait(false);

            var db = new DbInfo()
            {
                Server = _server,
                Database = _database,
                UserId = _userInfo.UserId,
                Password = _userInfo.Password,
                ConnectionTimeout = _timeout,
                TrustServerCertificate = _trustServerCertificate,
                AppName = _appName,
                ConnectionString = Util.ReplaceVariables(
                    _pattern,
                    ("server", _server),
                    ("database", _database),
                    ("uid", _uid),
                    ("pwd", _pwd),
                    ("app", _appName),
                    ("timeout", _timeout),
                    ("trustservercertificate", _trustServerCertificate)
                )
            };

            _dbList.Add(db);
            _logger.LogInformation($"DbInfo Created {Util.SerializeJson(db)}");
            return db;
        }

        public DbInfo Rebuild()
        {
            ClearCache();
            return BuildAsync().GetAwaiter().GetResult();
        }
        public async Task<DbInfo> RebuildAsync(CancellationToken cancellationToken = default)
        {
            ClearCache();
            return await BuildAsync(cancellationToken);
        }

        /// <summary>
        /// 驗證必要欄位
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        private void ValidateRequiredFields()
        {
            if (string.IsNullOrEmpty(_server))
            {
                _logger.LogError("Empty Server Name");
                throw new ArgumentNullException(nameof(_server), "Empty Server Name");
            }
            if (string.IsNullOrEmpty(_database))
            {
                _logger.LogError("Empty Database");
                throw new ArgumentNullException(nameof(_database), "Empty Database");
            }

            if (_protocal == ApiProtocal.HTTP)
            {
                _logger.LogWarning("Recommend to use HTTPS");
            }
            if (_type == ApiType.WCF)
            {
                _logger.LogWarning("Recommend to use WebAPI");
            }
        }

        /// <summary>
        /// 設定 API 通訊協定 HTTP or HTTPS
        /// </summary>
        /// <param name="protocal"></param>
        /// <returns></returns>
        public DbInfoBuilder SetProtocal(ApiProtocal protocal)
        {
            _protocal = protocal;
            _apiUrlBuilder.SetProtocal(protocal);
            return this;
        }

        /// <summary>
        /// 設定 API 類型為 WCF 或 WebAPI
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public DbInfoBuilder SetType(ApiType type)
        {
            _type = type;
            _apiUrlBuilder.SetType(type);
            return this;
        }

        /// <summary>
        /// 設定 API 方法，目前只有 GET，暫時可能用不到
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public DbInfoBuilder SetMethod(ApiMethod method)
        {
            _method = method;
            return this;
        }

        /// <summary>
        /// 設定加密演算法，是 DES 或 AES
        /// </summary>
        /// <param name="algorithm"></param>
        /// <returns></returns>
        public DbInfoBuilder SetAlgorithm(ApiAlgorithm algorithm)
        {
            _algorithm = algorithm;
            _apiUrlBuilder.SetAlgorithm(algorithm);
            return this;
        }

        public DbInfoBuilder EnableLog()
        {
            _logger = LoggerBuilder();
            return this;
        }

        private ILogger LoggerBuilder()
        {
            var fileLogger = new FileLogger(ConfigHelper.GetValue("LOG_PATH"));
            var consoleLogger = new ConsoleLogger();
            return new LoggerFactory(fileLogger, consoleLogger);
        }
    }
}
