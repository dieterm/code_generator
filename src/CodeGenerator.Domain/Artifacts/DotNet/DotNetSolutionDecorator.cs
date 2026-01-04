namespace CodeGenerator.Domain.Artifacts.DotNet;

using CodeGenerator.Domain.Artifacts.FileSystem;
using CodeGenerator.Domain.Previewers;

/// <summary>
/// Decorator that marks an artifact as a .NET solution
/// </summary>
public class DotNetSolutionDecorator : DirectoryDecorator
{
    /// <summary>
    /// Solution name
    /// </summary>
    public string SolutionName
    {
        get => Artifact?.GetProperty<string>("SolutionName") ?? Artifact?.Name ?? string.Empty;
        set => Artifact?.SetProperty("SolutionName", value);
    }

    /// <summary>
    /// Projects in the solution (children with DotNetProjectDecorator)
    /// </summary>
    public IEnumerable<Artifact> Projects =>
        Artifact?.Children.Where(c => c.HasDecorator<DotNetProjectDecorator>()) ?? Enumerable.Empty<Artifact>();

    /// <summary>
    /// Full path to the solution file
    /// </summary>
    public string SolutionFilePath => System.IO.Path.Combine(FullPath, $"{SolutionName}.sln");

    /// <summary>
    /// Solution folders for organizing projects
    /// </summary>
    public List<SolutionFolder> SolutionFolders
    {
        get
        {
            var folders = Artifact?.GetProperty<List<SolutionFolder>>("SolutionFolders");
            if (folders == null)
            {
                folders = new List<SolutionFolder>();
                Artifact?.SetProperty("SolutionFolders", folders);
            }
            return folders;
        }
    }

    /// <summary>
    /// Add a project to the solution
    /// </summary>
    public Artifact AddProject(string projectName, DotNetProjectType projectType = DotNetProjectType.ClassLibrary)
    {
        var project = new Artifact { Name = projectName };
        var decorator = new DotNetProjectDecorator
        {
            ProjectName = projectName,
            ProjectType = projectType
        };
        project.AddDecorator(decorator);
        Artifact?.AddChild(project);
        return project;
    }

    /// <summary>
    /// Add a project to a solution folder
    /// </summary>
    public void AddProjectToFolder(Artifact project, string folderName)
    {
        var folder = SolutionFolders.FirstOrDefault(f => f.Name == folderName);
        if (folder == null)
        {
            folder = new SolutionFolder { Name = folderName };
            SolutionFolders.Add(folder);
        }
        folder.Projects.Add(project);
        Artifact?.AddChild(project);
    }

    public override object? CreatePreview()
    {
        if (Artifact == null) return null;
        return new SolutionPreviewModel(Artifact);
    }
}

/// <summary>
/// Represents a solution folder for organizing projects
/// </summary>
public class SolutionFolder
{
    public string Name { get; set; } = string.Empty;
    public List<Artifact> Projects { get; } = new();
    public List<SolutionFolder> SubFolders { get; } = new();
}

/// <summary>
/// Model for solution preview
/// </summary>
public class SolutionPreviewModel
{
    public string SolutionName { get; }
    public List<ProjectPreviewModel> Projects { get; } = new();

    public SolutionPreviewModel(Artifact artifact)
    {
        var decorator = artifact.GetDecorator<DotNetSolutionDecorator>();
        SolutionName = decorator?.SolutionName ?? artifact.Name;
        
        foreach (var project in artifact.Children.Where(c => c.HasDecorator<DotNetProjectDecorator>()))
        {
            Projects.Add(new ProjectPreviewModel(project));
        }
    }
}
