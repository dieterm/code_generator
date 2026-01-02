using CodeGenerator.Core.Enums;

namespace CodeGenerator.Generators;

/// <summary>
/// Information about an available generator
/// </summary>
public class GeneratorInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public GeneratorType Type { get; set; }
    public ArchitectureLayer Layer { get; set; }
    public List<TargetLanguage> SupportedLanguages { get; set; } = new();
}
