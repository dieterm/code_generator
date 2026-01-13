using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace
{
    /// <summary>
    /// Controller for ViewArtifact
    /// </summary>
    public class ViewArtifactController : ArtifactControllerBase<ViewArtifact>
    {
        public ViewArtifactController(
            WorkspaceController workspaceController,
            ILogger<ViewArtifactController> logger)
            : base(workspaceController, logger)
        {
        }

        protected override IEnumerable<WorkspaceCommand> GetCommands(ViewArtifact artifact)
        {
            var commands = new List<WorkspaceCommand>();

            // Add Column command
            commands.Add(new WorkspaceCommand
            {
                Id = "add_column",
                Text = "Add Column",
                IconKey = "columns",
                Execute = async (a) =>
                {
                    var column = new ColumnArtifact("NewColumn", "varchar(255)", true);
                    artifact.AddChild(column);
                    WorkspaceController.OnArtifactAdded(artifact, column);
                    WorkspaceController.RequestBeginRename(column);
                    await Task.CompletedTask;
                }
            });

            // Add Index command (views can have indexes in some databases)
            commands.Add(new WorkspaceCommand
            {
                Id = "add_index",
                Text = "Add Index",
                IconKey = "list",
                Execute = async (a) =>
                {
                    var index = new IndexArtifact("IX_NewIndex", false);
                    artifact.AddChild(index);
                    WorkspaceController.OnArtifactAdded(artifact, index);
                    WorkspaceController.RequestBeginRename(index);
                    await Task.CompletedTask;
                }
            });

            commands.Add(WorkspaceCommand.Separator);

            // Rename command
            commands.Add(new WorkspaceCommand
            {
                Id = "rename_view",
                Text = "Rename",
                IconKey = "edit",
                Execute = async (a) =>
                {
                    WorkspaceController.RequestBeginRename(artifact);
                    await Task.CompletedTask;
                }
            });

            commands.Add(WorkspaceCommand.Separator);

            // Convert to Table command
            commands.Add(new WorkspaceCommand
            {
                Id = "convert_to_table",
                Text = "Convert to Table",
                IconKey = "table",
                Execute = async (a) =>
                {
                    await ConvertToTableAsync(artifact);
                }
            });

            commands.Add(WorkspaceCommand.Separator);

            // Delete command
            commands.Add(new WorkspaceCommand
            {
                Id = "delete_view",
                Text = "Delete",
                IconKey = "trash",
                Execute = async (a) =>
                {
                    var parent = artifact.Parent;
                    if (parent != null)
                    {
                        parent.RemoveChild(artifact);
                        WorkspaceController.OnArtifactRemoved(parent, artifact);
                    }
                    await Task.CompletedTask;
                }
            });

            return commands;
        }

        protected override Task OnSelectedInternalAsync(ViewArtifact artifact, CancellationToken cancellationToken)
        {
            // Views don't have a specific edit view, just show in tree
            WorkspaceController.ShowWorkspaceDetailsView(null);
            return Task.CompletedTask;
        }

        private async Task ConvertToTableAsync(ViewArtifact view)
        {
            var parent = view.Parent;
            if (parent == null) return;

            // Create a new table with the same properties
            var table = new TableArtifact(view.Name, view.Schema);

            // Copy columns
            foreach (var column in view.GetColumns().ToList())
            {
                view.RemoveChild(column);
                table.AddChild(column);
            }

            // Remove the view
            parent.RemoveChild(view);
            WorkspaceController.OnArtifactRemoved(parent, view);

            // Add the table
            parent.AddChild(table);
            WorkspaceController.OnArtifactAdded(parent, table);

            await Task.CompletedTask;
        }
    }
}
