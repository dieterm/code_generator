using CodeGenerator.Core.Enums;
using CodeGenerator.Core.Interfaces;
using CodeGenerator.Core.Models.Configuration;
using CodeGenerator.Core.Models.Domain;
using CodeGenerator.Core.Models.Output;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.Generators;

/// <summary>
/// Helper functions available in templates
/// </summary>
public class TemplateHelpers
{
    public string PascalCase(string input) => ToPascalCase(input);
    public string CamelCase(string input) => ToCamelCase(input);
    public string SnakeCase(string input) => ToSnakeCase(input);
    public string KebabCase(string input) => ToKebabCase(input);
    public string Pluralize(string input) => SimplePluralize(input);
    public string Singularize(string input) => SimpleSingularize(input);

    private static string ToPascalCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        var words = input.Split(new[] { '_', '-', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        return string.Concat(words.Select(w => char.ToUpperInvariant(w[0]) + w.Substring(1).ToLowerInvariant()));
    }

    private static string ToCamelCase(string input)
    {
        var pascal = ToPascalCase(input);
        if (string.IsNullOrEmpty(pascal)) return pascal;
        return char.ToLowerInvariant(pascal[0]) + pascal.Substring(1);
    }

    private static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        return string.Concat(input.Select((c, i) =>
            i > 0 && char.IsUpper(c) ? "_" + char.ToLowerInvariant(c) : char.ToLowerInvariant(c).ToString()));
    }

    private static string ToKebabCase(string input)
    {
        return ToSnakeCase(input).Replace('_', '-');
    }

    private static string SimplePluralize(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        if (input.EndsWith("y", StringComparison.OrdinalIgnoreCase) && input.Length > 1 &&
            !"aeiou".Contains(char.ToLowerInvariant(input[input.Length - 2])))
            return input.Substring(0, input.Length - 1) + "ies";
        if (input.EndsWith("s", StringComparison.OrdinalIgnoreCase) ||
            input.EndsWith("x", StringComparison.OrdinalIgnoreCase) ||
            input.EndsWith("ch", StringComparison.OrdinalIgnoreCase) ||
            input.EndsWith("sh", StringComparison.OrdinalIgnoreCase))
            return input + "es";
        return input + "s";
    }

    private static string SimpleSingularize(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        if (input.EndsWith("ies", StringComparison.OrdinalIgnoreCase))
            return input.Substring(0, input.Length - 3) + "y";
        if (input.EndsWith("es", StringComparison.OrdinalIgnoreCase))
            return input.Substring(0, input.Length - 2);
        if (input.EndsWith("s", StringComparison.OrdinalIgnoreCase))
            return input.Substring(0, input.Length - 1);
        return input;
    }
}
