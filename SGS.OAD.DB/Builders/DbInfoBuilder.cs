using SGS.OAD.DB.Enums;
using SGS.OAD.DB.Models;
using SGS.OAD.DB.Services.Implements;
using SGS.OAD.DB.Services.Interfaces;
using SGS.OAD.DB.Utilities;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

namespace SGS.OAD.DB.Builders
{
    /// <summary>
    /// 資料庫資訊建構器
    /// </summary>
    public class DbInfoBuilder
    {
        //private static readonly object _cacheLock = new object();
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
        //private static IList<DbInfo> _dbList = new List<DbInfo>();
        // 使用執行緒安全的集合
        //private static ConcurrentBag<DbInfo> _dbList = new ConcurrentBag<DbInfo>();
        private static ConcurrentDictionary<string, CacheItem> _dbList = new ConcurrentDictionary<string, CacheItem>();
        private static readonly SemaphoreSlim _cacheLock = new SemaphoreSlim(1, 1);
        private const int CacheExpirationMinutes = 5;
        private const int CleanupIntervalMinutes = 2;
        private static TimeSpan CacheExpiration = TimeSpan.FromSeconds(CacheExpirationMinutes);
        private static TimeSpan CleanupInterval = TimeSpan.FromSeconds(CleanupIntervalMinutes);

        private readonly IUserInfoService _userInfoService;
        private readonly IDecryptService _decryptService;

        static DbInfoBuilder()
        {
            // 啟動背景清理服務，每 30 分鐘執行一次，清理過期 1 小時的快取
            StartCacheCleanup(CleanupInterval, CacheExpiration);
        }


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

        /// <summary>
        /// 設定使用者資訊(帳密)
        /// </summary>
        /// <returns></returns>
        private async Task SetUserInfoAsync(CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"[Call Web API] @ {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

            _userInfo = new UserInfo() { Password = "123456", UserId = "admin" };
            _uid = _userInfo.UserId;
            _pwd = _userInfo.Password;
            return;
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

        public void ClearCache()
        {
            ClearCacheAsync().GetAwaiter();
        }

        public async Task ClearCacheAsync()
        {
            await _cacheLock.WaitAsync();
            try
            {
                _dbList.Clear();
                // 避免清除後立刻重建快取時出現競爭條件，加入短暫延遲
                await Task.Delay(100);
            }
            finally
            {
                _cacheLock.Release();
            }
        }

        public DbInfo Build()
        {
            return BuildAsync().GetAwaiter().GetResult();
        }
        public async Task<DbInfo> BuildAsync(CancellationToken cancellationToken = default)
        {
            // Check if a matching DbInfo already exists
            //foreach (var dbInfo in _dbList)
            //{
            //    if (dbInfo.Server == _server && dbInfo.Database == _database)
            //        return dbInfo;
            //}
            var cacheKey = $"{_server}_{_database}";

            await _cacheLock.WaitAsync(cancellationToken);

            try
            {
                // 檢查是否有符合條件的快取項目，並且該項目未過期
                if (_dbList.TryGetValue(cacheKey, out var cacheItem) &&
                !cacheItem.IsExpired(CacheExpiration))
                {
                    Console.WriteLine($"[Cache Status] {DateTime.Now:yyyy-MM-dd HH:mm:ss} Using cache [{cacheKey}]");
                    return cacheItem.DbInfo;
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

                Console.WriteLine($"[Cache Status] {DateTime.Now:yyyy-MM-dd HH:mm:ss} Adding cache [{cacheKey}]");

                _dbList[cacheKey] = new CacheItem(db);

                return db;
            }
            finally
            {
                _cacheLock.Release();
            }
        }

        public DbInfo Rebuild()
        {
            ClearCacheAsync().GetAwaiter();
            return BuildAsync().GetAwaiter().GetResult();
        }
        public async Task<DbInfo> RebuildAsync(CancellationToken cancellationToken = default)
        {
            await ClearCacheAsync().ConfigureAwait(false);
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

        private static void StartCacheCleanup(TimeSpan interval, TimeSpan expirationTime)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(interval);
                    try
                    {
                        await _cacheLock.WaitAsync();
                        try
                        {
                            var now = DateTime.UtcNow;
                            foreach (var key in _dbList.Keys)
                            {
                                if (_dbList.TryGetValue(key, out var cacheItem) && cacheItem.IsExpired(expirationTime))
                                {
                                    _dbList.TryRemove(key, out _);
                                    Console.WriteLine($"\n[Cache Status] {now} Remove Cache item [{key}]");
                                }
                            }
                        }
                        finally
                        {
                            _cacheLock.Release();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\n[Cache Status] Cache cleanup error: {ex.Message}");
                    }
                }
            });
        }

    }
}
