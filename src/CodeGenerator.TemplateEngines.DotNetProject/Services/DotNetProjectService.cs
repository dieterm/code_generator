using CodeGenerator.Domain.DotNet;
using CodeGenerator.TemplateEngines.DotNetProject.Models;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace CodeGenerator.TemplateEngines.DotNetProject.Services;

/// <summary>
/// Generates .NET projects using dotnet CLI and MSBuild
/// </summary>
public class DotNetProjectService
{
    private readonly ILogger<DotNetProjectService> _logger;

    public DotNetProjectService(ILogger<DotNetProjectService> logger)
    {
        _logger = logger;
    }

    public async Task<GeneratedProject> CreateProjectAsync(
     string name,
     string directory,
     string projectType,
     string targetFramework,
     string language = "C#", // Nieuw: language parameter
     CancellationToken cancellationToken = default)
    {
        var languageRecord = DotNetLanguages.AllLanguages
            .FirstOrDefault(l => l.DotNetCommandLineArgument.Equals(language, StringComparison.OrdinalIgnoreCase));
        if (languageRecord == null) throw new ArgumentException($"Unsupported language: {language}");

        var fileExtension = languageRecord.ProjectFileExtension;

        var project = new GeneratedProject
        {
            Name = name,
            Directory = directory,
            ProjectFilePath = Path.Combine(directory, $"{name}.{fileExtension}")
        };

        // Check if project already exists
        if (File.Exists(project.ProjectFilePath))
        {
            _logger.LogInformation("Project {Name} already exists at {Path}", name, project.ProjectFilePath);
            return project;
        }

        // Ensure directory exists
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Create project using dotnet CLI with language parameter
        var languageParam = language.ToUpper() != DotNetLanguages.CSharp.DotNetCommandLineArgument ? $" -lang {language}" : "";
        var command = $"new {projectType} -n \"{name}\" -f {targetFramework}{languageParam} -o \"{directory}\"";
        var result = await RunDotNetCommandAsync(command, cancellationToken);

        if (!result.Success)
        {
            throw new InvalidOperationException($"Failed to create project: {result.Error}");
        }

        // scan folder for created files
        var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            project.Files.Add(new GeneratedFile
            {
                AbsolutePath = file,
                FileName = Path.GetFileName(file),
                RelativePath = Path.GetRelativePath(directory, file),
                IsNew = true,
                Written = true
            });
        }

        _logger.LogInformation("Created {Language} project {Name} at {Path}", language, name, project.ProjectFilePath);
        return project;
    }

    public async Task AddPackageAsync(
        string projectPath,
        string packageId,
        string version,
        CancellationToken cancellationToken = default)
    {
        var command = $"add \"{projectPath}\" package {packageId} --version {version}";
        var result = await RunDotNetCommandAsync(command, cancellationToken);

        if (!result.Success)
        {
            _logger.LogWarning("Failed to add package {PackageId}: {Error}", packageId, result.Error);
        }
        else
        {
            _logger.LogInformation("Added package {PackageId} v{Version} to {Project}", packageId, version, projectPath);
        }
    }

    public async Task AddProjectReferenceAsync(
        string projectPath,
        string referencePath,
        CancellationToken cancellationToken = default)
    {
        var command = $"add \"{projectPath}\" reference \"{referencePath}\"";
        var result = await RunDotNetCommandAsync(command, cancellationToken);

        if (!result.Success)
        {
            _logger.LogWarning("Failed to add project reference: {Error}", result.Error);
        }
        else
        {
            _logger.LogInformation("Added reference {Reference} to {Project}", referencePath, projectPath);
        }
    }

    public async Task<string> CreateSolutionAsync(
        string name,
        string directory,
        IEnumerable<string> projectPaths,
        CancellationToken cancellationToken = default)
    {
        var solutionPath = Path.Combine(directory, $"{name}.sln");

        // Create solution if it doesn't exist
        if (!File.Exists(solutionPath))
        {
            var createResult = await RunDotNetCommandAsync($"new sln -n {name} -o \"{directory}\"", cancellationToken);
            if (!createResult.Success)
            {
                throw new InvalidOperationException($"Failed to create solution: {createResult.Error}");
            }
        }

        // Add projects to solution
        foreach (var projectPath in projectPaths)
        {
            var addResult = await RunDotNetCommandAsync($"sln \"{solutionPath}\" add \"{projectPath}\"", cancellationToken);
            if (!addResult.Success)
            {
                _logger.LogWarning("Failed to add project to solution: {Error}", addResult.Error);
            }
        }

        _logger.LogInformation("Created solution {Name} at {Path}", name, solutionPath);
        return solutionPath;
    }

    public async Task<bool> BuildAsync(string projectPath, CancellationToken cancellationToken = default)
    {
        var command = $"build \"{projectPath}\" --no-restore";
        var result = await RunDotNetCommandAsync(command, cancellationToken);

        if (!result.Success)
        {
            _logger.LogWarning("Build failed: {Error}", result.Error);
            return false;
        }

        _logger.LogInformation("Build succeeded for {Project}", projectPath);
        return true;
    }

    private async Task<(bool Success, string Output, string Error)> RunDotNetCommandAsync(
        string arguments,
        CancellationToken cancellationToken)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = startInfo };
            
            // Start reading output/error asynchronously BEFORE starting the process
            var outputTask = Task.CompletedTask;
            var errorTask = Task.CompletedTask;
            var outputBuilder = new System.Text.StringBuilder();
            var errorBuilder = new System.Text.StringBuilder();

            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    outputBuilder.AppendLine(e.Data);
                }
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    errorBuilder.AppendLine(e.Data);
                }
            };

            process.Start();
            
            // Begin async reading
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            // Wait for process to exit
            process.WaitForExit();
            //await process.WaitForExitAsync(cancellationToken);

            // Get the collected output
            var output = outputBuilder.ToString();
            var error = errorBuilder.ToString();

            _logger.LogDebug("dotnet {Arguments} exited with code {ExitCode}", arguments, process.ExitCode);

            return (process.ExitCode == 0, output, error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to run dotnet command: {Arguments}", arguments);
            return (false, string.Empty, ex.Message);
        }
    }
}
