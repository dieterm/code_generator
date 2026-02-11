using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Controllers.Generation;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.ViewModels;

namespace CodeGenerator.Application.ViewModels.Generation
{
    /// <summary>
    /// ViewModel for the generation tree view (artifact tree)
    /// </summary>
    public class GenerationTreeViewModel : ArtifactTreeViewModel<GenerationTreeViewController>
    {
        public GenerationTreeViewModel(GenerationTreeViewController treeViewController)
            : base(treeViewController)
        {
        }

        public override IEnumerable<ArtifactTreeNodeCommand> GetContextMenuCommands(IArtifact artifact)
        {
            return TreeViewController.GetContextMenuCommands(artifact);
        }

        public override async Task HandleDoubleClickAsync(IArtifact artifact, CancellationToken cancellationToken = default)
        {
            await TreeViewController.HandleDoubleClickAsync(artifact, cancellationToken);
        }
    }
}
