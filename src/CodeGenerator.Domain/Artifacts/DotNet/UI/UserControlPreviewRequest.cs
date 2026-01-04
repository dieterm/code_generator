namespace CodeGenerator.Domain.Artifacts.DotNet.UI;

/// <summary>
/// Request object for UserControl preview
/// </summary>
public class UserControlPreviewRequest
{
    /// <summary>
    /// The artifact to preview
    /// </summary>
    public Artifact Artifact { get; }

    /// <summary>
    /// The UserControl decorator
    /// </summary>
    public UserControlDecorator? Decorator => Artifact.GetDecorator<UserControlDecorator>();

    /// <summary>
    /// Additional assemblies needed for compilation
    /// </summary>
    public List<string> AdditionalAssemblies { get; } = new();

    /// <summary>
    /// Additional source files needed for compilation
    /// </summary>
    public List<string> AdditionalSourceFiles { get; } = new();

    public UserControlPreviewRequest(Artifact artifact)
    {
        Artifact = artifact ?? throw new ArgumentNullException(nameof(artifact));
    }
}
