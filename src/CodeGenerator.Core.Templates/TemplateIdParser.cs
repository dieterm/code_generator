namespace CodeGenerator.Core.Templates;

/// <summary>
/// Parses and resolves TemplateId strings with special folder syntax.
/// TemplateId format: @SpecialFolder/subfolder/.../TemplateName
/// Special folders are marked with @ prefix.
/// </summary>
public class TemplateIdParser
{
    /// <summary>
    /// Special folder prefix character
    /// </summary>
    public const char SpecialFolderPrefix = '@';

    /// <summary>
    /// Path separator used in TemplateIds
    /// </summary>
    public const char PathSeparator = '/';

    /// <summary>
    /// Characters not allowed in template names
    /// </summary>
    public static readonly char[] InvalidTemplateNameChars = { '@', '/', '\\' };

    /// <summary>
    /// Well-known special folder names (without @ prefix)
    /// </summary>
    public static class SpecialFolders
    {
        public const string Workspace = "Workspace";
        public const string Generators = "Generators";
        public const string TemplateEngines = "TemplateEngines";
        public const string UserDefined = "UserDefined";
    }

    /// <summary>
    /// Parse a TemplateId into its components
    /// </summary>
    /// <param name="templateId">The TemplateId to parse</param>
    /// <returns>Parsed result</returns>
    public static ParsedTemplateId Parse(string templateId)
    {
        if (string.IsNullOrWhiteSpace(templateId))
        {
            return new ParsedTemplateId
            {
                FullTemplateId = templateId ?? string.Empty,
                IsValid = false,
                ErrorMessage = "TemplateId cannot be null or empty"
            };
        }

        var segments = templateId.Split(PathSeparator, StringSplitOptions.RemoveEmptyEntries);
        
        if (segments.Length == 0)
        {
            return new ParsedTemplateId
            {
                FullTemplateId = templateId,
                IsValid = false,
                ErrorMessage = "TemplateId has no segments"
            };
        }

        var pathSegments = new List<string>();
        var specialFolderSegments = new List<string>();
        string? rootSpecialFolder = null;

        // All segments except the last are path segments
        for (int i = 0; i < segments.Length - 1; i++)
        {
            var segment = segments[i];
            pathSegments.Add(segment);

            if (segment.StartsWith(SpecialFolderPrefix))
            {
                var folderName = segment.TrimStart(SpecialFolderPrefix);
                specialFolderSegments.Add(folderName);
                
                if (i == 0)
                {
                    rootSpecialFolder = folderName;
                }
            }
        }

        // Last segment is the template name
        var templateName = segments[^1];

        // Validate template name
        if (templateName.IndexOfAny(InvalidTemplateNameChars) >= 0)
        {
            return new ParsedTemplateId
            {
                FullTemplateId = templateId,
                IsValid = false,
                ErrorMessage = $"Template name '{templateName}' contains invalid characters (@, /, \\)"
            };
        }

        // Build template path (everything except template name)
        var templatePath = pathSegments.Count > 0
            ? string.Join(PathSeparator, pathSegments) + PathSeparator
            : string.Empty;

        return new ParsedTemplateId
        {
            FullTemplateId = templateId,
            RootSpecialFolder = rootSpecialFolder,
            TemplatePath = templatePath,
            TemplateName = templateName,
            PathSegments = pathSegments,
            SpecialFolderSegments = specialFolderSegments,
            IsValid = true
        };
    }

    /// <summary>
    /// Check if a TemplateId uses special folder syntax
    /// </summary>
    public static bool HasSpecialFolderSyntax(string templateId)
    {
        return !string.IsNullOrEmpty(templateId) && templateId.Contains(SpecialFolderPrefix);
    }

    /// <summary>
    /// Build a TemplateId from components
    /// </summary>
    /// <param name="rootSpecialFolder">Root special folder (e.g., "Workspace")</param>
    /// <param name="subFolders">Optional subfolders</param>
    /// <param name="templateName">Template name</param>
    /// <returns>Full TemplateId</returns>
    public static string BuildTemplateId(string rootSpecialFolder, IEnumerable<string>? subFolders, string templateName)
    {
        var parts = new List<string> { SpecialFolderPrefix + rootSpecialFolder };

        if (subFolders != null)
        {
            foreach (var folder in subFolders)
            {
                // Add @ prefix if it's a special folder reference
                if (folder.StartsWith(SpecialFolderPrefix.ToString()))
                {
                    parts.Add(folder);
                }
                else
                {
                    parts.Add(folder);
                }
            }
        }

        parts.Add(templateName);

        return string.Join(PathSeparator, parts);
    }

    /// <summary>
    /// Build a TemplateId for a workspace artifact template
    /// </summary>
    public static string BuildWorkspaceTemplateId(string artifactName, string templateName, params string[] subFolders)
    {
        var folders = new List<string> { SpecialFolderPrefix + artifactName };
        folders.AddRange(subFolders);
        return BuildTemplateId(SpecialFolders.Workspace, folders, templateName);
    }

    /// <summary>
    /// Build a TemplateId for a generator template
    /// </summary>
    public static string BuildGeneratorTemplateId(string generatorId, string templateName, params string[] subFolders)
    {
        var folders = new List<string> { generatorId };
        folders.AddRange(subFolders);
        return BuildTemplateId(SpecialFolders.Generators, folders, templateName);
    }

    /// <summary>
    /// Build a TemplateId for a template engine template
    /// </summary>
    public static string BuildTemplateEngineTemplateId(string templateEngineId, string templateName, params string[] subFolders)
    {
        var folders = new List<string> { templateEngineId };
        folders.AddRange(subFolders);
        return BuildTemplateId(SpecialFolders.TemplateEngines, folders, templateName);
    }

    /// <summary>
    /// Build a TemplateId for a user-defined template
    /// </summary>
    public static string BuildUserDefinedTemplateId(string templateName, params string[] subFolders)
    {
        return BuildTemplateId(SpecialFolders.UserDefined, subFolders, templateName);
    }

    /// <summary>
    /// Extract special folder names from a TemplateId (used for auto-registration)
    /// </summary>
    public static IEnumerable<string> ExtractSpecialFolders(string templateId)
    {
        var parsed = Parse(templateId);
        return parsed.SpecialFolderSegments;
    }

    /// <summary>
    /// Validate a template name
    /// </summary>
    public static bool IsValidTemplateName(string templateName)
    {
        if (string.IsNullOrWhiteSpace(templateName))
            return false;

        return templateName.IndexOfAny(InvalidTemplateNameChars) < 0;
    }
}
