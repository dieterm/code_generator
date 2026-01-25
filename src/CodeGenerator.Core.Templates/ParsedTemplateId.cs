namespace CodeGenerator.Core.Templates;


/// <summary>
/// Result of parsing a TemplateId
/// </summary>
public class ParsedTemplateId
{
    /// <summary>
    /// The original full TemplateId
    /// </summary>
    public string FullTemplateId { get; init; } = string.Empty;

    /// <summary>
    /// The root special folder (e.g., "Workspace", "Generators")
    /// </summary>
    public string? RootSpecialFolder { get; init; }

    /// <summary>
    /// The path portion including special folders (e.g., "@Workspace/@TableArtifact/scripts/mysql/")
    /// </summary>
    public string TemplatePath { get; init; } = string.Empty;

    /// <summary>
    /// The template name (last segment, e.g., "create_table")
    /// </summary>
    public string TemplateName { get; init; } = string.Empty;

    /// <summary>
    /// All path segments (excluding template name)
    /// </summary>
    public List<string> PathSegments { get; init; } = new();

    /// <summary>
    /// Special folder segments (segments that start with @)
    /// </summary>
    public List<string> SpecialFolderSegments { get; init; } = new();

    /// <summary>
    /// Whether this TemplateId uses special folder syntax
    /// </summary>
    public bool HasSpecialFolders => SpecialFolderSegments.Count > 0;

    /// <summary>
    /// Whether this is a valid TemplateId
    /// </summary>
    public bool IsValid { get; init; }

    /// <summary>
    /// Error message if parsing failed
    /// </summary>
    public string? ErrorMessage { get; init; }
}

