using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace
{
    /// <summary>
    /// Controller for ViewArtifact
    /// </summary>
    public class ViewArtifactController : ArtifactControllerBase<WorkspaceTreeViewController, ViewArtifact>
    {
        //protected WorkspaceTreeViewController TreeViewController => (WorkspaceTreeViewController)base.TreeViewController;

        public ViewArtifactController(
            WorkspaceTreeViewController workspaceController,
            ILogger<ViewArtifactController> logger)
            : base(workspaceController, logger)
        {
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(ViewArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Add Column command
            commands.Add(new ArtifactTreeNodeCommand
            {
                Id = "add_column",
                Text = "Add Column",
                IconKey = "columns",
                Execute = async (a) =>
                {
                    var column = new ColumnArtifact("NewColumn", "varchar(255)", true);
                    artifact.AddChild(column);
                    TreeViewController.OnArtifactAdded(artifact, column);
                    TreeViewController.RequestBeginRename(column);
                    await Task.CompletedTask;
                }
            });

            // Add Index command
            commands.Add(new ArtifactTreeNodeCommand
            {
                Id = "add_index",
                Text = "Add Index",
                IconKey = "list",
                Execute = async (a) =>
                {
                    var index = new IndexArtifact("IX_NewIndex", false);
                    artifact.AddChild(index);
                    TreeViewController.OnArtifactAdded(artifact, index);
                    TreeViewController.RequestBeginRename(index);
                    await Task.CompletedTask;
                }
            });

            commands.Add(ArtifactTreeNodeCommand.Separator);

            // Rename command
            commands.Add(new ArtifactTreeNodeCommand
            {
                Id = "rename_view",
                Text = "Rename",
                IconKey = "edit",
                Execute = async (a) =>
                {
                    TreeViewController.RequestBeginRename(artifact);
                    await Task.CompletedTask;
                }
            });

            commands.Add(ArtifactTreeNodeCommand.Separator);

            // Convert to Table command
            commands.Add(new ArtifactTreeNodeCommand
            {
                Id = "convert_to_table",
                Text = "Convert to Table",
                IconKey = "table",
                Execute = async (a) =>
                {
                    await ConvertToTableAsync(artifact);
                }
            });

            commands.Add(ArtifactTreeNodeCommand.Separator);

            // Delete command
            commands.Add(new ArtifactTreeNodeCommand
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
                        TreeViewController.OnArtifactRemoved(parent, artifact);
                    }
                    await Task.CompletedTask;
                }
            });

            return commands;
        }

        protected override Task OnSelectedInternalAsync(ViewArtifact artifact, CancellationToken cancellationToken)
        {
            TreeViewController.ShowArtifactDetailsView(null);
            return Task.CompletedTask;
        }

        private async Task ConvertToTableAsync(ViewArtifact view)
        {
            var parent = view.Parent;
            if (parent == null) return;

            var table = new TableArtifact(view.Name, view.Schema);

            foreach (var column in view.GetColumns().ToList())
            {
                view.RemoveChild(column);
                table.AddChild(column);
            }

            parent.RemoveChild(view);
            TreeViewController.OnArtifactRemoved(parent, view);

            parent.AddChild(table);
            TreeViewController.OnArtifactAdded(parent, table);

            await Task.CompletedTask;
        }
    }
}
