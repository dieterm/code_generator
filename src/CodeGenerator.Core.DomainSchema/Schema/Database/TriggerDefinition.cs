using System.Text.Json.Serialization;

namespace CodeGenerator.Core.DomainSchema.Schema;

/// <summary>
/// Trigger definition
/// </summary>
public class TriggerDefinition
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("timing")]
    public string Timing { get; set; } = "AFTER";

    [JsonPropertyName("events")]
    public List<string> Events { get; set; } = new();

    [JsonPropertyName("body")]
    public string Body { get; set; } = string.Empty;
}
