using CodeGenerator.Core.Artifacts;

namespace CodeGenerator.Application.Controllers.Workspace
{
    /// <summary>
    /// Base interface for artifact controllers
    /// Controllers handle context menus, details views, and actions for specific artifact types
    /// </summary>
    public interface IArtifactController
    {
        /// <summary>
        /// Type of artifact this controller handles
        /// </summary>
        Type ArtifactType { get; }

        /// <summary>
        /// Check if this controller can handle the given artifact
        /// </summary>
        bool CanHandle(IArtifact artifact);

        /// <summary>
        /// Get context menu commands for the artifact
        /// </summary>
        IEnumerable<WorkspaceCommand> GetContextMenuCommands(IArtifact artifact);

        ///// <summary>
        ///// Get the detail view control for the artifact
        ///// Returns null if no detail view is available
        ///// </summary>
        //object? CreateDetailView(IArtifact artifact);

        /// <summary>
        /// Called when the artifact is selected in the tree
        /// </summary>
        Task OnSelectedAsync(IArtifact artifact, CancellationToken cancellationToken = default);

        /// <summary>
        /// Called when the artifact is double-clicked
        /// </summary>
        Task OnDoubleClickAsync(IArtifact artifact, CancellationToken cancellationToken = default);
    }
}
