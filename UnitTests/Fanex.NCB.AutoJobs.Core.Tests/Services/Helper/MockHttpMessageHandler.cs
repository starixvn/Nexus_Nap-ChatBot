namespace Fanex.NCB.AutoJobs.Core.Tests.Services.Helper;

using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

public class MockHttpMessageHandler : DelegatingHandler
{
    private readonly HttpResponseMessage httpResponseMessage;

    public MockHttpMessageHandler(HttpResponseMessage responseMessage)
    {
        httpResponseMessage = responseMessage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        return await Task.FromResult(httpResponseMessage);
    }
}