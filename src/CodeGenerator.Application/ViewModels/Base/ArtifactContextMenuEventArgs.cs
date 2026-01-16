using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;

namespace CodeGenerator.Application.ViewModels.Base;

/// <summary>
/// Event args for artifact context menu request
/// </summary>
public class ArtifactContextMenuEventArgs : EventArgs
{
    public IArtifact Artifact { get; }
    public IEnumerable<ArtifactTreeNodeCommand> Commands { get; }
    public int X { get; }
    public int Y { get; }

    public ArtifactContextMenuEventArgs(IArtifact artifact, IEnumerable<ArtifactTreeNodeCommand> commands, int x, int y)
    {
        Artifact = artifact;
        Commands = commands;
        X = x;
        Y = y;
    }
}
