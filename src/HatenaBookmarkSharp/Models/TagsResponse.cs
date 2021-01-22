#nullable enable
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HatenaBookmarkSharp.Models
{
    public class TagsResponse
    {
        [JsonPropertyName("tags")]
        public IReadOnlyList<Tag> Tags { get; set; } = Array.Empty<Tag>();
    }
}
