using CodeGenerator.Core.Enums;

namespace CodeGenerator.Core.Models.Configuration;

/// <summary>
/// Application-wide settings for the code generator
/// </summary>
public class GeneratorSettings
{
    /// <summary>
    /// Root folder of the solution to generate
    /// </summary>
    public string SolutionRootFolder { get; set; } = string.Empty;

    /// <summary>
    /// Root namespace for generated code
    /// </summary>
    public string RootNamespace { get; set; } = string.Empty;

    /// <summary>
    /// Folder containing Scriban templates
    /// </summary>
    public string TemplateFolder { get; set; } = string.Empty;

    /// <summary>
    /// Output folder for generated files
    /// </summary>
    public string OutputFolder { get; set; } = string.Empty;

    /// <summary>
    /// Path to the JSON schema file
    /// </summary>
    public string SchemaFilePath { get; set; } = string.Empty;

    /// <summary>
    /// Target framework for generated projects
    /// </summary>
    public string TargetFramework { get; set; } = "net8.0";

    /// <summary>
    /// Whether to overwrite existing files
    /// </summary>
    public bool OverwriteExisting { get; set; } = false;

    /// <summary>
    /// Whether to create backup of existing files
    /// </summary>
    public bool CreateBackup { get; set; } = true;

    /// <summary>
    /// Generator-specific configurations
    /// </summary>
    public Dictionary<string, GeneratorConfiguration> Generators { get; set; } = new();

    /// <summary>
    /// Language-specific settings
    /// </summary>
    public Dictionary<string, LanguageSettings> LanguageSettings { get; set; } = new();

    /// <summary>
    /// NuGet package references to include
    /// </summary>
    public List<NuGetPackageReference> NuGetPackages { get; set; } = new();
}
