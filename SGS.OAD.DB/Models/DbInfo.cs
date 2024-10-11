namespace SGS.OAD.DB.Models
{
    public class DbInfo
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public int ConnectionTimeout { get; set; }
        public bool TrustServerCertificate { get; set; }
        public string AppName { get; set; }
        public string ConnectionString { get; set; }
    }
}
