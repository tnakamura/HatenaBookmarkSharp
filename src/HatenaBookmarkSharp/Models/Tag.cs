#nullable enable
using System.Text.Json.Serialization;

namespace HatenaBookmarkSharp
{
    public class Tag
    {
        [JsonPropertyName("tag")]
        public string? Name { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}
