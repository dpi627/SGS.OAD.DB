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
        private string _server = string.Empty;
        private string _database = string.Empty;
        private string _uid = string.Empty;
        private string _pwd = string.Empty;
        private string _appName = "SYSOP"; // system operation
        private int _timeout = 30;
        private bool _trustServerCertificate = true;
        private readonly string _pattern = "Data Source={server};Initial Catalog={database};User ID={uid};Password={pwd};Application Name={app};Connect Timeout={timeout};TrustServerCertificate={servercertificate};";
        private ProgramLanguage _language = ProgramLanguage.Csharp;
        private DatabaseRole _databeseRole = DatabaseRole.db_datawriter;
        public readonly ApiUrlBuilder _apiUrlBuilder = ApiUrlBuilder.Empty();
        public UserInfo _userInfo;

        private readonly IUserInfoService _userInfoService;
        private readonly IDecryptService _decryptService;

        private DbInfoBuilder()
        {
            // 直接使用內建
            _userInfoService = new UserInfoService();
            _decryptService = new DecryptService();
        }

        public static DbInfoBuilder Init() => new();

        private DbInfoBuilder(
            IUserInfoService userInfoService,
            IDecryptService decryptService
            )
        {
            // 注入外部服務，如果沒有使用內建服務
            _userInfoService = userInfoService ?? throw new ArgumentNullException(nameof(userInfoService));
            _decryptService = decryptService ?? throw new ArgumentNullException(nameof(decryptService));
        }

        public static DbInfoBuilder Init(
            IUserInfoService userInfoService,
            IDecryptService decryptService
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
        /// <param name="serverCertificate"></param>
        /// <returns></returns>
        public DbInfoBuilder SetServerCertificate(bool serverCertificate)
        {
            _trustServerCertificate = serverCertificate;
            return this;
        }

        /// <summary>
        /// 設定使用者資訊(帳密)
        /// </summary>
        /// <returns></returns>
        private async Task SetUserInfoAsync()
        {
            // 取得 API 端點
            var apiUrlInfo = _apiUrlBuilder.Build();
            var url = apiUrlInfo.Url;
            // 取得加密的使用者資料
            var userInfoService = new UserInfoService();
            var encryptedUserInfo = await userInfoService.GetEncryptedUserInfoAsync(url).ConfigureAwait(false);
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
        public async Task<DbInfo> BuildAsync()
        {
            // 驗證必要欄位
            ValidateRequiredFields();
            // 取得使用者資訊
            await SetUserInfoAsync().ConfigureAwait(false);

            return new DbInfo()
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
                    ("servercertificate", _trustServerCertificate)
                )
            };
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
