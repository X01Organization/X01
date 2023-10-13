using System.Text.Json.Serialization;

namespace ContentfulModelGenerator.Dto;

public class CfModelItemsMetadataAnnotationsContentType
{
    [JsonPropertyName("sys")]
    public CfModelItemsMetadataAnnotationsContentTypeSys? Sys { get; set; }
}