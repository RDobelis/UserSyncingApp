using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace UserSyncingApp.ServiceInterface.Tests.Mocks;

public class MockHttpMessageHandler : HttpMessageHandler
{
    private HttpResponseMessage _responseMessage;

    public MockHttpMessageHandler(HttpResponseMessage responseMessage)
    {
        _responseMessage = responseMessage;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => 
        Task.FromResult(_responseMessage);
}