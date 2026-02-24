using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Services;
using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Templates;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Workspace;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Core.Workspaces.ViewModels;
using CodeGenerator.Core.Workspaces.ViewModels.Common;
using CodeGenerator.Core.Workspaces.ViewModels.Workspace;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.DesignPatterns.Structural.DependancyInjection;
using CodeGenerator.Domain.DotNet;
using CodeGenerator.Shared;
using CodeGenerator.Shared.ExtensionMethods;
using CodeGenerator.Shared.Operations;
using CodeGenerator.UserControls.ViewModels;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Logging;
using WorkspaceArtifactEditViewModel = CodeGenerator.Core.Workspaces.ViewModels.Workspace.WorkspaceArtifactEditViewModel;

namespace CodeGenerator.Application.Controllers.Workspace
{
    /// <summary>
    /// Controller for WorkspaceArtifact
    /// Handles context menus and detail views for the workspace root node
    /// </summary>
    public class WorkspaceArtifactController : WorkspaceArtifactControllerBase<WorkspaceArtifact>
    {
        private readonly IDatasourceFactory _datasourceFactory;
        private WorkspaceArtifactEditViewModel? _editViewModel;

        public WorkspaceArtifactController(
            OperationExecutor operationExecutor,
            IDatasourceFactory datasourceFactory, 
            WorkspaceTreeViewController workspaceController,
            ILogger<WorkspaceArtifactController> logger
            ): base(operationExecutor, workspaceController, logger)
        {
            _datasourceFactory = datasourceFactory;
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(WorkspaceArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Rename command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_RENAME)
            {
                Id = "rename_workspace",
                Text = "Rename",
                IconKey = "edit",
                Execute = async (a) => 
                {
                    TreeViewController.RequestBeginRename(artifact);
                    await Task.CompletedTask;
                }
            });
            
            var WORKSPACE_COMMANDS = "WorkspaceCommands";
            // Workspace commands
            commands.Add(new ArtifactTreeNodeCommand(WORKSPACE_COMMANDS)
            {
                Id = "save_workspace",
                Text = "Save Workspace",
                IconKey = "save",
                Execute = async (a) => await TreeViewController.SaveWorkspaceAsync()
            });

            commands.Add(new ArtifactTreeNodeCommand(WORKSPACE_COMMANDS)
            {
                Id = "workspace_properties",
                Text = "Properties",
                IconKey = "settings",
                Execute = async (a) => await ShowPropertiesAsync(artifact)
            });

            commands.Add(new ArtifactTreeNodeCommand(WORKSPACE_COMMANDS)
            {
                Id = "open_workspace_folder",
                Text = "Open Workspace Folder",
                IconKey = "folder",
                Execute = async (a) =>
                {
                    artifact.WorkspaceDirectory?.OpenFolderInExplorer();
                    await Task.CompletedTask;
                }
            });

            return commands;
        }

        protected override Task OnSelectedInternalAsync(WorkspaceArtifact artifact, CancellationToken cancellationToken)
        {
            return ShowPropertiesAsync(artifact);
        }

        
        private void EnsureEditViewModel(WorkspaceArtifact artifact)
        {
            if (_editViewModel == null)
            {
                _editViewModel = new WorkspaceArtifactEditViewModel();
                _editViewModel.ValueChanged += OnEditViewModelValueChanged;
            }
            
            _editViewModel.Artifact = artifact;
        }

        private void OnEditViewModelValueChanged(object? sender, ArtifactPropertyChangedEventArgs e)
        {
            // Notify the controller about the property change
            if (e.Artifact is IArtifact artifact)
            {
                TreeViewController.OnArtifactPropertyChanged(artifact, e.PropertyName, e.NewValue);
            }
        }

        private async Task AddDatasourceAsync(WorkspaceArtifact workspace, string typeId)
        {
            var datasource = TreeViewController.AddDatasource(typeId, $"New {typeId} Datasource");
            if (datasource != null)
            {
                await TreeViewController.SaveWorkspaceAsync();
            }
        }

        private Task ShowPropertiesAsync(WorkspaceArtifact workspace)
        {
            EnsureEditViewModel(workspace);
            TreeViewController.ShowArtifactDetailsView(_editViewModel!);
            return Task.CompletedTask;
        }

    }
}
