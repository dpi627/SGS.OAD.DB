using SGS.OAD.DB.Services;

namespace SGS.OAD.DB.Builders
{
    public class ConnectionStringBuilder
    {
        private string _server = string.Empty;
        private string _database = string.Empty;
        private string _connectionStringTemplate = @"server={0}, db={1}, id={2}, pw={3}";
        private readonly ApiUrlBuilder _apiUrlBuilder = ApiUrlBuilder.Init();
        private readonly ApiService _apiService = new ApiService();

        private ConnectionStringBuilder() { }

        public static ConnectionStringBuilder Init() => new();

        public ConnectionStringBuilder SetSever(string serverName)
        {
            _server = serverName;
            return this;
        }

        public ConnectionStringBuilder SetDatabase(string databaseName)
        {
            _database = databaseName;
            return this;
        }

        public ConnectionStringBuilder SetTemplate(string connectionStringTemplate)
        {
            _connectionStringTemplate = connectionStringTemplate;
            return this;
        }

        public ConnectionStringBuilder SetApi(Action<ApiUrlBuilder> action)
        {
            action(_apiUrlBuilder);
            return this;
        }

        private Task<string> FetchDataAsync()
        {
            var url = _apiUrlBuilder.Build();
            return _apiService.FetchUrlAsync(url).ContinueWith(t => t.Result);
        }

        public string Build()
        {
            //var url = _apiUrlBuilder.Build();
            var data = FetchDataAsync().GetAwaiter().GetResult();
            Console.WriteLine(data);
            return string.Format(_connectionStringTemplate, _server, _database, data, data);
        }
    }
}