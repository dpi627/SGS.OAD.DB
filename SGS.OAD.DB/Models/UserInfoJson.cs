using System.Runtime.Serialization;

namespace SGS.OAD.DB.Models
{
    // Data contract class for JSON deserialization
    [DataContract]
    public class UserInfoJson
    {
        [DataMember(Name = "ID")]
        public string ID { get; set; }

        [DataMember(Name = "PW")]
        public string PW { get; set; }
    }
}
