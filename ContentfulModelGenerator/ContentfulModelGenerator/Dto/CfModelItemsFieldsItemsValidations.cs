using System.Text.Json.Serialization;

namespace ContentfulModelGenerator.Dto;

public class CfModelItemsFieldsItemsValidations
{
    [JsonPropertyName("in")]
    public string[]? In { get; set; }

    [JsonPropertyName("linkContentType")]
    public string[]? LinkContentType { get; set; }
}