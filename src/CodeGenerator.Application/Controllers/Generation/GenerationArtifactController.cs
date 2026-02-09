using CodeGenerator.Application.Controllers.ArtifactPreview;
using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.ViewModels;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Shared;
using CodeGenerator.Shared.ExtensionMethods;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Generation
{
    /// <summary>
    /// Default controller for generation artifacts.
    /// Shows a preview when a non-folder artifact is selected.
    /// </summary>
    public class GenerationArtifactController : GenerationArtifactControllerBase<IArtifact>
    {
        public GenerationArtifactController(OperationExecutor operationExecutor,
            GenerationTreeViewController treeViewController,
            ILogger<GenerationArtifactController> logger)
            : base(operationExecutor, treeViewController, logger)
        {
        }

        public override bool CanHandle(IArtifact artifact)
        {
            // Handle all artifacts that are not folders
            return artifact is not FolderArtifact;
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(IArtifact artifact)
        {
            return new List<ArtifactTreeNodeCommand>();
        }

        protected override Task OnSelectedInternalAsync(IArtifact artifact, CancellationToken cancellationToken)
        {
            if (artifact.CanPreview)
            {
                var previewController = ServiceProviderHolder.GetRequiredService<ArtifactPreviewController>();

                object? previewOutput = artifact.CreatePreview();
                if (previewOutput is string textContent)
                {
                    var fileExtension = artifact.GetDecoratorOfType<FileArtifactDecorator>()?.FileName?.GetFileExtension();
                    var syntax = previewController.GetSyntaxByFileExtension(fileExtension);

                    previewController.ShowArtifactPreview(new ArtifactPreviewViewModel
                    {
                        FileName = artifact.TreeNodeText,
                        TabLabel = artifact.TreeNodeText,
                        TextContent = textContent,
                        TextLanguageSchema = syntax
                    });
                }
            }

            return Task.CompletedTask;
        }
    }
}
