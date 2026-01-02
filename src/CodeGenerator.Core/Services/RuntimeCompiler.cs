using System.Reflection;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.Services;

/// <summary>
/// Service for compiling C# code at runtime
/// </summary>
public class RuntimeCompiler
{
    private readonly ILogger<RuntimeCompiler> _logger;
    private static int _assemblyCounter = 0;

    public RuntimeCompiler(ILogger<RuntimeCompiler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Compile C# source code to an assembly and create an instance of the specified type
    /// </summary>
    public async Task<CompilationResult> CompileAndCreateInstanceAsync(
        string sourceCode,
        string typeName,
        string[]? additionalSources = null,
        CancellationToken cancellationToken = default)
    {
        var result = new CompilationResult();

        try
        {
            // Parse the source code
            var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode, cancellationToken: cancellationToken);
            var syntaxTrees = new List<SyntaxTree> { syntaxTree };

            // Add additional source files if provided
            if (additionalSources != null)
            {
                foreach (var additionalSource in additionalSources)
                {
                    syntaxTrees.Add(CSharpSyntaxTree.ParseText(additionalSource, cancellationToken: cancellationToken));
                }
            }

            // Generate unique assembly name
            var assemblyName = $"DynamicAssembly_{Interlocked.Increment(ref _assemblyCounter)}_{Guid.NewGuid():N}";

            // Get references
            var references = GetRequiredReferences();

            _logger.LogInformation("Compiling with {ReferenceCount} assembly references", references.Count);

            // Create compilation
            var compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: syntaxTrees,
                references: references,
                options: new CSharpCompilationOptions(
                    OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Debug,
                    allowUnsafe: false));

            // Compile to memory
            using var ms = new MemoryStream();
            EmitResult emitResult = compilation.Emit(ms, cancellationToken: cancellationToken);

            if (!emitResult.Success)
            {
                // Compilation failed
                result.Success = false;
                result.Errors = emitResult.Diagnostics
                    .Where(d => d.Severity == DiagnosticSeverity.Error)
                    .Select(d => $"{d.Id}: {d.GetMessage()} at {d.Location.GetLineSpan()}")
                    .ToList();

                result.Warnings = emitResult.Diagnostics
                    .Where(d => d.Severity == DiagnosticSeverity.Warning)
                    .Select(d => $"{d.Id}: {d.GetMessage()}")
                    .ToList();

                _logger.LogError("Compilation failed with {ErrorCount} errors", result.Errors.Count);
                return result;
            }

            // Load the assembly
            ms.Seek(0, SeekOrigin.Begin);
            var assembly = AssemblyLoadContext.Default.LoadFromStream(ms);

            // Find and instantiate the type
            var type = assembly.GetType(typeName);
            if (type == null)
            {
                result.Success = false;
                result.Errors.Add($"Type '{typeName}' not found in compiled assembly");
                _logger.LogError("Type {TypeName} not found in compiled assembly", typeName);
                return result;
            }

            // Create instance
            var instance = Activator.CreateInstance(type);
            if (instance == null)
            {
                result.Success = false;
                result.Errors.Add($"Failed to create instance of type '{typeName}'");
                _logger.LogError("Failed to create instance of {TypeName}", typeName);
                return result;
            }

            result.Success = true;
            result.CompiledAssembly = assembly;
            result.CompiledType = type;
            result.Instance = instance;

            _logger.LogInformation("Successfully compiled and instantiated {TypeName}", typeName);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Errors.Add($"Compilation error: {ex.Message}");
            _logger.LogError(ex, "Error during compilation");
        }

