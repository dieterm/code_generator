namespace CodeGenerator.Core.Models.Output;

/// <summary>
/// Preview of generation before execution
/// </summary>
public class GenerationPreview
{
    /// <summary>
    /// Files that will be created
    /// </summary>
    public List<FilePreview> FilesToCreate { get; set; } = new();

    /// <summary>
    /// Files that will be modified
    /// </summary>
    public List<FilePreview> FilesToModify { get; set; } = new();

    /// <summary>
    /// Projects that will be created
    /// </summary>
    public List<ProjectPreview> ProjectsToCreate { get; set; } = new();

    /// <summary>
    /// Folder structure that will be created
    /// </summary>
    public FolderNode RootFolder { get; set; } = new();

    /// <summary>
    /// Total number of files
    /// </summary>
    public int TotalFiles => FilesToCreate.Count + FilesToModify.Count;

    /// <summary>
    /// Total number of projects
    /// </summary>
    public int TotalProjects => ProjectsToCreate.Count;
}
