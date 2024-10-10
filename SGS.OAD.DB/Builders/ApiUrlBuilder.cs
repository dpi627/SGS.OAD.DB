using SGS.OAD.DB.Models;

namespace SGS.OAD.DB.Builders
{
    public class ApiUrlBuilder
    {
        private string _url = @"http://twws006/Decrypt_Server/ServiceDecryptPW.svc/Get_Decrypt4";
        private ApiRequestModel _req = default;
        private string _urlTemplate = @"{0}?servername={1}&databasename={2}&planguage={3}&databaserole={4}";

        private ApiUrlBuilder() { }

        public static ApiUrlBuilder Init() => new();

        public ApiUrlBuilder SetUrl(string url)
        {
            _url = url;
            return this;
        }

        public ApiUrlBuilder SetParameter(ApiRequestModel req)
        {
            _req = req;
            return this;
        }

        public ApiUrlBuilder SetTemplate(string connectionStringTemplate)
        {
            _urlTemplate = connectionStringTemplate;
            return this;
        }

        public string Build()
        {
            return string.Format(
                _urlTemplate,
                _url,
                _req.ServerName,
                _req.DatabaseName,
                _req.ProgramLanguage.ToString(),
                _req.DatabaseRole.ToString()
                );
        }
    }
}
