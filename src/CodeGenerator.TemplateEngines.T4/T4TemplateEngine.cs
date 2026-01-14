using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Templates;
using Microsoft.Extensions.Logging;
using Mono.TextTemplating;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Text;

namespace CodeGenerator.TemplateEngines.T4
{
    /// <summary>
    /// T4 Template Engine implementation using Mono.TextTemplating
    /// </summary>
    public class T4TemplateEngine : TemplateEngine<T4Template, T4TemplateInstance>
    {
        private readonly ConcurrentDictionary<string, (CompiledTemplate Template, TemplateGenerator Generator)> _compiledTemplates = new();
        private readonly string? _templateRootFolder;

        public T4TemplateEngine(ILogger<T4TemplateEngine> logger, string? templateRootFolder = null)
            : base(logger, "t4_template_engine", "T4 Template Engine", TemplateType.T4, new[] { "tt" })
        {
            _templateRootFolder = templateRootFolder;
        }

        /// <summary>
        /// Render a T4 template instance
        /// </summary>
        public override async Task<TemplateOutput> RenderAsync(T4TemplateInstance templateInstance, CancellationToken cancellationToken)
        {
            var filePath = templateInstance.Template is T4FileTemplate fileTemplate 
                ? fileTemplate.FilePath 
                : "in-memory template";

            try
            {
                Logger.LogDebug("Rendering T4 template: {TemplateId}", templateInstance.Template.TemplateId);

                // Create the template generator/host
                var generator = CreateTemplateGenerator(templateInstance);

                // Add parameters BEFORE compilation - these will be resolved during template execution
                foreach (var param in templateInstance.Parameters)
                {
                    generator.AddParameter(null, null, param.Key, param.Value?.ToString() ?? string.Empty);
                }

                // Determine the input file path for include resolution
                string inputFile;
                string templateContent;
                
                if (templateInstance.Template is T4FileTemplate ft)
                {
                    inputFile = ft.FilePath;
                    // Read the template from file - this ensures the input path is correct for include resolution
                    templateContent = await File.ReadAllTextAsync(inputFile, cancellationToken);
                }
                else
                {
                    var templateDirectory = GetTemplateDirectory(templateInstance);
                    inputFile = Path.Combine(templateDirectory ?? Environment.CurrentDirectory, $"{templateInstance.Template.TemplateId}.tt");
                    templateContent = ((T4Template)templateInstance.Template).Content;
                }

                // Use ProcessTemplateAsync which handles includes correctly
                // It returns a tuple (string fileName, string content, bool success)
                var result = await generator.ProcessTemplateAsync(inputFile, templateContent, null, cancellationToken);
                
                if (!result.success || generator.Errors.HasErrors)
                {
                    var errors = generator.Errors
                        .Cast<CompilerError>()
                        .Where(e => !e.IsWarning)
                        .Select(e => $"Line {e.Line}, Column {e.Column}: {e.ErrorText}")
                        .ToList();

                    Logger.LogError("T4 template processing failed with {ErrorCount} errors", errors.Count);
                    foreach (var error in errors)
                    {
                        Logger.LogError("T4 Error: {Error}", error);
                    }

                    return new TemplateOutput(errors.Any() ? errors : new[] { "Template processing failed without specific error" });
                }

                var output = result.content;

                // Log warnings
                var warnings = generator.Errors
                    .Cast<CompilerError>()
                    .Where(e => e.IsWarning)
                    .Select(e => $"Line {e.Line}, Column {e.Column}: {e.ErrorText}");

                foreach (var warning in warnings)
                {
                    Logger.LogWarning("T4 Warning: {Warning}", warning);
                }

                if (!string.IsNullOrEmpty(output))
                {
                    var outputFileName = templateInstance.OutputFileName ?? 
                        result.fileName ?? 
                        Path.ChangeExtension(Path.GetFileName(filePath), ".txt");
                    
                    var fileArtifact = new FileArtifact(outputFileName);
                    fileArtifact.SetTextContent(output);
                    
                    Logger.LogDebug("T4 template rendered successfully: {OutputFileName}", outputFileName);
                    return new TemplateOutput(fileArtifact);
                }
                else
                {
                    Logger.LogWarning("T4 template rendered to empty string without errors");
                    return new TemplateOutput("Template rendered to empty string without errors.");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error rendering T4 template: {TemplateId}", templateInstance.Template.TemplateId);
                return new TemplateOutput($"Error rendering template: {ex.Message}");
            }
        }

        /// <summary>
        /// Render a T4 template from a file path with parameters
        /// </summary>
        public async Task<string> RenderFileAsync(string templatePath, Dictionary<string, object>? parameters = null, CancellationToken cancellationToken = default)
        {
            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"Template file not found: {templatePath}");
            }

            var templateContent = await File.ReadAllTextAsync(templatePath, cancellationToken);
            var generator = new TemplateGenerator();

