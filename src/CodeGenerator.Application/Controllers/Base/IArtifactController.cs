using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Templates;

namespace CodeGenerator.Application.Controllers.Base
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
        IEnumerable<ArtifactTreeNodeCommand> GetContextMenuCommands(IArtifact artifact);

        /// <summary>
        /// Called when the artifact is selected in the tree
        /// </summary>
        Task OnSelectedAsync(IArtifact artifact, CancellationToken cancellationToken = default);

        /// <summary>
        /// Called when the artifact is double-clicked
        /// </summary>
        Task OnDoubleClickAsync(IArtifact artifact, CancellationToken cancellationToken = default);
        
        void OnArtifactRenamed(IArtifact artifact, string oldName, string newName);

        /// <summary>
        /// Register required templates for this controller.
        /// Returns a list of template definitions with their expected locations.
        /// </summary>
        List<TemplateDefinitionAndLocation> RegisterRequiredTemplates();
    }
}
