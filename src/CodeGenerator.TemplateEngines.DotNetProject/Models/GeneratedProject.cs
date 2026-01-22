namespace CodeGenerator.TemplateEngines.DotNetProject.Models;

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
    /// Files in this project
    /// </summary>
    public List<GeneratedFile> Files { get; set; } = new();
}