            // Add parameters BEFORE compilation
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    generator.AddParameter(null, null, param.Key, param.Value?.ToString() ?? string.Empty);
                }
            }

            // Add include path for the template directory
            var templateDir = Path.GetDirectoryName(templatePath);
            if (!string.IsNullOrEmpty(templateDir))
            {
                generator.IncludePaths.Add(templateDir);
            }

            if (!string.IsNullOrEmpty(_templateRootFolder))
            {
                generator.IncludePaths.Add(_templateRootFolder);
            }

            // Compile the template
            var compiled = await generator.CompileTemplateAsync(templateContent, cancellationToken);

            if (compiled == null || generator.Errors.HasErrors)
            {
                var errorBuilder = new StringBuilder("Template compilation errors:\n");
                foreach (CompilerError error in generator.Errors)
                {
                    if (!error.IsWarning)
                    {
                        errorBuilder.AppendLine($"  Line {error.Line}: {error.ErrorText}");
                    }
                }
                throw new InvalidOperationException(errorBuilder.ToString());
            }

            var output = compiled.Process();
            return output ?? string.Empty;
        }

        /// <summary>
        /// Render a T4 template from string content
        /// </summary>
        public async Task<string> RenderAsync(string templateContent, Dictionary<string, object>? parameters = null, CancellationToken cancellationToken = default)
        {
            var generator = new TemplateGenerator();

            // Add parameters BEFORE compilation
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    generator.AddParameter(null, null, param.Key, param.Value?.ToString() ?? string.Empty);
                }
            }

            if (!string.IsNullOrEmpty(_templateRootFolder))
            {
                generator.IncludePaths.Add(_templateRootFolder);
            }

            // Compile the template
            var compiled = await generator.CompileTemplateAsync(templateContent, cancellationToken);

            if (compiled == null || generator.Errors.HasErrors)
            {
                var errorBuilder = new StringBuilder("Template compilation errors:\n");
                foreach (CompilerError error in generator.Errors)
                {
                    if (!error.IsWarning)
                    {
                        errorBuilder.AppendLine($"  Line {error.Line}: {error.ErrorText}");
                    }
                }
                throw new InvalidOperationException(errorBuilder.ToString());
            }

            var output = compiled.Process();
            return output ?? string.Empty;
        }

        /// <summary>
        /// Compile a T4 template for later execution with parameters
        /// </summary>
        public async Task<bool> CompileTemplateAsync(string templateId, string templateContent, Dictionary<string, object>? parameters = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var generator = new TemplateGenerator();

                if (!string.IsNullOrEmpty(_templateRootFolder))
                {
                    generator.IncludePaths.Add(_templateRootFolder);
                }

                // Add parameters before compilation
                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        generator.AddParameter(null, null, param.Key, param.Value?.ToString() ?? string.Empty);
                    }
                }

                var compiled = await generator.CompileTemplateAsync(templateContent, cancellationToken);

                if (compiled != null && !generator.Errors.HasErrors)
                {
                    // Store both the compiled template and the generator (for parameter resolution)
                    _compiledTemplates[templateId] = (compiled, generator);
                    Logger.LogDebug("Compiled T4 template: {TemplateId}", templateId);
                    return true;
                }

                Logger.LogError("Failed to compile T4 template: {TemplateId}", templateId);
                return false;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error compiling T4 template: {TemplateId}", templateId);
                return false;
            }
        }

        /// <summary>
        /// Render a pre-compiled template
        /// </summary>
        public string RenderCompiled(string templateId)
        {
            if (!_compiledTemplates.TryGetValue(templateId, out var entry))
            {
                throw new KeyNotFoundException($"Compiled template '{templateId}' not found");
            }

            var output = entry.Template.Process();
            return output ?? string.Empty;
        }

        private TemplateGenerator CreateTemplateGenerator(T4TemplateInstance templateInstance)
        {
            var generator = new TemplateGenerator();

            // Add include directories
            if (!string.IsNullOrEmpty(_templateRootFolder) && Directory.Exists(_templateRootFolder))
            {
                generator.IncludePaths.Add(_templateRootFolder);
            }

            foreach (var dir in templateInstance.IncludeDirectories)
            {
                if (Directory.Exists(dir))
                {
                    generator.IncludePaths.Add(dir);
                }
            }

            // Add template directory
            var templateDir = GetTemplateDirectory(templateInstance);
            if (!string.IsNullOrEmpty(templateDir) && Directory.Exists(templateDir))
            {
                generator.IncludePaths.Add(templateDir);
            }

            // Add assembly references
            foreach (var assemblyRef in templateInstance.AssemblyReferences)
            {
                generator.ReferencePaths.Add(assemblyRef);
            }

            return generator;
        }

        private string? GetTemplateDirectory(T4TemplateInstance templateInstance)
        {
            if (templateInstance.Template is T4FileTemplate fileTemplate)
            {
                return Path.GetDirectoryName(fileTemplate.FilePath);
            }

            return _templateRootFolder;
        }

        public override ITemplate CreateTemplateFromFile(string filePath)
        {
            // Todo: detect .tt.def file and get id+parameters from there
            return new T4FileTemplate(filePath, filePath);
        }

        public override ITemplateInstance CreateTemplateInstance(ITemplate template)
        {
            throw new NotImplementedException();
        }
    }
}
