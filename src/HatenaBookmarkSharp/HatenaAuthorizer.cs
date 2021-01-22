#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HatenaBookmarkSharp.Models;

namespace HatenaBookmarkSharp
{
    public sealed class HatenaAuthorizer : IHatenaAuthorizer
    {
        readonly OAuth.OAuthAuthorizer authorizer;

        public HatenaAuthorizer(string consumerKey, string consumerSecret)
        {
            authorizer = new OAuth.OAuthAuthorizer(
                consumerKey: consumerKey,
                consumerSecret: consumerSecret);
        }

        public async Task<RequestToken> GetRequestTokenAsync(
            Scopes scope,
            string oauthCallback = "oob",
            CancellationToken cancellationToken = default)
        {
            var response = await authorizer.GetRequestToken(
                requestTokenUrl: "https://www.hatena.com/oauth/initiate",
                parameters: new Dictionary<string, string>
                {
                    ["oauth_callback"] = oauthCallback,
                },
                postValue: new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["scope"] = scope.ToScopesString(),
                }),
                cancellationToken: cancellationToken);

            var callbackConfirmed = response.ExtraData["oauth_callback_confirmed"]
                .Select(x => bool.Parse(x))
                .FirstOrDefault();

            return new RequestToken(
                oauthToken: response.Token.Key,
                oauthTokenSecret: response.Token.Secret,
                oauthCallbackConfirmed: callbackConfirmed);
        }

        public Uri GenerateAuthenticationUri(string requestToken)
        {
            return new Uri(
                $"https://www.hatena.ne.jp/oauth/authorize?oauth_token={requestToken}");
        }

        public async Task<AccessToken> GetAccessTokenAsync(
            string oauthToken,
            string oauthTokenSecret,
            string verifier,
            CancellationToken cancellationToken = default)
        {
            var response = await authorizer.GetAccessToken(
                "https://www.hatena.com/oauth/token",
                requestToken: new OAuth.RequestToken(
                    key: oauthToken,
                    secret: oauthTokenSecret),
                verifier: verifier,
                cancellationToken: cancellationToken);

            var urlName = response.ExtraData["url_name"].FirstOrDefault() ?? "";
            var displayName = response.ExtraData["display_name"].FirstOrDefault() ?? "";
            return new AccessToken(
                oauthToken: response.Token.Key,
                oauthTokenSecret: response.Token.Secret,
                urlName: urlName,
                displayName: displayName);
        }
    }
}
