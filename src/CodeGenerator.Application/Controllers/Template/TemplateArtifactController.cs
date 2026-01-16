using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Application.Services;
using CodeGenerator.Application.ViewModels.Template;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Templates;
using CodeGenerator.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Application.Controllers.Template
{
    public class TemplateArtifactController : ArtifactControllerBase<TemplateTreeViewController, TemplateArtifact>
    {
        private TemplateParametersViewModel? _editViewModel;
        private readonly WorkspaceTreeViewController _workspaceController;

        private void EnsureEditViewModel(TemplateArtifact templateArtifact)
        {
            if (_editViewModel == null || _editViewModel.TemplateArtifact != templateArtifact)
            {
                _editViewModel = new TemplateParametersViewModel();
                _editViewModel.SetWorkspaceController(_workspaceController);
                _editViewModel.ExecuteRequested += ParametersViewModel_ExecuteRequested;
            }
            _editViewModel.TemplateArtifact = templateArtifact;
        }
        public TemplateParametersViewModel? EditViewModel => _editViewModel;

        public TemplateArtifactController(WorkspaceTreeViewController workspaceController, TemplateTreeViewController treeViewController, ILogger<TemplateArtifactController> logger) 
            : base(treeViewController, logger)
        {
            _workspaceController = workspaceController;
            //treeViewController.TemplateSelected += TreeViewModel_TemplateSelected;
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(TemplateArtifact templateArtifact)
        {
            yield return new ArtifactTreeNodeCommand
            {
                Id = "edit_template",
                Text = "Edit Template",
                IconKey = "edit",
                Execute = async (a) =>
                {
                    var windowService = ServiceProviderHolder.GetRequiredService<IWindowManagerService>();
                    var previewViewModel = new ViewModels.ArtifactPreviewViewModel { 
                        TabLabel = templateArtifact.FileName, 
                        FilePath = templateArtifact.FilePath,
                        //TextContent = System.IO.File.ReadAllText(templateArtifact.FilePath), 
                        TextLanguageSchema = ViewModels.ArtifactPreviewViewModel.KnownLanguages.Text 
                    };
                    windowService.ShowArtifactPreview(previewViewModel);
                    await Task.CompletedTask;
                }
            };
            yield return new ArtifactTreeNodeCommand
            {
                Id = "execute_template",
                Text = "Execute",
                IconKey = "play",
                Execute = async (a) =>
                {
                    //OnTemplateSelected(templateArtifact);
                    await this.TreeViewController.SelectArtifactAsync(templateArtifact);
                    await Task.CompletedTask;
                }
            };

            yield return ArtifactTreeNodeCommand.Separator;

            yield return new ArtifactTreeNodeCommand
            {
                Id = "open_template_folder",
                Text = "Open Containing Folder",
                IconKey = "folder",
                Execute = async (a) =>
                {
                    var folder = Path.GetDirectoryName(templateArtifact.FilePath);
                    if (!string.IsNullOrEmpty(folder) && Directory.Exists(folder))
                    {
                        System.Diagnostics.Process.Start("explorer.exe", folder);
                    }
                    await Task.CompletedTask;
                }
            };
        }

        protected override Task OnSelectedInternalAsync(TemplateArtifact artifact, CancellationToken cancellationToken)
        {
            ShowTemplateParameters(artifact);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Show template parameters view for the selected template
        /// </summary>
        public void ShowTemplateParameters(TemplateArtifact template)
        {
            EnsureEditViewModel(template);

            _editViewModel!.TemplateArtifact = template;
            
            TreeViewController.ShowArtifactDetailsView(_editViewModel);
        }

        private async void ParametersViewModel_ExecuteRequested(object? sender, EventArgs e)
        {
            if (_editViewModel?.TemplateArtifact == null)
                return;

            await TreeViewController.ExecuteTemplateAsync(_editViewModel.TemplateArtifact, _editViewModel.GetParameterValues());
        }


    }
}
