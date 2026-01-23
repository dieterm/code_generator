using CodeGenerator.Application.Services;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Templates;
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
                return GetCommands(typedArtifact);
            }
            return Enumerable.Empty<ArtifactTreeNodeCommand>();
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
        /// Get commands for the artifact
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
    }
}
