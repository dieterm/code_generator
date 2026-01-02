using System.Collections.Concurrent;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Scriban;
using Scriban.Runtime;

namespace CodeGenerator.Templates;

/// <summary>
/// Scriban-based template engine implementation
/// </summary>
public class ScribanTemplateEngine : ITemplateEngine
{
    private readonly ILogger<ScribanTemplateEngine> _logger;
    private readonly ConcurrentDictionary<string, Template> _compiledTemplates;
    private readonly ScriptObject _globalFunctions;
    
    public ScribanTemplateEngine(ILogger<ScribanTemplateEngine> logger)
    {
        _logger = logger;
        _compiledTemplates = new ConcurrentDictionary<string, Template>();
        _globalFunctions = new ScriptObject();
        RegisterBuiltInFunctions();
    }

    public async Task<string> RenderAsync(string templateContent, object model, CancellationToken cancellationToken = default)
    {
        try
        {
            var template = Template.Parse(templateContent);
            if (template.HasErrors)
            {
                var errors = string.Join(Environment.NewLine, template.Messages.Select(m => m.Message));
                throw new InvalidOperationException($"Template parsing errors: {errors}");
            }

            var context = CreateContext(model);
            var result = await template.RenderAsync(context);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rendering template");
            throw;
        }
    }

    private async Task<string> RenderAsync(string templateContent, string templatePath, object model, CancellationToken cancellationToken = default)
    {
        try
        {
            var template = Template.Parse(templateContent, templatePath);
            if (template.HasErrors)
            {
                var errors = string.Join(Environment.NewLine, template.Messages.Select(m => m.Message));
                throw new InvalidOperationException($"Template parsing errors: {errors}");
            }

            var context = CreateContext(model);
            var result = await template.RenderAsync(context);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rendering template");
            throw;
        }
    }

