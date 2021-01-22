#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using HatenaBookmarkSharp.Models;

namespace HatenaBookmarkSharp
{
    public interface IHatenaAuthorizer
    {
        Task<RequestToken> GetRequestTokenAsync(
            Scopes scope,
            string oauthCallback = "oob",
            CancellationToken cancellationToken = default);

        Uri GenerateAuthenticationUri(string requestToken);

        Task<AccessToken> GetAccessTokenAsync(
            string oauthToken,
            string oauthTokenSecret,
            string verifier,
            CancellationToken cancellationToken = default);
    }
}