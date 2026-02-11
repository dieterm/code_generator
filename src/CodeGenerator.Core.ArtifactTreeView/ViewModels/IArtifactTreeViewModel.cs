using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using System.ComponentModel;
using System.Windows.Input;

namespace CodeGenerator.Core.Artifacts.ViewModels
{
    public interface IArtifactTreeViewModel : INotifyPropertyChanged
    {
        IArtifact? RootArtifact { get; set; }
        ICommand SelectArtifactCommand { get; }
        IArtifact? SelectedArtifact { get; set; }

        event EventHandler<IArtifact>? ArtifactRefreshRequested;
        event EventHandler<IArtifact>? ArtifactSelected;
        event EventHandler<IArtifact>? BeginRenameRequested;
        event EventHandler<ArtifactContextMenuEventArgs>? ContextMenuRequested;

        void Cleanup();
        IEnumerable<ArtifactTreeNodeCommand> GetContextMenuCommands(IArtifact artifact);
        Task HandleDoubleClickAsync(IArtifact artifact, CancellationToken cancellationToken = default);
        void RequestArtifactRefresh(IArtifact artifact);
        void RequestContextMenu(IArtifact artifact, int x, int y);
    }
}