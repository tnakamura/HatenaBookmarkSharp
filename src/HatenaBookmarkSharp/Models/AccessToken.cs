#nullable enable
namespace HatenaBookmarkSharp.Models
{
    public class AccessToken
    {
        public AccessToken(
            string oAuthToken,
            string oAuthTokenSecret,
            string urlName,
            string displayName)
        {
            OAuthToken = oAuthToken;
            OAuthTokenSecret = oAuthTokenSecret;
            UrlName = urlName;
            DisplayName = displayName;
        }

        public string OAuthToken { get; }

        public string OAuthTokenSecret { get; }

        public string UrlName { get; }

        public string DisplayName { get; }
    }
}
