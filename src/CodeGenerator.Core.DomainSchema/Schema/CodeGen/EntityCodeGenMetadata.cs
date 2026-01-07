using System.Text.Json.Serialization;

namespace CodeGenerator.Core.DomainSchema.Schema;

/// <summary>
/// Entity-level code generation metadata
/// </summary>
public class EntityCodeGenMetadata
{
    [JsonPropertyName("className")]
    public string? ClassName { get; set; }

    [JsonPropertyName("namespace")]
    public string? Namespace { get; set; }

    [JsonPropertyName("baseClass")]
    public string? BaseClass { get; set; }

    [JsonPropertyName("interfaces")]
    public List<string>? Interfaces { get; set; }

    [JsonPropertyName("isAbstract")]
    public bool IsAbstract { get; set; }

    [JsonPropertyName("isSealed")]
    public bool IsSealed { get; set; }

    [JsonPropertyName("generateRepository")]
    public bool GenerateRepository { get; set; } = true;

    [JsonPropertyName("generateController")]
    public bool GenerateController { get; set; } = true;

    [JsonPropertyName("generateViewModel")]
    public bool GenerateViewModel { get; set; } = true;

    [JsonPropertyName("generateView")]
    public bool GenerateView { get; set; } = true;

    [JsonPropertyName("isOwnedType")]
    public bool IsOwnedType { get; set; }

    [JsonPropertyName("customAttributes")]
    public List<CustomAttribute>? CustomAttributes { get; set; }
}
