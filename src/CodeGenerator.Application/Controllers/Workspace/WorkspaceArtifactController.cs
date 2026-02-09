using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Services;
using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace
{
    /// <summary>
    /// Controller for WorkspaceArtifact
    /// Handles context menus and detail views for the workspace root node
    /// </summary>
    public class WorkspaceArtifactController : WorkspaceArtifactControllerBase<WorkspaceArtifact>
    {
        private readonly IDatasourceFactory _datasourceFactory;
        private WorkspaceEditViewModel? _editViewModel;

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
            var DATASOURCE_COMMANDS = "DatasourceCommands";
            // Add datasource submenu
            var addDatasourceSubCommands = new List<ArtifactTreeNodeCommand>();
            
            foreach (var typeInfo in _datasourceFactory.GetAvailableTypes())
            {
                addDatasourceSubCommands.Add(new ArtifactTreeNodeCommand(DATASOURCE_COMMANDS)
                {
                    Id = $"add_datasource_{typeInfo.TypeId}",
                    Text = typeInfo.DisplayName,
                    IconKey = typeInfo.IconKey,
                    Execute = async (a) => await AddDatasourceAsync(artifact, typeInfo.TypeId)
                });
            }

            if (addDatasourceSubCommands.Any())
            {
                commands.Add(new ArtifactTreeNodeCommand(DATASOURCE_COMMANDS)
                {
                    Id = "add_datasource",
                    Text = "Add Datasource",
                    IconKey = "plus",
                    SubCommands = addDatasourceSubCommands
                });
            }
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
                _editViewModel = new WorkspaceEditViewModel();
                _editViewModel.ValueChanged += OnEditViewModelValueChanged;
            }

            _editViewModel.Workspace = artifact;
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
