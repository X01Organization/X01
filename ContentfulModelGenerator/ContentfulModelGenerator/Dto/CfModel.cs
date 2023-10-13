using System.Text.Json.Serialization;

namespace ContentfulModelGenerator.Dto;

public class CfModel
{
    [JsonPropertyName("items")]
    public CfModelItems[]? Items { get; set; }

    [JsonPropertyName("limit")]
    public decimal? Limit { get; set; }

    [JsonPropertyName("skip")]
    public decimal? Skip { get; set; }

    [JsonPropertyName("sys")]
    public CfModelSys? Sys { get; set; }

    [JsonPropertyName("total")]
    public decimal? Total { get; set; }
}