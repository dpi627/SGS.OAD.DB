namespace SGS.OAD.DB.Enums
{
    /// <summary>
    /// 資料庫角色
    /// </summary>
    public enum DatabaseRole
    {
        /// <summary>
        /// 具備讀寫權限者
        /// </summary>
        db_datawriter = 0,

        /// <summary>
        /// 檔案IO權限角色(非資料庫)
        /// </summary>
        db_filewriter = 1,

        /// <summary>
        /// 具備完整權限者
        /// </summary>
        db_owner = 2
    }
}
