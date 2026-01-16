using CodeGenerator.Application.ViewModels.Base;
using CodeGenerator.Core.Artifacts;

namespace CodeGenerator.Application.Controllers.Base
{
    public interface IArtifactTreeViewController
    {
        IArtifactTreeViewModel? TreeViewModel { get; set; }

        event EventHandler<ArtifactRenamedEventArgs>? ArtifactRenamed;
        event EventHandler<IArtifact?>? ArtifactSelected;

        IEnumerable<ArtifactTreeNodeCommand> GetContextMenuCommands(IArtifact artifact);
        IArtifactController? GetController(IArtifact artifact);
        Task HandleDoubleClickAsync(IArtifact artifact, CancellationToken cancellationToken = default);
        void OnArtifactRenamed(IArtifact artifact, string oldName, string newName);
        Task SelectArtifactAsync(IArtifact artifact, CancellationToken cancellationToken = default);
    }
}