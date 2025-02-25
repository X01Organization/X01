using System.Text.Json.Serialization;

namespace ContentfulModelGenerator.Dto;

public class CfModelItemsSysPublishedBy
{
    [JsonPropertyName("sys")]
    public CfModelItemsSysPublishedBySys? Sys { get; set; }
}