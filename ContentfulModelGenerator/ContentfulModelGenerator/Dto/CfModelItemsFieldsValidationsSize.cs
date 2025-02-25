using System.Text.Json.Serialization;

namespace ContentfulModelGenerator.Dto;

public class CfModelItemsFieldsValidationsSize
{
    [JsonPropertyName("max")]
    public decimal? Max { get; set; }

    [JsonPropertyName("min")]
    public decimal? Min { get; set; }
}