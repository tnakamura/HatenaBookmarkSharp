using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Xunit;

namespace HatenaBookmarkSharp.Tests
{
    public class HatenaBookmarkClientTest
    {
        [Fact]
        public async Task GetBookmarkAsyncTest()
        {
            var mock = new Mock<HttpMessageHandler>();
            mock.Protected()
                .As<IHttpMessageHandler>()
                .Setup(x => x.SendAsync(
                    It.Is<HttpRequestMessage>(m =>
                        m.Method == HttpMethod.Get &&
                        m.RequestUri == new Uri("https://bookmark.hatenaapis.com/rest/1/my/bookmark?url=https://tnakamura.hatenablog.com/")),
                    It.IsAny<CancellationToken>()))
                .Returns(() =>
                {
                    var now = DateTime.UtcNow;
                    return Task.FromResult(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(
                        $@"{{
                            ""comment"": ""self bookmarked"",
                            ""created_datetime"": ""{now:R}"",
                            ""created_epoch"": 12345,
                            ""user"": ""griefworker"",
                            ""permalink"": ""https://tnakamura.hatenablog.com/"",
                            ""private"": false,
                            ""tags"": [
                              {{
                                 ""name"": ""blog"",
                                 ""count"": 1,
                                 ""modified_datetime"": ""{now:R}"",
                                 ""modified_epoch"": 12345
                              }}
                            ]
                          }}",
                        Encoding.UTF8,
                        "application/json"),
                    });
                });

            var client = new HatenaBookmarkClient(
                new HttpClient(mock.Object));
            var bookmark = await client.GetBookmarkAsync(
                new Uri("https://tnakamura.hatenablog.com"));

            Assert.NotNull(bookmark);
            Assert.Equal("self bookmarked", bookmark.Comment);
            Assert.Single(bookmark.Tags);
            mock.VerifyAll();
        }
    }
}

