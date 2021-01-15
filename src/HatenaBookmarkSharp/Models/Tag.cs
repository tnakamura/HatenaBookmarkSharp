using System;
using System.Text.Json.Serialization;

namespace HatenaBookmarkSharp.Models
{
    public class Tag
    {
        [JsonPropertyName("tag")]
        public string Name { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("modified_datetime")]
        public DateTime ModifiedDateTime { get; set; }

        [JsonPropertyName("modified_epoch")]
        public int ModifiedEpoch { get; set; }
    }
}
