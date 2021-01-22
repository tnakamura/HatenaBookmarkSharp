#nullable enable

namespace HatenaBookmarkSharp
{
    public sealed class RequestToken
    {
        public RequestToken(
            string oauthToken,
            string oauthTokenSecret,
            bool oauthCallbackConfirmed)
        {
            OAuthToken = oauthToken;
            OAuthTokenSecret = oauthTokenSecret;
            OAuthCallbackConfirmed = oauthCallbackConfirmed;
        }

        public string OAuthToken { get; set; }

        public string OAuthTokenSecret { get; set; }

        public bool OAuthCallbackConfirmed { get; set; }
    }
}
