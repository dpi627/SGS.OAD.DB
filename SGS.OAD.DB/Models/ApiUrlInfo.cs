namespace SGS.OAD.DB
{
    /// <summary>
    /// API URL 相關資訊
    /// </summary>
    public class ApiUrlInfo
    {
        /// <summary>
        /// 資料庫主機名稱
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// 資料庫名稱
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// 應用程式名稱
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 程式語言
        /// </summary>
        public ProgramLanguage Language { get; set; } = ProgramLanguage.Csharp;

        /// <summary>
        /// 角色
        /// </summary>
        public DatabaseRole Role { get; set; } = DatabaseRole.db_datawriter;

        /// <summary>
        /// API 端點網址
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// API 完整網址 (含GET方法參數串接)
        /// </summary>
        public string Url { get; set; }
    }
}
