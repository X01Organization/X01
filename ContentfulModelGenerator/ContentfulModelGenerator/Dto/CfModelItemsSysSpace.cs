using System.Text.Json.Serialization;

namespace ContentfulModelGenerator.Dto;

public class CfModelItemsSysSpace
{
    [JsonPropertyName("sys")]
    public CfModelItemsSysSpaceSys? Sys { get; set; }
}