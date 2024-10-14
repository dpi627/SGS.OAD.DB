using SGS.OAD.DB.Enums;
using SGS.OAD.DB.Models;
using SGS.OAD.DB.Services.Implements;
using SGS.OAD.DB.Services.Interfaces;
using SGS.OAD.DB.Utilities;

namespace SGS.OAD.DB.Builders
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
        private ProgramLanguage _language;
        private DatabaseRole _databeseRole;
        private readonly ApiUrlBuilder _apiUrlBuilder;
        private UserInfo _userInfo;

        // 暫存已經呼叫過的資料
        private static IList<DbInfo> _dbList = new List<DbInfo>();

        private readonly IUserInfoService _userInfoService;
        private readonly IDecryptService _decryptService;

        private DbInfoBuilder(
            IUserInfoService userInfoService = null,
            IDecryptService decryptService = null
            )
        {
            // 注入外部服務，如果沒有使用內建服務
            _userInfoService = userInfoService ?? new UserInfoService();
            _decryptService = decryptService ?? new DecryptService();
            // 初始化預設值
            _apiUrlBuilder = ApiUrlBuilder.Empty();
            _language = ProgramLanguage.Csharp;
            _databeseRole = DatabaseRole.db_datawriter;
            _appName = ConfigHelper.GetValue("APP_NAME");
            _timeout = ConfigHelper.GetValue<int>("DB_TIMEOUT");
            _trustServerCertificate = ConfigHelper.GetValue<bool>("DB_TRUST_SERVER_CERTIFICATE");
            _pattern = ConfigHelper.GetValue("DB_CONNECTION_STRING_PATTERN");
        }

        public static DbInfoBuilder Init(
            IUserInfoService userInfoService = null,
            IDecryptService decryptService = null
            ) => new(userInfoService, decryptService);

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
        int i = 1;
        /// <summary>
        /// 設定使用者資訊(帳密)
        /// </summary>
        /// <returns></returns>
        private async Task SetUserInfoAsync(CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"\n#{i++}: [{DateTime.Now:yyyy-MM-dd HH:mm:ss}] call web api");
            // 取得 API 端點
            var apiUrlInfo = _apiUrlBuilder.Build();
            var url = apiUrlInfo.Url;
            // 取得加密的使用者資料
            var userInfoService = new UserInfoService();
            var encryptedUserInfo = await userInfoService.GetEncryptedUserInfoAsync(url, cancellationToken).ConfigureAwait(false);
            // 解密使用者資料
            var decryptService = new DecryptService();
            _userInfo = decryptService.DecryptUserInfo(encryptedUserInfo);
            _uid = _userInfo.UserId;
            _pwd = _userInfo.Password;
        }

        public DbInfo Build()
        {
            return BuildAsync().GetAwaiter().GetResult();
        }
        public async Task<DbInfo> BuildAsync(CancellationToken cancellationToken = default)
        {
            // Check if a matching DbInfo already exists in _dbList
            if (_dbList.Any())
                foreach (var dbInfo in _dbList)
                    if (dbInfo.Server == _server && dbInfo.Database == _database)
                        return dbInfo;

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

            return db;
        }

        public DbInfo Rebuild()
        {
            _dbList.Clear();
            return BuildAsync().GetAwaiter().GetResult();
        }
        public async Task<DbInfo> RebuildAsync(CancellationToken cancellationToken = default)
        {
            _dbList.Clear();
            return await BuildAsync(cancellationToken);
        }

        /// <summary>
        /// 驗證必要欄位
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        private void ValidateRequiredFields()
        {
            if (string.IsNullOrEmpty(_server))
                throw new ArgumentNullException(nameof(_server), "Empty Server Name");
            if (string.IsNullOrEmpty(_database))
                throw new ArgumentNullException(nameof(_database), "Empty Database");
        }
    }
}
