using System.Text.Json.Serialization;

namespace ContentfulModelGenerator.Dto;

public class CfModelItemsSysEnvironment
{
    [JsonPropertyName("sys")]
    public CfModelItemsSysEnvironmentSys? Sys { get; set; }
}