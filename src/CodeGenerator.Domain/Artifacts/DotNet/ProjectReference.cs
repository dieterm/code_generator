namespace CodeGenerator.Domain.Artifacts.DotNet;

/// <summary>
/// Represents a project reference
/// </summary>
public class ProjectReference
{
    /// <summary>
    /// Relative path to the referenced project file
    /// </summary>
    public string ProjectPath { get; set; } = string.Empty;

    /// <summary>
    /// Reference to the actual project artifact (if available)
    /// </summary>
    public Artifact? ReferencedProject { get; set; }

    /// <summary>
    /// Name of the referenced project
    /// </summary>
    public string ProjectName => ReferencedProject?.GetDecorator<DotNetProjectDecorator>()?.ProjectName 
        ?? System.IO.Path.GetFileNameWithoutExtension(ProjectPath);

    public override string ToString() => ProjectName;
}
