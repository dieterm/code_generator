using System.Text.Json.Serialization;

namespace CodeGenerator.Core.Models.Schema;

/// <summary>
/// Unique constraint definition
/// </summary>
public class UniqueConstraint
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("columns")]
    public List<string> Columns { get; set; } = new();
}
