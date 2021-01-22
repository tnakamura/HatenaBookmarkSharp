#nullable enable
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

        public async Task<Bookmark> PostBookmarkAsync(
            PostRequest parameter,
            CancellationToken cancellationToken = default)
        {
            var requestBody = new Dictionary<string, string>
            {
                ["url"] = parameter.Uri!.ToString(),
                ["post_twitter"] = parameter.IsPostTwitter.ToString(),
                ["post_facebook"] = parameter.IsPostFacebook.ToString(),
                ["post_mixi"] = parameter.IsPostMixi.ToString(),
                ["post_evernote"] = parameter.IsPostEvernote.ToString(),
                ["send_mail"] = parameter.IsSendMail.ToString(),
                ["private"] = parameter.IsPrivate.ToString(),
            };
            if (parameter.Comment != null)
            {
                requestBody["comment"] = parameter.Comment;
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

            var bookmark = Deserialize<Bookmark>(responseBody);

            return bookmark!;
        }

        public async Task DeleteBookmarkAsync(
            Uri uri,
            CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Delete,
                $"/rest/1/my/bookmark?url={uri}");
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
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            options.Converters.Add(DateTimeConverter.Default);
            var result = JsonSerializer.Deserialize<T>(json, options);
            return result!;
        }
    }
}
