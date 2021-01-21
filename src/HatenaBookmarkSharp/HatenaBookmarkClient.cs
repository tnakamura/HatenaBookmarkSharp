#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
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

        static HatenaBookmarkClient()
        {
            OAuth.OAuthUtility.ComputeHash = (key, buffer) =>
            {
                return new HMACSHA1(key).ComputeHash(buffer);
            };
        }

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
            var response = await httpClient.PostAsync(
                $"/rest/1/my/bookmark",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["url"] = parameter.Uri.ToString(),
                    ["comment"] = parameter.Comment.ToString(),
                    ["post_twitter"] = parameter.IsPostTwitter.ToString(),
                    ["post_facebook"] = parameter.IsPostFacebook.ToString(),
                    ["post_mixi"] = parameter.IsPostMixi.ToString(),
                    ["post_evernote"] = parameter.IsPostEvernote.ToString(),
                    ["send_mail"] = parameter.IsSendMail.ToString(),
                    ["private"] = parameter.IsPrivate.ToString(),
                }),
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
            var authorizer = CreateAuthorizer();

            var response = await authorizer.GetRequestToken(
                requestTokenUrl: "https://www.hatena.com/oauth/initiate",
                parameters: new Dictionary<string, string>
                {
                    ["oauth_callback"] = "oob",
                },
                postValue: new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["scope"] = options.Scope!,
                }));

            var callbackConfirmed = response.ExtraData["oauth_callback_confirmed"]
                .Select(x => bool.Parse(x))
                .FirstOrDefault();

            return new RequestToken(
                oAuthToken: response.Token.Key,
                oAuthTokenSecret: response.Token.Secret,
                oAuthCallbackConfirmed: callbackConfirmed);
        }

        OAuth.OAuthAuthorizer CreateAuthorizer()
        {
            if (options.OAuthConsumerKey == null ||
                options.OAuthConsumerSecret == null)
            {
                throw new InvalidOperationException();
            }
            return new OAuth.OAuthAuthorizer(
                consumerKey: options.OAuthConsumerKey,
                consumerSecret: options.OAuthConsumerSecret);
        }

        public Uri GenerateAuthenticationUri(string requestToken)
        {
            return new Uri(
                $"https://www.hatena.ne.jp/oauth/authorize?oauth_token={requestToken}");
        }

        public async Task<AccessToken> GetAccessTokenAsync(
            string authenticationCode,
            RequestToken requestToken,
            CancellationToken cancellationToken = default)
        {
            var authorizer = CreateAuthorizer();

            var response = await authorizer.GetAccessToken(
                "https://www.hatena.com/oauth/token",
                requestToken: new OAuth.RequestToken(
                    key: requestToken.OAuthToken,
                    secret: requestToken.OAuthTokenSecret),
                verifier: authenticationCode);

            var urlName = response.ExtraData["url_name"].FirstOrDefault() ?? "";
            var displayName = response.ExtraData["display_name"].FirstOrDefault() ?? "";
            return new AccessToken(
                oAuthToken: response.Token.Key,
                oAuthTokenSecret: response.Token.Secret,
                urlName: urlName,
                displayName: displayName);
        }

        public void SetAccessToken(string accessToken)
        {
            options.AccessToken = accessToken;
        }
    }
}
