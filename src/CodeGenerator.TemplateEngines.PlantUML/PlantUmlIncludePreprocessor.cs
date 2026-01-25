using CodeGenerator.Core.Templates;
using System.Text.RegularExpressions;

namespace CodeGenerator.TemplateEngines.PlantUML;

/// <summary>
/// Preprocessor for PlantUML diagrams that resolves TemplateId syntax in !include directives
/// </summary>
public static class PlantUmlIncludePreprocessor
{
    /// <summary>
    /// Regex pattern to match !include and !include_once directives with TemplateId syntax
    /// Matches: !include @... or !include_once @...
    /// </summary>
    private static readonly Regex IncludePattern = new Regex(
        @"^(\s*!include(?:_once)?\s+)(@[^\s]+)(\s*)$",
        RegexOptions.Multiline | RegexOptions.Compiled);

    /// <summary>
    /// Process PlantUML diagram definition and resolve any TemplateId references in !include directives
    /// </summary>
    /// <param name="diagramDefinition">The PlantUML diagram definition</param>
    /// <param name="pathResolver">The template path resolver</param>
    /// <returns>Processed diagram definition with resolved include paths</returns>
    public static string ProcessIncludes(string diagramDefinition, TemplatePathResolver? pathResolver)
    {
        if (string.IsNullOrEmpty(diagramDefinition) || pathResolver == null)
            return diagramDefinition;

        return IncludePattern.Replace(diagramDefinition, match =>
        {
            var prefix = match.Groups[1].Value; // "!include " or "!include_once "
            var templateId = match.Groups[2].Value; // "@..."
            var suffix = match.Groups[3].Value; // trailing whitespace

            // Check if this is a TemplateId (starts with @)
            if (TemplateIdParser.HasSpecialFolderSyntax(templateId))
            {
                var resolvedPath = pathResolver.ResolveTemplateId(templateId);
                if (!string.IsNullOrEmpty(resolvedPath))
                {
                    // Replace with the resolved file path
                    return $"{prefix}{resolvedPath}{suffix}";
                }
                else
                {
                    // Keep original if resolution failed (PlantUML will report the error)
                    return match.Value;
                }
            }

            return match.Value;
        });
    }

    /// <summary>
    /// Check if a diagram definition contains TemplateId includes
    /// </summary>
    public static bool HasTemplateIdIncludes(string diagramDefinition)
    {
        if (string.IsNullOrEmpty(diagramDefinition))
            return false;

        return IncludePattern.IsMatch(diagramDefinition) && 
               diagramDefinition.Contains("!include") && 
               diagramDefinition.Contains("@");
    }

    /// <summary>
    /// Extract all TemplateId references from !include directives
    /// </summary>
    public static IEnumerable<string> ExtractTemplateIdIncludes(string diagramDefinition)
    {
        if (string.IsNullOrEmpty(diagramDefinition))
            yield break;

        foreach (Match match in IncludePattern.Matches(diagramDefinition))
        {
            var templateId = match.Groups[2].Value;
            if (TemplateIdParser.HasSpecialFolderSyntax(templateId))
            {
                yield return templateId;
            }
        }
    }
}
