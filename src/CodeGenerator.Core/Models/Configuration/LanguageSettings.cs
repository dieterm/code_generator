using CodeGenerator.Core.Enums;

namespace CodeGenerator.Core.Models.Configuration;

/// <summary>
/// Language-specific settings
/// </summary>
public class LanguageSettings
{
    /// <summary>
    /// Language identifier
    /// </summary>
    public TargetLanguage Language { get; set; }

    /// <summary>
    /// File extension
    /// </summary>
    public string FileExtension { get; set; } = string.Empty;

    /// <summary>
    /// Type mappings from schema types to language types
    /// </summary>
    public Dictionary<string, string> TypeMappings { get; set; } = new();

    /// <summary>
    /// Naming convention settings
    /// </summary>
    public NamingConventions NamingConventions { get; set; } = new();

    /// <summary>
    /// Code formatting settings
    /// </summary>
    public FormattingSettings FormattingSettings { get; set; } = new();
}
