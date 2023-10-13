using System.Text.Json.Serialization;

namespace ContentfulModelGenerator.Dto;

public class CfModelItemsMetadataAnnotationsContentTypeField
{
    [JsonPropertyName("days")]
    public CfModelItemsMetadataAnnotationsContentTypeFieldDays[]? Days { get; set; }
}