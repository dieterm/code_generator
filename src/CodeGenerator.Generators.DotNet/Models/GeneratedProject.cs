namespace CodeGenerator.Generators.DotNet.Models;

/// <summary>
/// Represents a generated project
/// </summary>
public class GeneratedProject
{
    /// <summary>
    /// Project name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Project file path
    /// </summary>
    public string ProjectFilePath { get; set; } = string.Empty;

    /// <summary>
    /// Project directory
    /// </summary>
    public string Directory { get; set; } = string.Empty;

    /// <summary>
    /// Target framework
    /// </summary>
    public string TargetFramework { get; set; } = "net8.0";

    /// <summary>
    /// Project type
    /// </summary>
    public string ProjectType { get; set; } = "classlib";

    /// <summary>
    /// NuGet packages included
    /// </summary>
    public List<string> Packages { get; set; } = new();

    /// <summary>
    /// Project references
    /// </summary>
    public List<string> ProjectReferences { get; set; } = new();

    /// <summary>
    /// Files in this project
    /// </summary>
    public List<GeneratedFile> Files { get; set; } = new();
}
