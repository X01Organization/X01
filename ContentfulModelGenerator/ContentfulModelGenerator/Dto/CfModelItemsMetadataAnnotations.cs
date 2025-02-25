using System.Text.Json.Serialization;

namespace ContentfulModelGenerator.Dto;

public class CfModelItemsMetadataAnnotations
{
    [JsonPropertyName("ContentType")]
    public CfModelItemsMetadataAnnotationsContentType[]? ContentType { get; set; }

    [JsonPropertyName("ContentTypeField")]
    public CfModelItemsMetadataAnnotationsContentTypeField? ContentTypeField { get; set; }
}