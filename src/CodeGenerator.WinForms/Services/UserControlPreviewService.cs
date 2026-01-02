using CodeGenerator.Core.Models.Output;
using CodeGenerator.Core.Services;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace CodeGenerator.WinForms.Services;

/// <summary>
/// Service for creating visual previews of generated WinForms UserControls
/// </summary>
public class UserControlPreviewService
{
    private readonly RuntimeCompiler _compiler;
    private readonly ILogger<UserControlPreviewService> _logger;

    public UserControlPreviewService(RuntimeCompiler compiler, ILogger<UserControlPreviewService> logger)
    {
        _compiler = compiler;
        _logger = logger;
    }

    /// <summary>
    /// Compile a FilePreview and create a UserControl instance
    /// </summary>
    public async Task<UserControlPreviewResult> CreateUserControlPreviewAsync(
        FilePreview mainFile,
        GenerationPreview generationPreview,
        CancellationToken cancellationToken = default)
    {
        var result = new UserControlPreviewResult();

        try
        {
            if (!mainFile.IsPreviewable)
            {
                result.Success = false;
                result.ErrorMessage = "File is not previewable (must be a WinForms view file)";
                return result;
            }

            // Extract the full typename from the main file
            var typeName = ExtractTypeName(mainFile.Content, mainFile.FileName);
            if (string.IsNullOrEmpty(typeName))
            {
                result.Success = false;
                result.ErrorMessage = "Could not extract type name from source code";
                return result;
            }

            // Find the related Designer.cs file
            var designerFileName = mainFile.FileName.Replace(".cs", ".Designer.cs");
            var designerFile = generationPreview.FilesToCreate
                .Concat(generationPreview.FilesToModify)
                .FirstOrDefault(f => f.FileName == designerFileName && 
                                   f.EntityName == mainFile.EntityName);

            List<string> additionalSources = new();
            if (designerFile != null)
            {
                additionalSources.Add(designerFile.Content);
                _logger.LogInformation("Found designer file: {DesignerFile}", designerFileName);
            }
            else
            {
                _logger.LogWarning("Designer file not found: {DesignerFile}", designerFileName);
            }
            
            // Compile and create instance
            var compilationResult = await _compiler.CompileAndCreateInstanceAsync(
                mainFile.Content,
                typeName,
                additionalSources.ToArray(),
                cancellationToken);

            if (!compilationResult.Success)
            {
                result.Success = false;
                result.ErrorMessage = $"Compilation failed:\n{string.Join("\n", compilationResult.Errors)}";
                result.CompilationErrors = compilationResult.Errors;
                result.CompilationWarnings = compilationResult.Warnings;
                return result;
            }

            // Verify it's a UserControl
            if (compilationResult.Instance is not UserControl userControl)
            {
                result.Success = false;
                result.ErrorMessage = $"Compiled type is not a UserControl: {compilationResult.CompiledType?.FullName}";
                return result;
            }

            result.Success = true;
            result.UserControl = userControl;
            result.TypeName = typeName;
            result.CompilationWarnings = compilationResult.Warnings;

            _logger.LogInformation("Successfully created UserControl preview for {TypeName}", typeName);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = $"Unexpected error: {ex.Message}";
            _logger.LogError(ex, "Error creating UserControl preview");
        }

        return result;
    }

    /// <summary>
    /// Extract the full type name (including namespace) from C# source code
    /// </summary>
    private string? ExtractTypeName(string sourceCode, string fileName)
    {
        try
        {
            // Extract namespace
            var namespaceMatch = Regex.Match(sourceCode, @"namespace\s+([\w\.]+)");
            string? namespaceName = namespaceMatch.Success ? namespaceMatch.Groups[1].Value : null;

            // Extract class name (look for partial class that inherits from UserControl or Form)
            var classMatch = Regex.Match(sourceCode, 
                @"public\s+partial\s+class\s+(\w+)\s*:\s*(?:UserControl|Form)",
                RegexOptions.Multiline);

            if (!classMatch.Success)
            {
                // Try without partial
                classMatch = Regex.Match(sourceCode,
                    @"public\s+class\s+(\w+)\s*:\s*(?:UserControl|Form)",
                    RegexOptions.Multiline);
            }

            if (!classMatch.Success)
            {
                _logger.LogWarning("Could not find class definition in {FileName}", fileName);
                return null;
            }

            var className = classMatch.Groups[1].Value;

            // Combine namespace and class name
            if (!string.IsNullOrEmpty(namespaceName))
            {
                return $"{namespaceName}.{className}";
            }

            return className;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting type name from {FileName}", fileName);
            return null;
        }
    }
}

/// <summary>
/// Result of UserControl preview creation
/// </summary>
public class UserControlPreviewResult
{
    public bool Success { get; set; }
    public UserControl? UserControl { get; set; }
    public string? TypeName { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string> CompilationErrors { get; set; } = new();
    public List<string> CompilationWarnings { get; set; } = new();
}
