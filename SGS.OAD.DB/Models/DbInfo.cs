namespace SGS.OAD.DB.Models
{
    /// <summary>
    /// 資料庫資訊
    /// </summary>
    public class DbInfo
    {
        /// <summary>
        /// 伺服器名稱
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// 置資料庫名稱
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// 使用者ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 使用者密碼
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 連線逾時時間
        /// </summary>
        public int ConnectionTimeout { get; set; } = 30;

        /// <summary>
        /// 是否信任伺服器憑證
        /// </summary>
        public bool TrustServerCertificate { get; set; } = true;

        /// <summary>
        /// 應用程式名稱 (預設 SYSOP)
        /// </summary>
        public string AppName { get; set; } = "SYSOP";

        /// <summary>
        /// 資料庫連線字串
        /// </summary>
        public string ConnectionString { get; set; }
    }
}
