namespace SGS.OAD.DB
{
    /// <summary>
    /// 使用者資料服務
    /// </summary>
    public interface IUserInfoService
    {
        /// <summary>
        /// 取得加密的使用者資料
        /// </summary>
        /// <param name="url">API端點</param>
        /// <param name="cancellationToken"></param>
        /// <returns>使用者資料(帳密加密)</returns>
        UserInfo GetEncryptedUserInfo(string url, CancellationToken cancellationToken = default);

        /// <summary>
        /// 取得加密的使用者資料
        /// </summary>
        /// <param name="url">API端點</param>
        /// <param name="cancellationToken"></param>
        /// <returns>使用者資料(帳密加密)</returns>
        Task<UserInfo> GetEncryptedUserInfoAsync(string url, CancellationToken cancellationToken = default);
    }
}
