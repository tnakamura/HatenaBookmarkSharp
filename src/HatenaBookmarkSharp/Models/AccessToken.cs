#nullable enable
namespace HatenaBookmarkSharp
{
    public sealed class AccessToken
    {
        public AccessToken(
            string oauthToken,
            string oauthTokenSecret,
            string urlName,
            string displayName)
        {
            OAuthToken = oauthToken;
            OAuthTokenSecret = oauthTokenSecret;
            UrlName = urlName;
            DisplayName = displayName;
        }

        public string OAuthToken { get; }

        public string OAuthTokenSecret { get; }

        public string UrlName { get; }

        public string DisplayName { get; }
    }
}
