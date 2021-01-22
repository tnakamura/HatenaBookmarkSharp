#nullable enable
using System;
using System.Text.Json.Serialization;

namespace HatenaBookmarkSharp
{
    public sealed class PostRequest
    {
        [JsonPropertyName("url")]
        public Uri? Uri { get; set; }

        [JsonPropertyName("comment")]
        public string? Comment { get; set; }

        [JsonPropertyName("post_twitter")]
        public bool IsPostTwitter { get; set; }

        [JsonPropertyName("post_facebook")]
        public bool IsPostFacebook { get; set; }

        [JsonPropertyName("post_mixi")]
        public bool IsPostMixi { get; set; }

        [JsonPropertyName("post_evernote")]
        public bool IsPostEvernote { get; set; }

        [JsonPropertyName("send_mail")]
        public bool IsSendMail { get; set; }

        [JsonPropertyName("private")]
        public bool IsPrivate { get; set; }
    }
}