    public async Task<string> RenderFileAsync(string templatePath, object model, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"Template file not found: {templatePath}");
            }

            var templateContent = await File.ReadAllTextAsync(templatePath, cancellationToken);
            return await RenderAsync(templateContent, templatePath, model, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rendering template file {Path}", templatePath);
            throw;
        }
    }

    public void CompileTemplate(string templateId, string templateContent)
    {
        var template = Template.Parse(templateContent);
        if (template.HasErrors)
        {
            var errors = string.Join(Environment.NewLine, template.Messages.Select(m => m.Message));
            throw new InvalidOperationException($"Template compilation errors for '{templateId}': {errors}");
        }

        _compiledTemplates[templateId] = template;
        _logger.LogDebug("Compiled template {TemplateId}", templateId);
    }

    public async Task<string> RenderCompiledAsync(string templateId, object model, CancellationToken cancellationToken = default)
    {
        if (!_compiledTemplates.TryGetValue(templateId, out var template))
        {
            throw new KeyNotFoundException($"Template '{templateId}' not found in cache");
        }

        var context = CreateContext(model);
        var result = await template.RenderAsync(context);
        return result;
    }

    public void RegisterFunction(string name, Delegate function)
    {
        _globalFunctions.Import(name, function);
        _logger.LogDebug("Registered function {FunctionName}", name);
    }

    private TemplateContext CreateContext(object model)
    {
        var context = new TemplateContext
        {
            MemberRenamer = member => member.Name,
            LoopLimit = 10000,
            RecursiveLimit = 100
        };

        var scriptObject = new ScriptObject();
        scriptObject.Import(model, renamer: member => member.Name);

        // Add global functions
        foreach (var member in _globalFunctions)
        {
            scriptObject[member.Key] = member.Value;
        }

        context.PushGlobal(scriptObject);

        return context;
    }

    private void RegisterBuiltInFunctions()
    {
        // String manipulation
        _globalFunctions.Import("pascal_case", _templateHelper.PascalCase);
        _globalFunctions.Import("camel_case", _templateHelper.CamelCase);
        _globalFunctions.Import("snake_case", _templateHelper.SnakeCase);
        _globalFunctions.Import("kebab_case", _templateHelper.KebabCase);
        _globalFunctions.Import("pluralize", _templateHelper.Pluralize);
        _globalFunctions.Import("singularize", _templateHelper.Singularize);
        //_globalFunctions.Import("lower", new Func<string, string>(s => s?.ToLowerInvariant() ?? string.Empty));
        //_globalFunctions.Import("upper", new Func<string, string>(s => s?.ToUpperInvariant() ?? string.Empty));
        
        // Type helpers
        _globalFunctions.Import("nullable_type", new Func<string, bool, string>((type, nullable) =>
            nullable && !type.EndsWith("?") ? $"{type}?" : type));

        _globalFunctions.Import("is_collection", new Func<string, bool>(type =>
            type?.StartsWith("List<") == true || type?.StartsWith("IList<") == true ||
            type?.StartsWith("IEnumerable<") == true || type?.StartsWith("ICollection<") == true));

        _globalFunctions.Import("get_collection_type", new Func<string, string>(type =>
        {
            if (string.IsNullOrEmpty(type)) return "object";
            var start = type.IndexOf('<');
            var end = type.LastIndexOf('>');
            if (start >= 0 && end > start)
                return type.Substring(start + 1, end - start - 1);
            return type;
        }));

        // C# specific
        _globalFunctions.Import("csharp_type", new Func<string, string>(MapToCSharpType));
        _globalFunctions.Import("sql_type", new Func<string, int?, string>(MapToSqlType));

        // Date/Time
        _globalFunctions.Import("now", new Func<string>(() => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
        _globalFunctions.Import("utc_now", new Func<string>(() => DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")));

        // Conditional helpers
        _globalFunctions.Import("if_empty", new Func<string, string, string>((value, defaultValue) =>
            string.IsNullOrEmpty(value) ? defaultValue : value));
    }
    private static TemplateHelpers _templateHelper = new TemplateHelpers();
    //private static string ToPascalCase(string input)
    //{
    //    _templateHelper.PascalCase(input);
    //    if (string.IsNullOrEmpty(input)) return input;
    //    var words = input.Split(new[] { '_', '-', ' ' }, StringSplitOptions.RemoveEmptyEntries);
    //    return string.Concat(words.Select(w => char.ToUpperInvariant(w[0]) + w.Substring(1)));
    //}

    //private static string ToCamelCase(string input)
    //{
    //    var pascal = ToPascalCase(input);
    //    if (string.IsNullOrEmpty(pascal)) return pascal;
    //    return char.ToLowerInvariant(pascal[0]) + pascal.Substring(1);
    //}

    //private static string ToSnakeCase(string input)
    //{
    //    if (string.IsNullOrEmpty(input)) return input;
    //    return string.Concat(input.Select((c, i) =>
    //        i > 0 && char.IsUpper(c) ? "_" + char.ToLowerInvariant(c) : char.ToLowerInvariant(c).ToString()));
    //}

    //private static string ToKebabCase(string input)
    //{
    //    return ToSnakeCase(input).Replace('_', '-');
    //}

    //private static string Pluralize(string input)
    //{
    //    if (string.IsNullOrEmpty(input)) return input;
    //    if (input.EndsWith("y", StringComparison.OrdinalIgnoreCase) && input.Length > 1 &&
    //        !"aeiou".Contains(char.ToLowerInvariant(input[input.Length - 2])))
    //        return input.Substring(0, input.Length - 1) + "ies";
    //    if (input.EndsWith("s", StringComparison.OrdinalIgnoreCase) ||
    //        input.EndsWith("x", StringComparison.OrdinalIgnoreCase) ||
    //        input.EndsWith("ch", StringComparison.OrdinalIgnoreCase) ||
    //        input.EndsWith("sh", StringComparison.OrdinalIgnoreCase))
    //        return input + "es";
    //    return input + "s";
    //}

    //private static string Singularize(string input)
    //{
    //    if (string.IsNullOrEmpty(input)) return input;
    //    if (input.EndsWith("ies", StringComparison.OrdinalIgnoreCase))
    //        return input.Substring(0, input.Length - 3) + "y";
    //    if (input.EndsWith("es", StringComparison.OrdinalIgnoreCase))
    //        return input.Substring(0, input.Length - 2);
    //    if (input.EndsWith("s", StringComparison.OrdinalIgnoreCase))
    //        return input.Substring(0, input.Length - 1);
    //    return input;
    //}

    private static string MapToCSharpType(string schemaType)
    {
        return schemaType?.ToLowerInvariant() switch
        {
            "string" => "string",
            "integer" => "int",
            "int32" => "int",
            "int64" => "long",
            "number" => "decimal",
            "decimal" => "decimal",
            "float" => "float",
            "double" => "double",
            "boolean" => "bool",
            "bool" => "bool",
            "datetime" => "DateTime",
            "date" => "DateOnly",
            "time" => "TimeOnly",
            "guid" => "Guid",
            "uuid" => "Guid",
            "binary" => "byte[]",
            _ => schemaType ?? "object"
        };
    }

    private static string MapToSqlType(string clrType, int? maxLength)
    {
        var length = maxLength?.ToString() ?? "MAX";
        return clrType?.ToLowerInvariant() switch
        {
            "string" => $"NVARCHAR({length})",
            "int" => "INT",
            "integer" => "INT",
            "long" => "BIGINT",
            "decimal" => "DECIMAL(18,2)",
            "float" => "FLOAT",
            "double" => "FLOAT",
            "bool" => "BIT",
            "boolean" => "BIT",
            "datetime" => "DATETIME2",
            "dateonly" => "DATE",
            "timeonly" => "TIME",
            "guid" => "UNIQUEIDENTIFIER",
            "byte[]" => "VARBINARY(MAX)",
            _ => "NVARCHAR(MAX)"
        };
    }
}
