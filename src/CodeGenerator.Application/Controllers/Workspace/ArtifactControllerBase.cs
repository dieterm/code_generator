using CodeGenerator.Application.Services;
using CodeGenerator.Core.Artifacts;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace
{
    /// <summary>
    /// Base class for artifact controllers
    /// </summary>
    public abstract class ArtifactControllerBase<TArtifact> : IArtifactController where TArtifact : class, IArtifact
    {
        protected ILogger Logger { get; private set; }
        protected WorkspaceController WorkspaceController { get; private set; }

        protected ArtifactControllerBase(WorkspaceController workspaceController, ILogger logger)
        {
            WorkspaceController = workspaceController;
            Logger = logger;
        }
        public Type ArtifactType => typeof(TArtifact);

        public virtual bool CanHandle(IArtifact artifact)
        {
            return artifact is TArtifact;
        }

        public IEnumerable<WorkspaceCommand> GetContextMenuCommands(IArtifact artifact)
        {
            if (artifact is TArtifact typedArtifact)
            {
                return GetCommands(typedArtifact);
            }
            return Enumerable.Empty<WorkspaceCommand>();
        }

        //public object? CreateDetailView(IArtifact artifact)
        //{
        //    if (artifact is TArtifact typedArtifact)
        //    {
        //        return CreateDetailViewInternal(typedArtifact);
        //    }
        //    return null;
        //}

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
        protected abstract IEnumerable<WorkspaceCommand> GetCommands(TArtifact artifact);

        ///// <summary>
        ///// Create the detail view for the artifact
        ///// </summary>
        //protected virtual object? CreateDetailViewInternal(TArtifact artifact) => null;

        /// <summary>
        /// Handle artifact selection
        /// </summary>
        protected virtual Task OnSelectedInternalAsync(TArtifact artifact, CancellationToken cancellationToken) => Task.CompletedTask;

        /// <summary>
        /// Handle artifact double-click
        /// </summary>
        protected virtual Task OnDoubleClickInternalAsync(TArtifact artifact, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
