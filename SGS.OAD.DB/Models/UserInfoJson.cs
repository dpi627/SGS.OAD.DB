using System.Runtime.Serialization;

namespace SGS.OAD.DB
{
    /// <summary>
    /// 承接 API 回傳加密資料
    /// </summary>
    [DataContract]
    public class UserInfoJson
    {
        /// <summary>
        /// ID (加密)
        /// </summary>
        [DataMember(Name = "ID")]
        public string ID { get; set; }

        /// <summary>
        /// 密碼 (加密)
        /// </summary>
        [DataMember(Name = "PW")]
        public string PW { get; set; }
    }
}
