using System.Text.Json.Serialization;

namespace ContentfulModelGenerator.Dto;

public class CfModelSys
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }
}