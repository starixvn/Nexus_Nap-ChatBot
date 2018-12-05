using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NCB.Core.Services
{
    public interface IBridgeClientService
    {
        Task<HttpResponseMessage> PostJsonAsync<T>(string url, T value);
        Task<HttpResponseMessage> GetJsonAsync(string url);
    }
}
