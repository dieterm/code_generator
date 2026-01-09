using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Templates;
using CodeGenerator.TemplateEngines.Scriban;
using Microsoft.Extensions.Logging;
using Scriban;
using Scriban.Runtime;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace CodeGenerator.TemplateEngines.Scriban;

/// <summary>
/// Scriban-based template engine implementation
/// </summary>
public class ScribanTemplateEngine : TemplateEngine<ScribanTemplate, ScribanTemplateInstance>
{
    private readonly ScriptObject _globalFunctions = new ScriptObject();
    private readonly ConcurrentDictionary<string, Template> _compiledTemplates = new ConcurrentDictionary<string, Template>();

    public string TemplateRootFolder { get; }

    public ScribanTemplateEngine(ILogger<ScribanTemplateEngine> logger, string templateRootFolder)
        : base(logger, "scriban_template_engine", "Scriban Template Engine", TemplateType.Scriban, new[] { "scriban" })
    {
        TemplateRootFolder = templateRootFolder;
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
            Logger.LogError(ex, "Error rendering template");
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
            Logger.LogError(ex, "Error rendering template");
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
            Logger.LogError(ex, "Error rendering template file {Path}", templatePath);
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
        Logger.LogDebug("Compiled template {TemplateId}", templateId);
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
        Logger.LogDebug("Registered function {FunctionName}", name);
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

    public override async Task<TemplateOutput> RenderAsync(ScribanTemplateInstance templateInstance, CancellationToken cancellationToken)
    {
        try
        {
            // First compile the template if not already compiled
            if(!string.IsNullOrEmpty(templateInstance.Template.TemplateId) && !_compiledTemplates.ContainsKey(templateInstance.Template.TemplateId))
            {
                CompileTemplate(templateInstance.Template.TemplateId, ((ScribanTemplate)templateInstance.Template).Content);
            }
            // Then render the compiled template
            if (!string.IsNullOrEmpty(templateInstance.Template.TemplateId) && _compiledTemplates.TryGetValue(templateInstance.Template.TemplateId, out var compiledTemplate))
            {
                var context = new TemplateContext();
                var templateLocations = new List<string>() { TemplateRootFolder };
                if (templateInstance.Template is ScribanFileTemplate fileTemplate)
                {
                    templateLocations.Add(System.IO.Path.GetDirectoryName(fileTemplate.FilePath));
                }
                templateLocations.AddRange(templateInstance.ExtraTemplateLocations);
                var finalTemplateLocations = templateLocations.Where(l => !string.IsNullOrWhiteSpace(l)).Distinct().ToList();
                context.TemplateLoader = new DefaultTemplateLoader(finalTemplateLocations, Logger);
                context.PushGlobal(_globalFunctions);
                var extraParams = new ScriptObject();
                foreach (var param in templateInstance.Parameters)
                {
                    extraParams.Add(param.Key, param.Value);
                }
                foreach (var func in templateInstance.Functions)
                {
                    extraParams.Import(func.Key, func.Value );
                }
                context.PushGlobal(extraParams);
                var result = compiledTemplate.Render(context);
                if(result!= null)
                {
                    var fileArtifact = new FileArtifact(templateInstance.OutputFileName ?? "output.txt");
                    fileArtifact.SetTextContent(result);
                    return new TemplateOutput(fileArtifact);
                } 
                else
                {
                    if(compiledTemplate.HasErrors)
                    {
                        var errors = compiledTemplate.Messages.Select(m => m.ToString());
                        Logger.LogError("Template rendering errors: {Errors}", string.Join(Environment.NewLine, errors));
                        return new TemplateOutput(errors);
                    }
                    else
                    {
                        Logger.LogWarning("Template rendered to null without errors.");
                        return new TemplateOutput("Template rendered to null without errors.");
                    }
                }
            }
            else
            {
                var errorMsg = $"Template '{templateInstance.Template.TemplateId}' not found in compiled templates.";
                Logger.LogError(errorMsg);
                return new TemplateOutput(errorMsg);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An error occurred while rendering the template.");
            return new TemplateOutput(ex.Message);
        }
    }
}