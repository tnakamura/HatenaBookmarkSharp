#nullable enable
using System;
namespace HatenaBookmarkSharp.Models
{
    public class AccessToken
    {
        public string? OAuthToken { get; set; }

        public string? OAuthTokenSecret { get; set; }

        public string? UrlName { get; set; }

        public string? DisplayName { get; set; }
    }
}
