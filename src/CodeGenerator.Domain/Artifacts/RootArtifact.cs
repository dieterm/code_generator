namespace CodeGenerator.Domain.Artifacts;

using CodeGenerator.Domain.Artifacts.DotNet;

/// <summary>
/// Decorator for the root artifact that serves as the top-level container
/// </summary>
public class RootDecorator : ArtifactDecorator
{
    /// <summary>
    /// The solution artifact if this is a .NET generation
    /// </summary>
    public Artifact? Solution => 
        Artifact?.Children.FirstOrDefault(c => c.HasDecorator<DotNetSolutionDecorator>());

    /// <summary>
    /// Get the solution decorator
    /// </summary>
    public DotNetSolutionDecorator? SolutionDecorator => Solution?.GetDecorator<DotNetSolutionDecorator>();
}

/// <summary>
/// Factory for creating common artifacts
/// </summary>
public static class ArtifactFactory
{
    /// <summary>
    /// Create a root artifact
    /// </summary>
    public static Artifact CreateRoot(string name = "Root")
    {
        var artifact = new Artifact { Name = name };
        artifact.AddDecorator(new RootDecorator());
        return artifact;
    }

    /// <summary>
    /// Create a directory artifact
    /// </summary>
    public static Artifact CreateDirectory(string name)
    {
        var artifact = new Artifact { Name = name };
        artifact.AddDecorator(new FileSystem.DirectoryDecorator());
        return artifact;
    }

    /// <summary>
    /// Create a file artifact
    /// </summary>
    public static Artifact CreateFile(string name, string content, string extension = "")
    {
        var artifact = new Artifact { Name = name };
        var decorator = new FileSystem.FileDecorator
        {
            Content = content,
            Extension = extension
        };
        artifact.AddDecorator(decorator);
        return artifact;
    }

    /// <summary>
    /// Create a text file artifact
    /// </summary>
    public static Artifact CreateTextFile(string name, string content, string extension = ".txt", string mimeType = "text/plain")
    {
        var artifact = new Artifact { Name = name };
        var decorator = new FileSystem.TextFileDecorator
        {
            Content = content,
            Extension = extension,
            MimeType = mimeType
        };
        artifact.AddDecorator(decorator);
        return artifact;
    }

    /// <summary>
    /// Create a .NET solution artifact
    /// </summary>
    public static Artifact CreateSolution(string name)
    {
        var artifact = new Artifact { Name = name };
        var decorator = new DotNetSolutionDecorator { SolutionName = name };
        artifact.AddDecorator(decorator);
        return artifact;
    }

    /// <summary>
    /// Create a .NET project artifact
    /// </summary>
    public static Artifact CreateProject(string name, DotNetProjectType projectType = DotNetProjectType.ClassLibrary)
    {
        var artifact = new Artifact { Name = name };
        var decorator = new DotNetProjectDecorator
        {
            ProjectName = name,
            ProjectType = projectType
        };
        artifact.AddDecorator(decorator);
        return artifact;
    }
}
