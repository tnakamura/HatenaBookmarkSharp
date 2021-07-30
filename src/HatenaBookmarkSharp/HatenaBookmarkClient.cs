#nullable enable
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace HatenaBookmarkSharp
{
    public sealed class HatenaBookmarkClient : IHatenaBookmarkClient
    {
        readonly HttpClient httpClient;

        readonly static JsonSerializerOptions jsonSerializerOptions;

        static HatenaBookmarkClient()
        {
            jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            jsonSerializerOptions.Converters.Add(DateTimeConverter.Default);
        }

        public HatenaBookmarkClient(
            string consumerKey,
            string consumerSecret,
            string oauthToken,
            string oauthTokenSecret)
            : this(
                  consumerKey,
                  consumerSecret,
                  oauthToken,
                  oauthTokenSecret,
                  new HttpClientHandler())
        {
        }

        public HatenaBookmarkClient(
            string consumerKey,
            string consumerSecret,
            string oauthToken,
            string oauthTokenSecret,
            HttpMessageHandler innerHandler)
        {
            httpClient = new HttpClient(
                new OAuth.OAuthMessageHandler(
                    innerHandler: innerHandler,
                    consumerKey: consumerKey,
                    consumerSecret: consumerSecret,
                    token: new OAuth.AccessToken(
                        key: oauthToken,
                        secret: oauthTokenSecret)));
            httpClient.BaseAddress = new Uri("https://bookmark.hatenaapis.com");
        }

        public Task<Bookmark> GetBookmarkAsync(
            Uri uri,
            CancellationToken cancellationToken = default)
        {
            return GetAsync<Bookmark>(
                $"/rest/1/my/bookmark?url={uri}",
                cancellationToken);
        }

        public Task<Entry> GetEntryAsync(
            Uri uri,
            CancellationToken cancellationToken = default)
        {
            return GetAsync<Entry>(
                $"/rest/1/entry?url={uri}",
                cancellationToken);
        }

        public Task<User> GetMyAsync(CancellationToken cancellationToken = default)
        {
            return GetAsync<User>(
                $"/rest/1/my",
                cancellationToken);
        }

        public async Task<TagsResponse> GetMyTagsAsync(
            CancellationToken cancellationToken = default)
        {
            return await GetAsync<TagsResponse>(
                $"/rest/1/my/tags",
                cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Bookmark> CreateBookmarkAsync(
            NewBookmark bookmark,
            CancellationToken cancellationToken = default)
        {
            var requestBody = new Dictionary<string, string>
            {
                ["url"] = bookmark.Uri!.ToString(),
                ["post_twitter"] = bookmark.IsPostTwitter.ToString(),
                ["post_facebook"] = bookmark.IsPostFacebook.ToString(),
                ["post_mixi"] = bookmark.IsPostMixi.ToString(),
                ["post_evernote"] = bookmark.IsPostEvernote.ToString(),
                ["send_mail"] = bookmark.IsSendMail.ToString(),
                ["private"] = bookmark.IsPrivate.ToString(),
            };
            if (bookmark.Comment != null)
            {
                requestBody["comment"] = bookmark.Comment;
            }

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "/rest/1/my/bookmark")
            {
                Content = new FormUrlEncodedContent(requestBody),
            };

            var response = await SendAsync(request, cancellationToken)
                .ConfigureAwait(false);

            var responseBody = await response.Content.ReadAsStringAsync()
                .ConfigureAwait(false);

            var createdBookmark = Deserialize<Bookmark>(responseBody);

            return createdBookmark;
        }

        public async Task DeleteBookmarkAsync(
            Uri uri,
            CancellationToken cancellationToken = default)
        {
            var escapedUri = Uri.EscapeDataString(uri.ToString()); //httpの場合はURLエンコードしないと削除できない。httpsの場合は、URLエンコードはどちらでもOK。
            var request = new HttpRequestMessage(
                HttpMethod.Delete,
                $"/rest/1/my/bookmark?url={escapedUri}");
            await SendAsync(request, cancellationToken)
                .ConfigureAwait(false);
        }

        async Task<T> GetAsync<T>(
            string requestUri,
            CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var response = await SendAsync(request, cancellationToken)
                .ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            return Deserialize<T>(json);
        }

        async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken = default)
        {
            var response = await httpClient.SendAsync(request, cancellationToken)
                .ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return response;
        }

        static T Deserialize<T>(string json)
        {
#if DEBUG
            if (Debugger.IsAttached)
            {
                Console.WriteLine(json);
            }
#endif
            var result = JsonSerializer.Deserialize<T>(json, jsonSerializerOptions);
            return result!;
        }
    }
}
