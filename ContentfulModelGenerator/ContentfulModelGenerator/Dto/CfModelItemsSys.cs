using System.Text.Json.Serialization;

namespace ContentfulModelGenerator.Dto;

public class CfModelItemsSys
{
    [JsonPropertyName("createdAt")]
    public string? CreatedAt { get; set; }

    [JsonPropertyName("createdBy")]
    public CfModelItemsSysCreatedBy? CreatedBy { get; set; }

    [JsonPropertyName("environment")]
    public CfModelItemsSysEnvironment? Environment { get; set; }

    [JsonPropertyName("firstPublishedAt")]
    public string? FirstPublishedAt { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("publishedAt")]
    public string? PublishedAt { get; set; }

    [JsonPropertyName("publishedBy")]
    public CfModelItemsSysPublishedBy? PublishedBy { get; set; }

    [JsonPropertyName("publishedCounter")]
    public decimal? PublishedCounter { get; set; }

    [JsonPropertyName("publishedVersion")]
    public decimal? PublishedVersion { get; set; }

    [JsonPropertyName("space")]
    public CfModelItemsSysSpace? Space { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("updatedAt")]
    public string? UpdatedAt { get; set; }

    [JsonPropertyName("updatedBy")]
    public CfModelItemsSysUpdatedBy? UpdatedBy { get; set; }

    [JsonPropertyName("version")]
    public decimal? Version { get; set; }
}