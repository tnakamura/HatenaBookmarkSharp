using System.Text.Json.Serialization;

namespace HatenaBookmarkSharp.Models
{
    public class User
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("plususer")]
        public bool IsPlusUser { get; set; }

        [JsonPropertyName("private")]
        public bool IsPrivate { get; set; }

        [JsonPropertyName("is_oauth_twitter")]
        public bool IsOAuthTwitter { get; set; }

        [JsonPropertyName("is_oauth_evernote")]
        public bool IsOAuthEvernote { get; set; }

        [JsonPropertyName("is_oauth_facebook")]
        public bool IsOAuthFacebook { get; set; }

        [JsonPropertyName("is_oauth_mixi_check")]
        public bool IsOAuthMixiCheck { get; set; }
    }
}
