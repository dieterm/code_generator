using System.Text.Json.Serialization;

namespace CodeGenerator.Core.Models.Schema;

/// <summary>
/// Index column definition
/// </summary>
public class IndexColumn
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("descending")]
    public bool Descending { get; set; }
}
