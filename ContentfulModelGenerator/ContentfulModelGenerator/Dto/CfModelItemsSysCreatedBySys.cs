using System.Text.Json.Serialization;

namespace ContentfulModelGenerator.Dto;

public class CfModelItemsSysCreatedBySys
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("linkType")]
    public string? LinkType { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }
}