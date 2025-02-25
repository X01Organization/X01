using System.Text.Json.Serialization;

namespace ContentfulModelGenerator.Dto;

public class CfModelItems
{
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("displayField")]
    public string? DisplayField { get; set; }

    [JsonPropertyName("fields")]
    public CfModelItemsFields[]? Fields { get; set; }

    [JsonPropertyName("metadata")]
    public CfModelItemsMetadata? Metadata { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("sys")]
    public CfModelItemsSys? Sys { get; set; }
}