using System.Text.Json.Serialization;

namespace ContentfulModelGenerator.Dto;

public class CfModelItemsFieldsValidations
{
    [JsonPropertyName("enabledMarks")]
    public string[]? EnabledMarks { get; set; }

    [JsonPropertyName("enabledNodeTypes")]
    public string[]? EnabledNodeTypes { get; set; }

    //[JsonPropertyName("in")]
    //public string[]? In { get; set; }

    //[JsonPropertyName("in")]
    //public decimal[]? In { get; set; }

    [JsonPropertyName("linkContentType")]
    public string[]? LinkContentType { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("nodes")]
    public CfModelItemsFieldsValidationsNodes? Nodes { get; set; }

    [JsonPropertyName("size")]
    public CfModelItemsFieldsValidationsSize? Size { get; set; }

    [JsonPropertyName("unique")]
    public bool? Unique { get; set; }
}