using System.Text.Json.Serialization;

namespace CodeGenerator.Core.DomainSchema.Schema;

/// <summary>
/// Custom attribute definition
/// </summary>
public class CustomAttribute
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("arguments")]
    public Dictionary<string, object>? Arguments { get; set; }
}
