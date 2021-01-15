using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HatenaBookmarkSharp.Tests
{
    public interface IHttpMessageHandler
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken);
    }
}
