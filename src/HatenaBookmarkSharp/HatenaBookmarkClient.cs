#nullable enable
using System.Security.Cryptography;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HatenaBookmarkSharp.Models;

namespace HatenaBookmarkSharp
{
    public partial class HatenaBookmarkClient : IHatenaBookmarkClient
    {
        readonly HttpClient httpClient;

        readonly HatenaBookmarkClientOptions options;

        public HatenaBookmarkClient(
            HttpClient? httpClient = null,
            HatenaBookmarkClientOptions? options = null)
        {
            this.options = options ?? new HatenaBookmarkClientOptions(); ;
            this.httpClient = httpClient ?? new HttpClient();
            this.httpClient.BaseAddress = new Uri("https://bookmark.hatenaapis.com");
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

        public Task<IReadOnlyList<Tag>> GetMyTagsAsync(
            CancellationToken cancellationToken = default)
        {
            return GetAsync<IReadOnlyList<Tag>>(
                $"/rest/1/my/tags",
                cancellationToken);
        }

        public async Task<Bookmark> PostBookmarkAsync(
            PostRequest parameter,
            CancellationToken cancellationToken = default)
        {
            var requestBody = JsonSerializer.Serialize(parameter);
            var response = await httpClient.PostAsync(
                $"/rest/1/my/bookmark",
                new StringContent(
                    requestBody,
                    Encoding.UTF8,
                    "application/json"),
                cancellationToken)
                .ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var bookmark = Deserialize<Bookmark>(responseBody);
            return bookmark!;
        }

        public async Task DeleteBookmarkAsync(
            Uri uri,
            CancellationToken cancellationToken = default)
        {
            var response = await httpClient
                .DeleteAsync(
                    $"/rest/1/my/bookmark?url={uri}",
                    cancellationToken)
                .ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        async Task<T> GetAsync<T>(
            string requestUri,
            CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetAsync(requestUri, cancellationToken)
                .ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            return Deserialize<T>(json);
        }

        static T Deserialize<T>(string json)
        {
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            options.Converters.Add(DateTimeConverter.Default);
            var result = JsonSerializer.Deserialize<T>(json, options);
            return result!;
        }

        public async Task<RequestToken> GetRequestTokenAsync(CancellationToken cancellationToken = default)
        {
            var content = new FormUrlEncodedContent(
                new Dictionary<string, string>()
                {
                    ["scope"] = "read_public,read_private",
                });

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://www.hatena.com/oauth/initiate")
            {
                Content = content,
            };

            var bytes = await content.ReadAsByteArrayAsync();
            var hmacsha1 = new HMACSHA1(Encoding.UTF8.GetBytes(options.OAuthConsumerSecret));
            var hash = hmacsha1.ComputeHash(bytes);
            var oauthSignature = Convert.ToBase64String(hash);
            var nonce = Guid.NewGuid().ToString("N");
            var timestamp = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            var oauthParameter = "realm=\"\","
                + "oauth_callback=\"oob\","
                + "oauth_consumer_key=\"" + options.OAuthConsumerKey + "\","
                + "oauth_nonce=\"" + nonce + "\","
                + "oauth_signature=\"" + oauthSignature + "\","
                + "oauth_signature_method=\"HMAC-SHA1\","
                + "oauth_timestamp=\"" + timestamp + "\","
                + "oauth_version=\"1.0\"";
            request.Headers.Authorization = new AuthenticationHeaderValue(
                scheme: "OAuth",
                parameter: oauthParameter);

            var response = await httpClient.SendAsync(request, cancellationToken);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            var parsedBody = ParseFormUrlEncoded(body);

            return new RequestToken
            {
                OAuthToken = parsedBody["oauth_token"],
                OAuthTokenSecret = parsedBody["oauth_token_secret"],
                OAuthCallbackConfirmed = bool.Parse(parsedBody["oauth_callback_confirmed"]),
            };
        }

        static IDictionary<string, string> ParseFormUrlEncoded(string formUrlEncoded)
        {
            return formUrlEncoded.Split('&')
                .Select(x => x.Split('='))
                .Where(x => x.Length == 2)
                .ToDictionary(x => x[0], x => x[1]);
        }

        public Uri GenerateAuthenticationUri(string requestToken)
        {
            return new Uri(
                $"https://www.hatena.ne.jp/oauth/authorize?oauth_token={requestToken}");
        }

        public Task<AccessToken> GetAccessTokenAsync(string authenticationCode)
        {
            return Task.FromResult(new AccessToken
            {
            });
        }

        public void SetAccessToken(string accessToken)
        {
            options.AccessToken = accessToken;
        }
    }
}
