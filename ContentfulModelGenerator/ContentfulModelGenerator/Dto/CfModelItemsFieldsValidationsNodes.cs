using System.Text.Json.Serialization;

namespace ContentfulModelGenerator.Dto;

public class CfModelItemsFieldsValidationsNodes
{
    [JsonPropertyName("embedded-entry-block")]
    public CfModelItemsFieldsValidationsNodesEmbeddedentryblock[]? Embeddedentryblock { get; set; }

    [JsonPropertyName("embedded-entry-inline")]
    public CfModelItemsFieldsValidationsNodesEmbeddedentryinline[]? Embeddedentryinline { get; set; }

    [JsonPropertyName("entry-hyperlink")]
    public CfModelItemsFieldsValidationsNodesEntryhyperlink[]? Entryhyperlink { get; set; }
}