namespace Fanex.NCB.Services.Tests.Helper
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public class MockHttpMessageHandler : DelegatingHandler
    {
        private readonly IDictionary<string, HttpResponseMessage> messages;

        public MockHttpMessageHandler(IDictionary<string, HttpResponseMessage> messages)
        {
            this.messages = messages;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.NotFound);
            if (messages.Keys.Any(key => request.RequestUri.ToString().Contains(key)))
            {
                string key = messages.Keys.FirstOrDefault(k => request.RequestUri.ToString().Contains(k));
                response = messages[key] ?? new HttpResponseMessage(HttpStatusCode.NoContent);
            }

            response.RequestMessage = request;
            return Task.FromResult(response);
        }
    }
}