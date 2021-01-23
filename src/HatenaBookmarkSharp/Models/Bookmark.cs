#nullable enable
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HatenaBookmarkSharp
{
    public sealed class Bookmark
    {
        [JsonPropertyName("comment")]
        public string? Comment { get; set; }

        [JsonPropertyName("created_datetime")]
        public DateTime CreatedDateTime { get; set; }

        [JsonPropertyName("created_epoch")]
        public long CreatedEpoch { get; set; }

        [JsonPropertyName("user")]
        public string? User { get; set; }

        [JsonPropertyName("permalink")]
        public Uri? Permalink { get; set; }

        [JsonPropertyName("private")]
        public bool IsPrivate { get; set; }

        [JsonPropertyName("tags")]
        public IReadOnlyList<string> Tags { get; set; } = Array.Empty<string>();
    }
}
