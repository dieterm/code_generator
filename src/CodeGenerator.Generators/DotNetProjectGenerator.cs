using System.Diagnostics;
using CodeGenerator.Core.Interfaces;
using CodeGenerator.Core.Models.Output;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Generators;

/// <summary>
/// Generates .NET projects using dotnet CLI and MSBuild
/// </summary>
public class DotNetProjectGenerator : IProjectGenerator
{
    private readonly ILogger<DotNetProjectGenerator> _logger;

    public DotNetProjectGenerator(ILogger<DotNetProjectGenerator> logger)
    {
        _logger = logger;
    }

    public async Task<GeneratedProject> CreateProjectAsync(
        string name,
        string directory,
        string projectType,
        string targetFramework,
        CancellationToken cancellationToken = default)
    {
        var project = new GeneratedProject
        {
            Name = name,
            Directory = directory,
            ProjectType = projectType,
            TargetFramework = targetFramework,
            ProjectFilePath = Path.Combine(directory, $"{name}.csproj")
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

        // Create project using dotnet CLI
        var command = $"new {projectType} -n {name} -f {targetFramework} -o \"{directory}\"";
        var result = await RunDotNetCommandAsync(command, cancellationToken);

        if (!result.Success)
        {
            throw new InvalidOperationException($"Failed to create project: {result.Error}");
        }

        _logger.LogInformation("Created project {Name} at {Path}", name, project.ProjectFilePath);
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
            process.Start();

            var output = await process.StandardOutput.ReadToEndAsync(cancellationToken);
            var error = await process.StandardError.ReadToEndAsync(cancellationToken);

            await process.WaitForExitAsync(cancellationToken);

            return (process.ExitCode == 0, output, error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to run dotnet command: {Arguments}", arguments);
            return (false, string.Empty, ex.Message);
        }
    }
}
