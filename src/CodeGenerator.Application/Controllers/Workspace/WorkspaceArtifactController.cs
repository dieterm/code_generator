using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Workspace;
using CodeGenerator.Shared.ExtensionMethods;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;
using WorkspaceArtifactEditViewModel = CodeGenerator.Core.Workspaces.ViewModels.Workspace.WorkspaceArtifactEditViewModel;

namespace CodeGenerator.Application.Controllers.Workspace
{
    /// <summary>
    /// Controller for WorkspaceArtifact
    /// Handles context menus and detail views for the workspace root node
    /// </summary>
    public class WorkspaceArtifactController : WorkspaceArtifactControllerBase<WorkspaceArtifact, WorkspaceArtifactEditViewModel>
    {
        //private readonly IDatasourceFactory _datasourceFactory;
        public WorkspaceArtifactController(
            OperationExecutor operationExecutor,
            //IDatasourceFactory datasourceFactory, 
            WorkspaceTreeViewController workspaceController,
            ILogger<WorkspaceArtifactController> logger
            ): base(operationExecutor, workspaceController, logger)
        {
            //_datasourceFactory = datasourceFactory;
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

        //private async Task AddDatasourceAsync(WorkspaceArtifact workspace, string typeId)
        //{
        //    var datasource = TreeViewController.AddDatasource(typeId, $"New {typeId} Datasource");
        //    if (datasource != null)
        //    {
        //        await TreeViewController.SaveWorkspaceAsync();
        //    }
        //}

    }
}
