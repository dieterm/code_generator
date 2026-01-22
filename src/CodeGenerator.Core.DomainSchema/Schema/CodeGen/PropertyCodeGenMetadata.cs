using System.Text.Json.Serialization;

namespace CodeGenerator.Core.DomainSchema.Schema;

/// <summary>
/// Property-level code generation metadata
/// </summary>
public class PropertyCodeGenMetadata
{
    [JsonPropertyName("propertyName")]
    public string? PropertyName { get; set; }

    [JsonPropertyName("clrType")]
    public string? ClrType { get; set; }

    [JsonPropertyName("isNullable")]
    public bool IsNullable { get; set; }

    [JsonPropertyName("isReadOnly")]
    public bool IsReadOnly { get; set; }

    [JsonPropertyName("isComputed")]
    public bool IsComputed { get; set; }

    [JsonPropertyName("defaultValue")]
    public string? DefaultValue { get; set; }

    [JsonPropertyName("visibility")]
    public string Visibility { get; set; } = "public";

    [JsonPropertyName("customAttributes")]
    public List<CustomAttribute>? CustomAttributes { get; set; }

    [JsonPropertyName("validationRules")]
    public List<ValidationRule>? ValidationRules { get; set; }

    [JsonPropertyName("displaySettings")]
    public DisplaySettings? DisplaySettings { get; set; }
}
