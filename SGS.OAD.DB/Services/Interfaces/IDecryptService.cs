namespace SGS.OAD.DB
{
    /// <summary>
    /// 解密服務
    /// </summary>
    public interface IDecryptService
    {
        /// <summary>
        /// 解密使用者資料
        /// </summary>
        /// <param name="encryptedUserInfo">加密資料</param>
        /// <returns>解密資料</returns>
        UserInfo DecryptUserInfo(UserInfo encryptedUserInfo);
    }
}
