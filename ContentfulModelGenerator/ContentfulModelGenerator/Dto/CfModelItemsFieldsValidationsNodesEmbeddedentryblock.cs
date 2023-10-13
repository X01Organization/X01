using System.Text.Json.Serialization;

namespace ContentfulModelGenerator.Dto;

public class CfModelItemsFieldsValidationsNodesEmbeddedentryblock
{
    [JsonPropertyName("linkContentType")]
    public string[]? LinkContentType { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }
}