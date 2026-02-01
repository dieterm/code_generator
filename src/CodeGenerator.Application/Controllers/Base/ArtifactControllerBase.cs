using CodeGenerator.Application.Services;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.Views;
using CodeGenerator.Core.Templates;
using CodeGenerator.Core.Workspaces.MessageBus;
using CodeGenerator.Shared;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Base
{
    /// <summary>
    /// Base class for artifact controllers
    /// </summary>
    public abstract class ArtifactControllerBase<TTreeView, TArtifact> : IArtifactController where TArtifact : class, IArtifact where TTreeView : IArtifactTreeViewController
    {
        protected ILogger Logger { get; }
        protected TTreeView TreeViewController { get; }

        /// <summary>
        /// Access to the clipboard service
        /// </summary>
        protected ArtifactClipboardService ClipboardService => ArtifactClipboardService.Instance;

        protected ArtifactControllerBase(TTreeView treeViewController, ILogger logger)
        {
            TreeViewController = treeViewController;
            Logger = logger;
        }

        public Type ArtifactType => typeof(TArtifact);

        public virtual bool CanHandle(IArtifact artifact)
        {
            return artifact is TArtifact;
        }

        public IEnumerable<ArtifactTreeNodeCommand> GetContextMenuCommands(IArtifact artifact)
        {
            if (artifact is TArtifact typedArtifact)
            {
                var commands = GetCommands(typedArtifact).ToList();
                
                // Add clipboard commands
                var clipboardCommands = GetClipboardCommands(typedArtifact);
                if (clipboardCommands.Any())
                {
                    commands.AddRange(clipboardCommands);
                }
                
                var messageBus = ServiceProviderHolder.GetRequiredService<WorkspaceMessageBus>().PublishArtifactContextMenuOpening(typedArtifact, commands);

                return messageBus.Commands;
            }
            return Enumerable.Empty<ArtifactTreeNodeCommand>();
        }

        /// <summary>
        /// Get clipboard-related context menu commands
        /// </summary>
        protected virtual IEnumerable<ArtifactTreeNodeCommand> GetClipboardCommands(TArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Cut command
            if (CanCut(artifact))
            {
                commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_EDIT)
                {
                    Id = "cut",
                    Text = "Cut",
                    IconKey = "scissors",
                    Execute = async (a) => Cut(a as TArtifact),
                    CanExecute = (a) => CanCut(a as TArtifact)
                });
            }

            // Copy command
            if (CanCopy(artifact))
            {
                commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_EDIT)
                {
                    Id = "copy",
                    Text = "Copy",
                    IconKey = "copy",
                    Execute = async (a) => Copy(a as TArtifact),
                    CanExecute = (a) => CanCopy(a as TArtifact)
                });
            }

            // Paste command
            var clipboardArtifact = ClipboardService.GetArtifact();
            if (clipboardArtifact != null && CanPaste(clipboardArtifact, artifact))
            {
                commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_EDIT)
                {
                    Id = "paste",
                    Text = "Paste",
                    IconKey = "clipboard-paste",
                    Execute = async (a) => 
                    {
                        var toPaste = ClipboardService.GetArtifact();
                        if (toPaste != null)
                        {
                            Paste(toPaste, a as TArtifact);
                        }
                    },
                    CanExecute = (a) => 
                    {
                        var toPaste = ClipboardService.GetArtifact();
                        return toPaste != null && CanPaste(toPaste, a as TArtifact);
                    }
                });
            }

            // Delete command
            if (CanDelete(artifact))
            {
                commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_DELETE)
                {
                    Id = "delete",
                    Text = "Delete",
                    IconKey = "trash",
                    Execute = async (a) => Delete(a as TArtifact),
                    CanExecute = (a) => CanDelete(a as TArtifact)
                });
            }

            return commands;
        }

        public async Task OnSelectedAsync(IArtifact artifact, CancellationToken cancellationToken = default)
        {
            if (artifact is TArtifact typedArtifact)
            {
                await OnSelectedInternalAsync(typedArtifact, cancellationToken);
            }
        }

        public async Task OnDoubleClickAsync(IArtifact artifact, CancellationToken cancellationToken = default)
        {
            if (artifact is TArtifact typedArtifact)
            {
                await OnDoubleClickInternalAsync(typedArtifact, cancellationToken);
            }
        }

        /// <summary>
        /// Get commands for the artifact (excluding clipboard commands which are added automatically)
        /// </summary>
        protected abstract IEnumerable<ArtifactTreeNodeCommand> GetCommands(TArtifact artifact);

        /// <summary>
        /// Handle artifact selection
        /// </summary>
        protected virtual Task OnSelectedInternalAsync(TArtifact artifact, CancellationToken cancellationToken) => Task.CompletedTask;

        /// <summary>
        /// Handle artifact double-click
        /// </summary>
        protected virtual Task OnDoubleClickInternalAsync(TArtifact artifact, CancellationToken cancellationToken) => Task.CompletedTask;

        protected virtual void OnArtifactRenamedInternal(TArtifact artifact, string oldName, string newName)
        {
            // Default implementation does nothing
        }

        void IArtifactController.OnArtifactRenamed(IArtifact artifact, string oldName, string newName)
        {
            if (artifact is TArtifact typedArtifact)
            {
                OnArtifactRenamedInternal(typedArtifact, oldName, newName);
            }
        }

        /// <summary>
        /// Return a list of required template IDs for this controller
        /// </summary>
        /// <returns></returns>
        public virtual List<TemplateDefinitionAndLocation> RegisterRequiredTemplates()
        {
            return new List<TemplateDefinitionAndLocation>();
        }

        #region Clipboard Operations - Default Implementations

        /// <summary>
        /// Check if the artifact can be copied. Override to enable copy for specific artifacts.
        /// </summary>
        public virtual bool CanCopy(TArtifact artifact)
        {
            return false; // Disabled by default, override in derived classes
        }

        /// <summary>
        /// Check if the artifact can be cut. Override to enable cut for specific artifacts.
        /// </summary>
        public virtual bool CanCut(TArtifact artifact)
        {
            return false; // Disabled by default, override in derived classes
        }

        /// <summary>
        /// Check if the artifact can be deleted. Override to enable delete for specific artifacts.
        /// </summary>
        public virtual bool CanDelete(TArtifact artifact)
        {
            return false; // Disabled by default, override in derived classes
        }

        /// <summary>
        /// Check if an artifact can be pasted onto this target. Override to enable paste for specific artifacts.
        /// </summary>
        public virtual bool CanPaste(IArtifact artifactToPaste, TArtifact targetArtifact)
        {
            return false; // Disabled by default, override in derived classes
        }

        /// <summary>
        /// Copy the artifact to the clipboard
        /// </summary>
        public virtual void Copy(TArtifact artifact)
        {
            if (!CanCopy(artifact))
            {
                Logger.LogWarning("Cannot copy artifact {ArtifactId}", artifact.Id);
                return;
            }

            ClipboardService.Copy(artifact);
            Logger.LogDebug("Copied artifact {ArtifactId} to clipboard", artifact.Id);
        }

        /// <summary>
        /// Cut the artifact to the clipboard
        /// </summary>
        public virtual void Cut(TArtifact artifact)
        {
            if (!CanCut(artifact))
            {
                Logger.LogWarning("Cannot cut artifact {ArtifactId}", artifact.Id);
                return;
            }

            ClipboardService.Cut(artifact);
            Logger.LogDebug("Cut artifact {ArtifactId} to clipboard", artifact.Id);
        }

        /// <summary>
        /// Delete the artifact. Override to implement actual deletion logic.
        /// </summary>
        public virtual void Delete(TArtifact artifact)
        {
            if (!CanDelete(artifact))
            {
                Logger.LogWarning("Cannot delete artifact {ArtifactId}", artifact.Id);
                return;
            }

            // Default implementation - derived classes should override for actual deletion
            Logger.LogWarning("Delete not implemented for artifact type {ArtifactType}", artifact.GetType().Name);
        }

        /// <summary>
        /// Paste the artifact onto the target. Override to implement actual paste logic.
        /// </summary>
        public virtual void Paste(IArtifact artifactToPaste, TArtifact targetArtifact)
        {
            if (!CanPaste(artifactToPaste, targetArtifact))
            {
                Logger.LogWarning("Cannot paste artifact {SourceId} onto {TargetId}", 
                    artifactToPaste.Id, targetArtifact.Id);
                return;
            }

            // Default implementation - derived classes should override for actual paste
            Logger.LogWarning("Paste not implemented for target type {TargetType}", targetArtifact.GetType().Name);
        }

        bool IArtifactController.CanCopy(IArtifact artifact)
        {

            return artifact is TArtifact && CanCopy((TArtifact)artifact);
        }

        bool IArtifactController.CanCut(IArtifact artifact)
        {
            return artifact is TArtifact && CanCut((TArtifact)artifact);
        }

        bool IArtifactController.CanDelete(IArtifact artifact)
        {
            return artifact is TArtifact && CanDelete((TArtifact)artifact);
        }

        bool IArtifactController.CanPaste(IArtifact artifactToPaste, IArtifact targetArtifact)
        {
            return targetArtifact is TArtifact && CanPaste(artifactToPaste, (TArtifact)targetArtifact);
        }

        void IArtifactController.Copy(IArtifact artifact)
        {
            Copy((TArtifact)artifact);
        }

        void IArtifactController.Cut(IArtifact artifact)
        {
            Cut((TArtifact)artifact);
        }

        void IArtifactController.Delete(IArtifact artifact)
        {
            Delete((TArtifact)artifact);
        }

        void IArtifactController.Paste(IArtifact artifactToPaste, IArtifact targetArtifact)
        {
            Paste(artifactToPaste, (TArtifact)targetArtifact);
        }

        #endregion
    }
}