        return result;
    }

    /// <summary>
    /// Get all required assembly references for compilation
    /// </summary>
    private List<MetadataReference> GetRequiredReferences()
    {
        var references = new List<MetadataReference>();
        var addedAssemblies = new HashSet<string>();

        // Core .NET references
        var coreAssemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
        
        AddReference(references, addedAssemblies, typeof(object).Assembly.Location); // System.Private.CoreLib
        AddReference(references, addedAssemblies, typeof(Console).Assembly.Location); // System.Console
        AddReference(references, addedAssemblies, typeof(IEnumerable<>).Assembly.Location); // System.Collections
        AddReference(references, addedAssemblies, typeof(System.Linq.Enumerable).Assembly.Location); // System.Linq
        
        // Try to add common system assemblies
        TryAddAssembly(references, addedAssemblies, coreAssemblyPath, "System.Runtime.dll");
        TryAddAssembly(references, addedAssemblies, coreAssemblyPath, "System.Private.CoreLib.dll");
        TryAddAssembly(references, addedAssemblies, coreAssemblyPath, "System.ComponentModel.Primitives.dll");
        TryAddAssembly(references, addedAssemblies, coreAssemblyPath, "System.ComponentModel.TypeConverter.dll");
        TryAddAssembly(references, addedAssemblies, coreAssemblyPath, "System.ObjectModel.dll");
        TryAddAssembly(references, addedAssemblies, coreAssemblyPath, "netstandard.dll");

        // Add all currently loaded assemblies that might be relevant
        AddLoadedAssemblies(references, addedAssemblies);

        // Try to load WinForms references
        TryAddWinFormsReferences(references, addedAssemblies, coreAssemblyPath);

        _logger.LogInformation("Added {Count} unique assembly references", references.Count);

        return references;
    }

    private void AddReference(List<MetadataReference> references, HashSet<string> addedAssemblies, string assemblyPath)
    {
        if (!string.IsNullOrEmpty(assemblyPath) && File.Exists(assemblyPath) && addedAssemblies.Add(assemblyPath))
        {
            references.Add(MetadataReference.CreateFromFile(assemblyPath));
            _logger.LogDebug("Added reference: {Path}", Path.GetFileName(assemblyPath));
        }
    }

    private void TryAddAssembly(List<MetadataReference> references, HashSet<string> addedAssemblies, string directory, string fileName)
    {
        try
        {
            var path = Path.Combine(directory, fileName);
            if (File.Exists(path))
            {
                AddReference(references, addedAssemblies, path);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to add assembly {FileName}", fileName);
        }
    }

    private void AddLoadedAssemblies(List<MetadataReference> references, HashSet<string> addedAssemblies)
    {
        try
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    // Skip dynamic assemblies
                    if (assembly.IsDynamic)
                        continue;

                    // Skip assemblies without location
                    if (string.IsNullOrEmpty(assembly.Location))
                        continue;

                    var assemblyName = assembly.GetName().Name;
                    
                    // Add System.* and Microsoft.* assemblies, plus WinForms
                    if (assemblyName?.StartsWith("System") == true ||
                        assemblyName?.StartsWith("Microsoft") == true ||
                        assemblyName?.Contains("Windows") == true ||
                        assemblyName?.Contains("Drawing") == true)
                    {
                        AddReference(references, addedAssemblies, assembly.Location);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Could not add loaded assembly");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to enumerate loaded assemblies");
        }
    }

    private void TryAddWinFormsReferences(List<MetadataReference> references, HashSet<string> addedAssemblies, string coreAssemblyPath)
    {
        try
        {
            // WinForms assemblies
            var winFormsAssemblies = new[]
            {
                "System.Windows.Forms.dll",
                "System.Windows.Forms.Primitives.dll",
                "System.Drawing.dll",
                "System.Drawing.Common.dll",
                "System.Drawing.Primitives.dll",
                "Accessibility.dll"
            };

            foreach (var assemblyName in winFormsAssemblies)
            {
                TryAddAssembly(references, addedAssemblies, coreAssemblyPath, assemblyName);
            }

            _logger.LogInformation("WinForms references processing completed");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to add WinForms references");
        }
    }
}

/// <summary>
/// Result of runtime compilation
/// </summary>
public class CompilationResult
{
    public bool Success { get; set; }
    public Assembly? CompiledAssembly { get; set; }
    public Type? CompiledType { get; set; }
    public object? Instance { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}
