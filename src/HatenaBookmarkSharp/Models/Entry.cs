#nullable enable
using System;
using System.Text.Json.Serialization;

namespace HatenaBookmarkSharp
{
    public class Entry
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("url")]
        public Uri? Url { get; set; }

        [JsonPropertyName("entry_url")]
        public Uri? EntryUrl { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("favicon_url")]
        public Uri? FaviconUrl { get; set; }

        [JsonPropertyName("smartphone_app_entry_url")]
        public Uri? SmartPhoneAppEntryUrl { get; set; }
    }
}
