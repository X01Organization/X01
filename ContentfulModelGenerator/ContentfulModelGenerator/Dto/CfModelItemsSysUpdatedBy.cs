using System.Text.Json.Serialization;

namespace ContentfulModelGenerator.Dto;

public class CfModelItemsSysUpdatedBy
{
    [JsonPropertyName("sys")]
    public CfModelItemsSysUpdatedBySys? Sys { get; set; }
}