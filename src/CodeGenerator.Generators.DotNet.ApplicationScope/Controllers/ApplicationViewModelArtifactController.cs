using CodeGenerator.Application.Controllers.Template;
using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Templates;
using CodeGenerator.Generators.DotNet.Generators;
using CodeGenerator.Generators.DotNet.ApplicationScope.ViewModels;
using CodeGenerator.Generators.DotNet.ApplicationScope.Workspace.Artifacts;
using CodeGenerator.Shared;
using CodeGenerator.Shared.ExtensionMethods;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeGenerator.Generators.DotNet.ApplicationScope.Generators;

namespace CodeGenerator.Generators.DotNet.ApplicationScope.Controllers
{
    public class ApplicationViewModelArtifactController : WorkspaceArtifactControllerBase<ApplicationViewModelArtifact>
    {
        private readonly ApplicationViewModelArtifactEditViewModel _editViewModel;
        public ApplicationViewModelArtifactController(ApplicationViewModelArtifactEditViewModel editViewModel, OperationExecutor operationExecutor, WorkspaceTreeViewController treeViewController, ILogger<ApplicationViewModelArtifactController> logger) 
            : base(operationExecutor, treeViewController, logger)
        {
            _editViewModel = editViewModel;
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(ApplicationViewModelArtifact artifact)
        {
            yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "edit_applicationviewmodel_artifact",
                Text = "Edit Template",
                IconKey = "Edit",
                Execute = async (a) =>
                {
                    var templateManager = ServiceProviderHolder.GetRequiredService<TemplateManager>();
                    var templateEngineManager = ServiceProviderHolder.GetRequiredService<TemplateEngineManager>();
                    var templateId = TemplateIdParser.BuildGeneratorTemplateId(nameof(ApplicationControllerGenerator), ApplicationControllerGenerator.MAIN_VIEW_MODEL_TEMPLATE_NAME);
                    var templatePath = templateManager.ResolveTemplateIdToPath(templateId);
                    var extension = templatePath.GetFileExtension(false);
                    var templateEngine = templateEngineManager.GetTemplateEngineByFileExtension(extension);
                    var templateArtifact = new TemplateArtifact(templatePath, templateEngine);
                    var templateController = ServiceProviderHolder.GetRequiredService<TemplateTreeViewController>();
                    var parameters = new Dictionary<string, object?>
                    {
                        { "Namespace", artifact.Context.Namespace }
                    };
                    await templateController.OpenTemplateEditor(templateArtifact, parameters);
                }
            };
        }

        protected override async Task OnSelectedInternalAsync(ApplicationViewModelArtifact artifact, CancellationToken cancellationToken)
        {
            _editViewModel.Artifact = artifact;
            TreeViewController.ShowArtifactDetailsView(_editViewModel!);
        }
    }
}
