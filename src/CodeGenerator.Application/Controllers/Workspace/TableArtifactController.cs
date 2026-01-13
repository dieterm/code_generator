using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace
{
    /// <summary>
    /// Controller for TableArtifact
    /// </summary>
    public class TableArtifactController : ArtifactControllerBase<TableArtifact>
    {
        public TableArtifactController(
            WorkspaceController workspaceController,
            ILogger<TableArtifactController> logger)
            : base(workspaceController, logger)
        {
        }

        protected override IEnumerable<WorkspaceCommand> GetCommands(TableArtifact artifact)
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

            // Add Index command
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
                Id = "rename_table",
                Text = "Rename",
                IconKey = "edit",
                Execute = async (a) =>
                {
                    WorkspaceController.RequestBeginRename(artifact);
                    await Task.CompletedTask;
                }
            });

            commands.Add(WorkspaceCommand.Separator);

            // Convert to View command
            commands.Add(new WorkspaceCommand
            {
                Id = "convert_to_view",
                Text = "Convert to View",
                IconKey = "eye",
                Execute = async (a) =>
                {
                    await ConvertToViewAsync(artifact);
                }
            });

            commands.Add(WorkspaceCommand.Separator);

            // Delete command
            commands.Add(new WorkspaceCommand
            {
                Id = "delete_table",
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

        protected override Task OnSelectedInternalAsync(TableArtifact artifact, CancellationToken cancellationToken)
        {
            // Tables don't have a specific edit view, just show in tree
            WorkspaceController.ShowWorkspaceDetailsView(null);
            return Task.CompletedTask;
        }

        private async Task ConvertToViewAsync(TableArtifact table)
        {
            var parent = table.Parent;
            if (parent == null) return;

            // Create a new view with the same properties
            var view = new ViewArtifact(table.Name, table.Schema);

            // Copy columns
            foreach (var column in table.GetColumns().ToList())
            {
                table.RemoveChild(column);
                view.AddChild(column);
            }

            // Remove the table
            parent.RemoveChild(table);
            WorkspaceController.OnArtifactRemoved(parent, table);

            // Add the view
            parent.AddChild(view);
            WorkspaceController.OnArtifactAdded(parent, view);

            await Task.CompletedTask;
        }
    }
}
