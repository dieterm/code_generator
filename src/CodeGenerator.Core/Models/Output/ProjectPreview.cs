namespace CodeGenerator.Core.Models.Output;

/// <summary>
/// Preview of a project to be generated
/// </summary>
public class ProjectPreview
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string ProjectType { get; set; } = string.Empty;
    public List<string> Packages { get; set; } = new();
    public List<string> ProjectReferences { get; set; } = new();
}
