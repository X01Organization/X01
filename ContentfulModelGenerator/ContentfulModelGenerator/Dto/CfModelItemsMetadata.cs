using System.Text.Json.Serialization;

namespace ContentfulModelGenerator.Dto;

public class CfModelItemsMetadata
{
    [JsonPropertyName("annotations")]
    public CfModelItemsMetadataAnnotations? Annotations { get; set; }
}