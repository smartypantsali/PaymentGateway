using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Framework.WebUtilities
{
    /// <summary>
    /// Wrapper created to be able to use DI with HttpClient
    /// </summary>
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private HttpClient _httpClient = new HttpClient();

        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content) => _httpClient.PostAsync(requestUri, content);
        public void Dispose() => _httpClient.Dispose();
    }
}
