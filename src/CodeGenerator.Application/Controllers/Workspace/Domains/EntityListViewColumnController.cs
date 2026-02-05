using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.ViewModels.Workspace.Domains;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Domains
{
    /// <summary>
    /// Controller for EntityListViewColumnArtifact
    /// </summary>
    public class EntityListViewColumnController : WorkspaceArtifactControllerBase<WorkspaceTreeViewController, EntityListViewColumnArtifact>
    {
        private EntityListViewColumnEditViewModel? _editViewModel;

        public EntityListViewColumnController(
            WorkspaceTreeViewController workspaceController,
            ILogger<EntityListViewColumnController> logger)
            : base(workspaceController, logger)
        {
        }

        protected override void OnArtifactRenamedInternal(EntityListViewColumnArtifact artifact, string oldName, string newName)
        {
            artifact.PropertyPath = newName;
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(EntityListViewColumnArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Column Type submenu
            var columnTypeCommand = new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "set_column_type",
                Text = "Set Column Type",
                IconKey = "columns-3",
                SubCommands = new List<ArtifactTreeNodeCommand>()
            };

            foreach (var columnType in Enum.GetValues<ListViewColumnType>())
            {
                var ct = columnType;
                columnTypeCommand.SubCommands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
                {
                    Id = $"set_column_{ct}",
                    Text = ct.ToString(),
                    IconKey = GetIconForColumnType(ct),
                    Execute = async (a) =>
                    {
                        artifact.ColumnType = ct;
                        await Task.CompletedTask;
                    }
                });
            }

            commands.Add(columnTypeCommand);
            var LISTVIEW_ACTIONS = "ListViewActions";

            // Toggle visibility
            commands.Add(new ArtifactTreeNodeCommand(LISTVIEW_ACTIONS)
            {
                Id = "toggle_visibility",
                Text = artifact.IsVisible ? "Hide Column" : "Show Column",
                IconKey = artifact.IsVisible ? "eye-off" : "eye",
                Execute = async (a) =>
                {
                    artifact.IsVisible = !artifact.IsVisible;
                    await Task.CompletedTask;
                }
            });

            // Toggle sortable
            commands.Add(new ArtifactTreeNodeCommand(LISTVIEW_ACTIONS)
            {
                Id = "toggle_sortable",
                Text = artifact.IsSortable ? "Disable Sorting" : "Enable Sorting",
                IconKey = "arrow-up-down",
                Execute = async (a) =>
                {
                    artifact.IsSortable = !artifact.IsSortable;
                    await Task.CompletedTask;
                }
            });

            // Toggle filterable
            commands.Add(new ArtifactTreeNodeCommand(LISTVIEW_ACTIONS)
            {
                Id = "toggle_filterable",
                Text = artifact.IsFilterable ? "Disable Filter" : "Enable Filter",
                IconKey = "filter",
                Execute = async (a) =>
                {
                    artifact.IsFilterable = !artifact.IsFilterable;
                    await Task.CompletedTask;
                }
            });

            var MOVE_ACTIONS = "Move";

            // Move Left command
            commands.Add(new ArtifactTreeNodeCommand(MOVE_ACTIONS)
            {
                Id = "move_left",
                Text = "Move Left",
                IconKey = "arrow-left",
                Execute = async (a) =>
                {
                    var columns = artifact.Parent?.Children.OfType<EntityListViewColumnArtifact>()
                        .OrderBy(c => c.DisplayOrder).ToList();
                    if (columns != null)
                    {
                        var index = columns.IndexOf(artifact);
                        if (index > 0)
                        {
                            var prevColumn = columns[index - 1];
                            var tempOrder = artifact.DisplayOrder;
                            artifact.DisplayOrder = prevColumn.DisplayOrder;
                            prevColumn.DisplayOrder = tempOrder;
                        }
                    }
                    await Task.CompletedTask;
                }
            });

            // Move Right command
            commands.Add(new ArtifactTreeNodeCommand(MOVE_ACTIONS)
            {
                Id = "move_right",
                Text = "Move Right",
                IconKey = "arrow-right",
                Execute = async (a) =>
                {
                    var columns = artifact.Parent?.Children.OfType<EntityListViewColumnArtifact>()
                        .OrderBy(c => c.DisplayOrder).ToList();
                    if (columns != null)
                    {
                        var index = columns.IndexOf(artifact);
                        if (index < columns.Count - 1)
                        {
                            var nextColumn = columns[index + 1];
                            var tempOrder = artifact.DisplayOrder;
                            artifact.DisplayOrder = nextColumn.DisplayOrder;
                            nextColumn.DisplayOrder = tempOrder;
                        }
                    }
                    await Task.CompletedTask;
                }
            });

             // Properties command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "column_properties",
                Text = "Properties",
                IconKey = "settings",
                Execute = async (a) => await ShowPropertiesAsync(artifact)
            });

            return commands;
        }

        private string GetIconForColumnType(ListViewColumnType columnType)
        {
            return columnType switch
            {
                ListViewColumnType.Text => "type",
                ListViewColumnType.Numeric => "hash",
                ListViewColumnType.Date => "calendar",
                ListViewColumnType.DateTime => "calendar-clock",
                ListViewColumnType.Boolean => "toggle-left",
                ListViewColumnType.Image => "image",
                ListViewColumnType.Hyperlink => "link",
                ListViewColumnType.Combobox => "chevrons-up-down",
                ListViewColumnType.Template => "code",
                _ => "columns-3"
            };
        }

        #region Clipboard Operations

        public override bool CanDelete(EntityListViewColumnArtifact artifact)
        {
            return artifact.Parent != null;
        }

        public override void Delete(EntityListViewColumnArtifact artifact)
        {
            if (!CanDelete(artifact)) return;

            var parent = artifact.Parent;
            if (parent != null)
            {
                parent.RemoveChild(artifact);
                TreeViewController.OnArtifactRemoved(parent, artifact);
            }
        }

        #endregion

        protected override Task OnSelectedInternalAsync(EntityListViewColumnArtifact artifact, CancellationToken cancellationToken)
        {
            return ShowPropertiesAsync(artifact);
        }

        private void EnsureEditViewModel(EntityListViewColumnArtifact artifact)
        {
            if (_editViewModel == null)
            {
                _editViewModel = new EntityListViewColumnEditViewModel();
                _editViewModel.ValueChanged += OnEditViewModelValueChanged;
            }

            _editViewModel.Column = artifact;
        }

        private void OnEditViewModelValueChanged(object? sender, ArtifactPropertyChangedEventArgs e)
        {
            TreeViewController.OnArtifactPropertyChanged(e.Artifact, e.PropertyName, e.NewValue);
        }

        private Task ShowPropertiesAsync(EntityListViewColumnArtifact column)
        {
            EnsureEditViewModel(column);
            TreeViewController.ShowArtifactDetailsView(_editViewModel!);
            return Task.CompletedTask;
        }
    }
}
