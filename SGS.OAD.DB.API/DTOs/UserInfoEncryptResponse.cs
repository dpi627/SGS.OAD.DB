#nullable disable

namespace SGS.OAD.DB.API.DTOs
{
    /// <summary>
    /// 資料庫使用者資訊
    /// </summary>
    public class UserInfoEncryptResponse
    {
        /// <summary>
        /// 使用者帳號(加密)
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 使用者密碼(加密)
        /// </summary>
        public string PW { get; set; }
    }
}
