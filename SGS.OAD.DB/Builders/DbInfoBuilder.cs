using SGS.OAD.DB.Enums;
using SGS.OAD.DB.Models;
using SGS.OAD.DB.Services.Implements;
using SGS.OAD.DB.Services.Interfaces;
using SGS.OAD.DB.Utilities;

namespace SGS.OAD.DB.Builders
{
    public class DbInfoBuilder : IDisposable
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
        private readonly ILogger _logger;
        private bool _disposed = false;

        private readonly IUserInfoService _userInfoService;
        private readonly IDecryptService _decryptService;

        private DbInfoBuilder(ILogger logger = null)
        {
            _userInfoService = new UserInfoService(logger);
            _decryptService = new DecryptService();
            _logger = logger ?? new Logger();
        }

        public static DbInfoBuilder Init(ILogger logger = null) => new(logger);

        private DbInfoBuilder(
            IUserInfoService userInfoService,
            IDecryptService decryptService,
            ILogger logger = null
            )
        {
            _userInfoService = userInfoService ?? throw new ArgumentNullException(nameof(userInfoService));
            _decryptService = decryptService ?? throw new ArgumentNullException(nameof(decryptService));
            _logger = logger ?? new Logger();
        }

        public static DbInfoBuilder Init(
            IUserInfoService userInfoService,
            IDecryptService decryptService, ILogger logger = null
            ) => new(userInfoService, decryptService, logger);

        public DbInfoBuilder SetServer(string serverName)
        {
            _server = serverName;
            _apiUrlBuilder.SetServer(serverName);
            return this;
        }
        public DbInfoBuilder SetDatabase(string databaseName)
        {
            _database = databaseName;
            _apiUrlBuilder.SetDatabase(databaseName);
            return this;
        }
        public DbInfoBuilder SetLanguage(ProgramLanguage language)
        {
            _language = language;
            _apiUrlBuilder.SetLanguage(language);
            return this;
        }
        public DbInfoBuilder SetDatabaseRole(DatabaseRole role)
        {
            _databeseRole = role;
            _apiUrlBuilder.SetDatabaseRole(role);
            return this;
        }
        public DbInfoBuilder SetApiUrl(Action<ApiUrlBuilder> action)
        {
            action(_apiUrlBuilder);
            return this;
        }
        public DbInfoBuilder SetAppName(string appName)
        {
            _appName = appName;
            return this;
        }
        public DbInfoBuilder SetTimeout(int timeout)
        {
            _timeout = timeout;
            return this;
        }
        public DbInfoBuilder SetServerCertificate(bool serverCertificate)
        {
            _trustServerCertificate = serverCertificate;
            return this;
        }

        private async Task SetUserInfoAsync()
        {
            var apiUrlInfo = _apiUrlBuilder.Build();
            var url = apiUrlInfo.Url;

            var userInfoService = new UserInfoService(_logger);
            var encryptedUserInfo = await userInfoService.GetEncryptedUserInfoAsync(url).ConfigureAwait(false);

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
            _logger.LogInformation($"[{_appName}] ask for [{_server}/{_database}]");

            ValidateRequiredFields();
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

        private void ValidateRequiredFields()
        {
            if (string.IsNullOrEmpty(_server))
                throw new ArgumentNullException(nameof(_server), "Empty Server Name");
            if (string.IsNullOrEmpty(_database))
                throw new ArgumentNullException(nameof(_database), "Empty Database");
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                // 確保當 DbInfoBuilder 被釋放時，若使用 QueueLogger，則會等待所有日誌處理完畢。
                if (_logger is Logger logger)
                {
                    logger.FlushAndDispose();
                }

                _disposed = true;
            }
        }
    }
}
