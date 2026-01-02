using CodeGenerator.Core.Enums;

namespace CodeGenerator.Core.Models.Configuration;


/// <summary>
/// Configuration for a specific generator
/// </summary>
public class GeneratorConfiguration
{
    /// <summary>
    /// Generator identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Display name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Generator description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Whether this generator is enabled
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Type of generator
    /// </summary>
    public GeneratorType Type { get; set; }

    /// <summary>
    /// Target architecture layer
    /// </summary>
    public ArchitectureLayer Layer { get; set; }

    /// <summary>
    /// Target programming language
    /// </summary>
    public TargetLanguage Language { get; set; } = TargetLanguage.CSharp;

    /// <summary>
    /// List of template files to use
    /// </summary>
    public List<TemplateReference> Templates { get; set; } = new();

    /// <summary>
    /// Output path pattern (supports placeholders)
    /// </summary>
    public string OutputPathPattern { get; set; } = string.Empty;

    /// <summary>
    /// File extension for generated files
    /// </summary>
    public string FileExtension { get; set; } = ".cs";

    /// <summary>
    /// Additional parameters for this generator
    /// </summary>
    public Dictionary<string, object> Parameters { get; set; } = new();

    /// <summary>
    /// Dependencies on other generators (must run first)
    /// </summary>
    public List<string> DependsOn { get; set; } = new();
}
