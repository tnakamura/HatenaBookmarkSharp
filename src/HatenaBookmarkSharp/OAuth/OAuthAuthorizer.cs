#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace HatenaBookmarkSharp.OAuth
{
    internal class OAuthAuthorizer
    {
        readonly string consumerKey;

        readonly string consumerSecret;

        public OAuthAuthorizer(string consumerKey, string consumerSecret)
        {
            this.consumerKey = consumerKey;
            this.consumerSecret = consumerSecret;
        }

        async Task<TokenResponse<T>> GetTokenResponse<T>(
            string url,
            OAuthMessageHandler handler,
            HttpContent? postValue,
            Func<string, string, T> tokenFactory)
            where T : Token
        {
            var client = new HttpClient(handler);

            var response = await client.PostAsync(url, postValue ?? new FormUrlEncodedContent(Enumerable.Empty<KeyValuePair<string, string>>())).ConfigureAwait(false);
            var tokenBase = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new HttpRequestException(response.StatusCode + ":" + tokenBase); // error message
            }

            var splitted = tokenBase.Split('&').Select(s => s.Split('=')).ToLookup(xs => xs[0], xs => xs[1]);
            var token = tokenFactory(splitted["oauth_token"].First().UrlDecode(), splitted["oauth_token_secret"].First().UrlDecode());
            var extraData = splitted.Where(kvp => kvp.Key != "oauth_token" && kvp.Key != "oauth_token_secret")
                .SelectMany(g => g, (g, value) => new { g.Key, Value = value })
                .ToLookup(kvp => kvp.Key, kvp => kvp.Value);
            return new TokenResponse<T>(token, extraData);
        }

        public string BuildAuthorizeUrl(string authUrl, RequestToken requestToken)
        {
            if (authUrl == null) throw new ArgumentNullException(nameof(authUrl));
            if (requestToken == null) throw new ArgumentNullException(nameof(requestToken));

            return authUrl + "?oauth_token=" + requestToken.Key;
        }

        public Task<TokenResponse<RequestToken>> GetRequestToken(
            string requestTokenUrl,
            IEnumerable<KeyValuePair<string, string>>? parameters = null,
            HttpContent? postValue = null)
        {
            if (requestTokenUrl == null) throw new ArgumentNullException(nameof(requestTokenUrl));

            var handler = new OAuthMessageHandler(consumerKey, consumerSecret, token: null, optionalOAuthHeaderParameters: parameters);
            return GetTokenResponse(
                requestTokenUrl,
                handler,
                postValue,
                (key, secret) => new RequestToken(key, secret));
        }

        public Task<TokenResponse<AccessToken>> GetAccessToken(
            string accessTokenUrl,
            RequestToken requestToken,
            string verifier,
            IEnumerable<KeyValuePair<string, string>>? parameters = null,
            HttpContent? postValue = null)
        {
            if (accessTokenUrl == null) throw new ArgumentNullException(nameof(accessTokenUrl));
            if (requestToken == null) throw new ArgumentNullException(nameof(requestToken));
            if (verifier == null) throw new ArgumentNullException(nameof(verifier));

            var verifierParam = new KeyValuePair<string, string>("oauth_verifier", verifier.Trim());

            if (parameters == null) parameters = Enumerable.Empty<KeyValuePair<string, string>>();
            var handler = new OAuthMessageHandler(consumerKey, consumerSecret, token: requestToken, optionalOAuthHeaderParameters: parameters.Concat(new[] { verifierParam }));

            return GetTokenResponse(
                accessTokenUrl,
                handler,
                postValue,
                (key, secret) => new AccessToken(key, secret));
        }
    }
}
