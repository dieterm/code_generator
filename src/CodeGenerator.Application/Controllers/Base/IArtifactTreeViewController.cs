using CodeGenerator.Application.ViewModels.Base;
using CodeGenerator.Core.Artifacts;

namespace CodeGenerator.Application.Controllers.Base
{
    public interface IArtifactTreeViewController
    {
        IArtifactTreeViewModel? TreeViewModel { get; set; }

        event EventHandler<IArtifact>? BeginRenameRequested;
        event EventHandler<ArtifactRenamedEventArgs>? ArtifactRenamed;
        event EventHandler<IArtifact?>? ArtifactSelected;
        event EventHandler<ArtifactChildChangedEventArgs>? ArtifactRemoved;
        event EventHandler<ArtifactChildChangedEventArgs>? ArtifactAdded;
        event EventHandler<ArtifactPropertyChangedEventArgs>? ArtifactPropertyChanged;

        IEnumerable<ArtifactTreeNodeCommand> GetContextMenuCommands(IArtifact artifact);
        IArtifactController? GetController(IArtifact artifact);
        Task HandleDoubleClickAsync(IArtifact artifact, CancellationToken cancellationToken = default);
        void OnArtifactRenamed(IArtifact artifact, string oldName, string newName);
        Task SelectArtifactAsync(IArtifact artifact, CancellationToken cancellationToken = default);

        #region Clipboard Operations

        /// <summary>
        /// Copy the specified artifact to the clipboard
        /// </summary>
        void CopyArtifact(IArtifact artifact);

        /// <summary>
        /// Cut the specified artifact to the clipboard
        /// </summary>
        void CutArtifact(IArtifact artifact);

        /// <summary>
        /// Paste the clipboard content onto the specified target artifact
        /// </summary>
        void PasteArtifact(IArtifact targetArtifact);

        /// <summary>
        /// Delete the specified artifact
        /// </summary>
        void DeleteArtifact(IArtifact artifact);

        /// <summary>
        /// Check if copy is available for the specified artifact
        /// </summary>
        bool CanCopyArtifact(IArtifact artifact);

        /// <summary>
        /// Check if cut is available for the specified artifact
        /// </summary>
        bool CanCutArtifact(IArtifact artifact);

        /// <summary>
        /// Check if paste is available for the specified target artifact
        /// </summary>
        bool CanPasteArtifact(IArtifact targetArtifact);

        /// <summary>
        /// Check if delete is available for the specified artifact
        /// </summary>
        bool CanDeleteArtifact(IArtifact artifact);

        #endregion
    }
}