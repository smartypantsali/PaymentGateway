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
    public interface IHttpClientWrapper
    {
        Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content);
        void Dispose();
    }
}
