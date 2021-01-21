#nullable enable

namespace HatenaBookmarkSharp.Models
{
    public sealed class RequestToken
    {
        public RequestToken(
            string oAuthToken,
            string oAuthTokenSecret,
            bool oAuthCallbackConfirmed)
        {
            OAuthToken = oAuthToken;
            OAuthTokenSecret = oAuthTokenSecret;
            OAuthCallbackConfirmed = oAuthCallbackConfirmed;
        }

        public string OAuthToken { get; set; }

        public string OAuthTokenSecret { get; set; }

        public bool OAuthCallbackConfirmed { get; set; }
    }
}
