using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SGS.OAD.DB.Services
{
    public class ApiService
    {
        public Task<string> FetchUrlAsync(string url)
        {
            HttpClient client = new HttpClient();
            return client.GetAsync(url)
                .ContinueWith(responseTask =>
                {
                    HttpResponseMessage response = responseTask.Result;
                    response.EnsureSuccessStatusCode();
                    return response.Content.ReadAsStringAsync();
                }).Unwrap();
        }
    }
}
