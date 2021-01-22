#nullable enable
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HatenaBookmarkSharp
{
    public class TagsResponse
    {
        [JsonPropertyName("tags")]
        public IReadOnlyList<Tag> Tags { get; set; } = Array.Empty<Tag>();
    }
}
