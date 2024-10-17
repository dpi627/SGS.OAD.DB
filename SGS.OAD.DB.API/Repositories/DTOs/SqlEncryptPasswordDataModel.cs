#nullable disable

namespace SGS.OAD.DB.API.Repositories.DTOs
{
    public class SqlEncryptPasswordDataModel
    {
        public string ID { get; set; }
        public string PW { get; set; }
        public string EncrptedId { get; set; }
        public string EncrptedPassword { get; set; }
    }
}
