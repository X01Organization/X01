using System.Text.Json.Serialization;

namespace ContentfulModelGenerator.Dto;

public class CfModelItemsSysCreatedBy
{
    [JsonPropertyName("sys")]
    public CfModelItemsSysCreatedBySys? Sys { get; set; }
}