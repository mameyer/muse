using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Player.Interfaces.DTO
{
    public interface IContextDTO 
    {
        [JsonPropertyName("externalUrls")]
        object ExternalUrls { get; set; }
        //Dictionary<string, string> ExternalUrls { get; set; }

        [JsonPropertyName("href")]
        string Href { get; set; }

        [JsonPropertyName("type")]
        string Type { get; set; }

        [JsonPropertyName("uri")]
        string Uri { get; set; }
    }
}