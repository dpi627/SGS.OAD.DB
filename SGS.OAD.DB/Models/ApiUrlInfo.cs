using SGS.OAD.DB.Enums;

namespace SGS.OAD.DB.Models
{
    public class ApiUrlInfo
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public ProgramLanguage Language { get; set; }
        public DatabaseRole Role { get; set; }
        public string Endpoint { get; set; }
        public string Url { get; set; }
    }
}
