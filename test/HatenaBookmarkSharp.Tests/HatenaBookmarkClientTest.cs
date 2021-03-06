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
        const string ConsumerKey = "TestConsumerKey";
        const string ConsumerSecret = "TestConsumerSecret";
        const string OAuthToken = "TestOAuthToken";
        const string OAuthTokenSecret = "OAuthTokenSecret";

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
                            ""created_epoch"": {now.ToUnixTime()},
                            ""user"": ""griefworker"",
                            ""permalink"": ""https://tnakamura.hatenablog.com/"",
                            ""private"": false,
                            ""tags"": [
                              ""blog""
                            ]
                          }}",
                        Encoding.UTF8,
                        "application/json"),
                    });
                });

            var client = new HatenaBookmarkClient(
                consumerKey: ConsumerKey,
                consumerSecret: ConsumerSecret,
                oauthToken: OAuthToken,
                oauthTokenSecret: OAuthTokenSecret,
                innerHandler: mock.Object);
            var bookmark = await client.GetBookmarkAsync(
                new Uri("https://tnakamura.hatenablog.com"));

            Assert.NotNull(bookmark);
            Assert.Equal("self bookmarked", bookmark.Comment);
            Assert.Single(bookmark.Tags);
            Assert.Equal("blog", bookmark.Tags[0]);
            mock.VerifyAll();
        }

        [Fact]
        public async Task DeleteBookmarkAsyncTest()
        {
            var mock = new Mock<HttpMessageHandler>();
            mock.Protected()
                .As<IHttpMessageHandler>()
                .Setup(x => x.SendAsync(
                    It.Is<HttpRequestMessage>(m =>
                        m.Method == HttpMethod.Delete &&
                        m.RequestUri == new Uri("https://bookmark.hatenaapis.com/rest/1/my/bookmark?url=https://tnakamura.hatenablog.com/")),
                    It.IsAny<CancellationToken>()))
                .Returns(() =>
                {
                    var now = DateTime.UtcNow;
                    return Task.FromResult(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.NoContent,
                    });
                });

            var client = new HatenaBookmarkClient(
                consumerKey: ConsumerKey,
                consumerSecret: ConsumerSecret,
                oauthToken: OAuthToken,
                oauthTokenSecret: OAuthTokenSecret,
                innerHandler: mock.Object);
            await client.DeleteBookmarkAsync(
                new Uri("https://tnakamura.hatenablog.com"));

            mock.VerifyAll();
        }

        [Fact]
        public async Task GetMyAsyncTest()
        {
            var mock = new Mock<HttpMessageHandler>();
            mock.Protected()
                .As<IHttpMessageHandler>()
                .Setup(x => x.SendAsync(
                    It.Is<HttpRequestMessage>(m =>
                        m.Method == HttpMethod.Get &&
                        m.RequestUri == new Uri("https://bookmark.hatenaapis.com/rest/1/my")),
                    It.IsAny<CancellationToken>()))
                .Returns(() =>
                {
                    return Task.FromResult(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(
                        $@"{{
                            ""name"": ""griefworker"",
                            ""plususer"": true,
                            ""private"": true,
                            ""is_oauth_twitter"": true,
                            ""is_oauth_evernote"": true,
                            ""is_oauth_facebook"": true,
                            ""is_oauth_mixi_check"": true
                          }}",
                        Encoding.UTF8,
                        "application/json"),
                    });
                });

            var client = new HatenaBookmarkClient(
                consumerKey: ConsumerKey,
                consumerSecret: ConsumerSecret,
                oauthToken: OAuthToken,
                oauthTokenSecret: OAuthTokenSecret,
                innerHandler: mock.Object);
            var user = await client.GetMyAsync();

            Assert.NotNull(user);
            Assert.Equal("griefworker", user.Name);
            Assert.True(user.IsPrivate);
            Assert.True(user.IsPlusUser);
            Assert.True(user.IsOAuthTwitter);
            Assert.True(user.IsOAuthEvernote);
            Assert.True(user.IsOAuthFacebook);
            Assert.True(user.IsOAuthMixiCheck);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetEntryAsyncTest()
        {
            var mock = new Mock<HttpMessageHandler>();
            mock.Protected()
                .As<IHttpMessageHandler>()
                .Setup(x => x.SendAsync(
                    It.Is<HttpRequestMessage>(m =>
                        m.Method == HttpMethod.Get &&
                        m.RequestUri == new Uri("https://bookmark.hatenaapis.com/rest/1/entry?url=https://tnakamura.hatenablog.com/")),
                    It.IsAny<CancellationToken>()))
                .Returns(() =>
                {
                    return Task.FromResult(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(
                        $@"{{
                            ""title"": ""present"",
                            ""url"": ""https://tnakamura.hatenablog.com"",
                            ""entry_url"": ""https://tnakamura.hatenablog.com"",
                            ""count"": 1,
                            ""favicon_Url"": ""https://tnakamura.hatenablog.com/favicon.ico"",
                            ""smartphone_app_entry_url"": ""https://tnakamura.hatenablog.com/mobile""
                          }}",
                        Encoding.UTF8,
                        "application/json"),
                    });
                });

            var client = new HatenaBookmarkClient(
                consumerKey: ConsumerKey,
                consumerSecret: ConsumerSecret,
                oauthToken: OAuthToken,
                oauthTokenSecret: OAuthTokenSecret,
                innerHandler: mock.Object);
            var entry = await client.GetEntryAsync(
                new Uri("https://tnakamura.hatenablog.com/"));

            Assert.NotNull(entry);
            Assert.Equal("present", entry.Title);
            Assert.Equal(1, entry.Count);
            Assert.Equal(
                new Uri("https://tnakamura.hatenablog.com"),
                entry.Url);
            Assert.Equal(
                new Uri("https://tnakamura.hatenablog.com"),
                entry.EntryUrl);
            Assert.Equal(
                new Uri("https://tnakamura.hatenablog.com/favicon.ico"),
                entry.FaviconUrl);
            Assert.Equal(
                new Uri("https://tnakamura.hatenablog.com/mobile"),
                entry.SmartPhoneAppEntryUrl);
            mock.VerifyAll();
        }

        [Fact]
        public async Task GetMyTagsAsyncTest()
        {
            var mock = new Mock<HttpMessageHandler>();
            mock.Protected()
                .As<IHttpMessageHandler>()
                .Setup(x => x.SendAsync(
                    It.Is<HttpRequestMessage>(m =>
                        m.Method == HttpMethod.Get &&
                        m.RequestUri == new Uri("https://bookmark.hatenaapis.com/rest/1/my/tags")),
                    It.IsAny<CancellationToken>()))
                .Returns(() =>
                {
                    var now = DateTime.UtcNow;
                    return Task.FromResult(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(
                        $@"{{
                             ""tags"": [
                               {{
                                   ""tag"": ""blog"",
                                   ""count"": 1
                               }},
                               {{
                                   ""tag"": ""csharp"",
                                   ""count"": 2
                               }}
                              ]
                            }}",
                        Encoding.UTF8,
                        "application/json"),
                    });
                });

            var client = new HatenaBookmarkClient(
                consumerKey: ConsumerKey,
                consumerSecret: ConsumerSecret,
                oauthToken: OAuthToken,
                oauthTokenSecret: OAuthTokenSecret,
                innerHandler: mock.Object);
            var response = await client.GetMyTagsAsync();

            Assert.NotNull(response);
            Assert.Equal(2, response.Tags.Count);
            Assert.Equal("blog", response.Tags[0].Name);
            Assert.Equal("csharp", response.Tags[1].Name);
            mock.VerifyAll();
        }

        [Fact]
        public async Task PostBookmarkAsyncTest()
        {
            var mock = new Mock<HttpMessageHandler>();
            mock.Protected()
                .As<IHttpMessageHandler>()
                .Setup(x => x.SendAsync(
                    It.Is<HttpRequestMessage>(m =>
                        m.Method == HttpMethod.Post &&
                        m.RequestUri == new Uri("https://bookmark.hatenaapis.com/rest/1/my/bookmark") &&
                        m.Content.ReadAsStringAsync().Result.Contains("url") &&
                        m.Content.ReadAsStringAsync().Result.Contains("comment") &&
                        m.Content.ReadAsStringAsync().Result.Contains("self+bookmarked") &&
                        m.Content.ReadAsStringAsync().Result.Contains("private")),
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
                            ""created_epoch"": {now.ToUnixTime()},
                            ""user"": ""griefworker"",
                            ""permalink"": ""https://tnakamura.hatenablog.com/"",
                            ""private"": true,
                            ""tags"": [
                              ""blog""
                            ]
                          }}",
                        Encoding.UTF8,
                        "application/json"),
                    });
                });

            var client = new HatenaBookmarkClient(
                consumerKey: ConsumerKey,
                consumerSecret: ConsumerSecret,
                oauthToken: OAuthToken,
                oauthTokenSecret: OAuthTokenSecret,
                innerHandler: mock.Object);
            var bookmark = await client.CreateBookmarkAsync(
                new NewBookmark(new Uri("https://tnakamura.hatenablog.com"))
                {
                    Comment = "[blog]self bookmarked",
                    IsPostMixi = false,
                    IsPostEvernote = true,
                    IsPostTwitter = true,
                    IsPostFacebook = false,
                    IsPrivate = true,
                    IsSendMail = false,
                });

            Assert.NotNull(bookmark);
            Assert.Equal("self bookmarked", bookmark.Comment);
            Assert.Equal(
                new Uri("https://tnakamura.hatenablog.com/"),
                bookmark.Permalink);
            Assert.True(bookmark.IsPrivate);
            mock.VerifyAll();
        }

        //[Fact]
        //public void GenerateAuthenticationUriTest()
        //{
        //    var client = new HatenaBookmarkClient();
        //    var uri = client.GenerateAuthenticationUri("QB%2FfqbXTpFB1GQ%3D%3D");
        //    Assert.Equal(
        //        new Uri("https://www.hatena.ne.jp/oauth/authorize?oauth_token=QB%2FfqbXTpFB1GQ%3D%3D"),
        //        uri);
        //}
    }
}

