using CodeGenerator.Application.Services;
using CodeGenerator.Application.ViewModels;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Services;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace
{
    /// <summary>
    /// Controller for WorkspaceArtifact
    /// Handles context menus and detail views for the workspace root node
    /// </summary>
    public class WorkspaceArtifactController : ArtifactControllerBase<WorkspaceArtifact>
    {
        private readonly IDatasourceFactory _datasourceFactory;
        private WorkspaceEditViewModel? _editViewModel;

        public WorkspaceArtifactController(
            IDatasourceFactory datasourceFactory, 
            WorkspaceController workspaceController,
            ILogger<WorkspaceArtifactController> logger
            ): base(workspaceController, logger)
        {
            _datasourceFactory = datasourceFactory;
        }

        protected override IEnumerable<WorkspaceCommand> GetCommands(WorkspaceArtifact artifact)
        {
            var commands = new List<WorkspaceCommand>();

            // Rename command
            commands.Add(new WorkspaceCommand
            {
                Id = "rename_workspace",
                Text = "Rename",
                IconKey = "edit",
                Execute = async (a) => 
                {
                    WorkspaceController.RequestBeginRename(artifact);
                    await Task.CompletedTask;
                }
            });

            commands.Add(WorkspaceCommand.Separator);

            // Add datasource submenu
            var addDatasourceSubCommands = new List<WorkspaceCommand>();
            
            foreach (var typeInfo in _datasourceFactory.GetAvailableTypes())
            {
                addDatasourceSubCommands.Add(new WorkspaceCommand
                {
                    Id = $"add_datasource_{typeInfo.TypeId}",
                    Text = typeInfo.DisplayName,
                    IconKey = typeInfo.IconKey,
                    Execute = async (a) => await AddDatasourceAsync(artifact, typeInfo.TypeId)
                });
            }

            if (addDatasourceSubCommands.Any())
            {
                commands.Add(new WorkspaceCommand
                {
                    Id = "add_datasource",
                    Text = "Add Datasource",
                    IconKey = "plus",
                    SubCommands = addDatasourceSubCommands
                });

                commands.Add(WorkspaceCommand.Separator);
            }

            // Workspace commands
            commands.Add(new WorkspaceCommand
            {
                Id = "save_workspace",
                Text = "Save Workspace",
                IconKey = "save",
                Execute = async (a) => await WorkspaceController.SaveWorkspaceAsync()
            });

            commands.Add(new WorkspaceCommand
            {
                Id = "workspace_properties",
                Text = "Properties",
                IconKey = "settings",
                Execute = async (a) => await ShowPropertiesAsync(artifact)
            });

            return commands;
        }

        //protected override object? CreateDetailViewInternal(WorkspaceArtifact artifact)
        //{
        //    // Return the edit view model - the view will be shown by OnSelectedInternalAsync
        //    return null;
        //}

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
                WorkspaceController.OnArtifactPropertyChanged(artifact, e.PropertyName, e.NewValue);
            }
        }

        private async Task AddDatasourceAsync(WorkspaceArtifact workspace, string typeId)
        {
            // TODO: Show dialog to configure the datasource
            // For now, create with default name
            var datasource = WorkspaceController.AddDatasource(typeId, $"New {typeId} Datasource");
            if (datasource != null)
            {
                await WorkspaceController.SaveWorkspaceAsync();
            }
        }

        private Task ShowPropertiesAsync(WorkspaceArtifact workspace)
        {
            // Show the edit view
            EnsureEditViewModel(workspace);
            WorkspaceController.ShowWorkspaceDetailsView(_editViewModel!);

            return Task.CompletedTask;
        }
    }
}
