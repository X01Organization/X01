using System.Text.Json.Serialization;

namespace ContentfulModelGenerator.Dto;

public class CfModelItemsFieldsItems
{
    [JsonPropertyName("linkType")]
    public string? LinkType { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("validations")]
    public CfModelItemsFieldsItemsValidations[]? Validations { get; set; }
}