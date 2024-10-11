using SGS.OAD.DB.Enums;
using SGS.OAD.DB.Models;
using SGS.OAD.DB.Services.Implements;
using SGS.OAD.DB.Services.Interfaces;
using SGS.OAD.DB.Utilities;

namespace SGS.OAD.DB.Builders
{
    public class DbInfoBuilder
    {
        private string _server = string.Empty;
        private string _database = string.Empty;
        private string _uid = string.Empty;
        private string _pwd = string.Empty;
        private string _app = "SYSOP"; // system operation
        private int _timeout = 30;
        private bool _trustServerCertificate = true;
        private readonly string _pattern = "Data Source={server};Initial Catalog={database};User ID={uid};Password={pwd};Application Name={app};Connect Timeout={timeout};TrustServerCertificate={servercertificate};";
        private ProgramLanguage _language = ProgramLanguage.Csharp; // "CSharp";
        private DatabaseRole _role = DatabaseRole.db_datawriter; // "db_datawriter";
        public readonly ApiUrlBuilder _apiUrlBuilder = ApiUrlBuilder.Empty();
        public UserInfo _userInfo = default;

        private readonly IUserInfoService _userInfoService;
        private readonly IDecryptService _decryptService;

        private DbInfoBuilder()
        {
            _userInfoService = new UserInfoService();
            _decryptService = new DecryptService();
        }

        public static DbInfoBuilder Init() => new();

        private DbInfoBuilder(IUserInfoService userInfoService, IDecryptService decryptService)
        {
            _userInfoService = userInfoService ?? throw new ArgumentNullException(nameof(userInfoService));
            _decryptService = decryptService ?? throw new ArgumentNullException(nameof(decryptService));
        }
        public static DbInfoBuilder Init(IUserInfoService userInfoService, IDecryptService decryptService) => new(userInfoService, decryptService);

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
            _role = role;
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
            _app = appName;
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

            var userInfoService = new UserInfoService();
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
                AppName = _app,
                ConnectionString = Util.ReplaceVariables(
                    _pattern,
                    ("server", _server),
                    ("database", _database),
                    ("uid", _uid),
                    ("pwd", _pwd),
                    ("app", _app),
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
    }
}
