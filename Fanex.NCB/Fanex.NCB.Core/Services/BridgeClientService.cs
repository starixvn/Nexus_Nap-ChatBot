using Fanex.NCB.Core.Models.DataDomain;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NCB.Core.Services
{
    public class BridgeClientService : IBridgeClientService
    {

        private readonly AppSettings _options;

        public BridgeClientService(IOptions<AppSettings> options)
        {
            _options = options.Value;
        }
        public async Task<HttpResponseMessage> PostJsonAsync<T>(string url, T value)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-authKey", _options.Secret);
            var content = new StringContent(JsonConvert.SerializeObject(value),
                Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            response.EnsureSuccessStatusCode();

            return response;
        }
    }
}
