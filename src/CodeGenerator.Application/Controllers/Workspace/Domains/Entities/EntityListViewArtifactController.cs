using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.ViewModels.Workspace.Domains;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Domains.Entities
{
    /// <summary>
    /// Controller for EntityListViewArtifact
    /// </summary>
    public class EntityListViewArtifactController : WorkspaceArtifactControllerBase<EntityListViewArtifact>
    {
        private EntityListViewEditViewModel? _editViewModel;

        public EntityListViewArtifactController(OperationExecutor operationExecutor,
            WorkspaceTreeViewController workspaceController,
            ILogger<EntityListViewArtifactController> logger)
            : base(operationExecutor, workspaceController, logger)
        {
        }

        protected override void OnArtifactRenamedInternal(EntityListViewArtifact artifact, string oldName, string newName)
        {
            artifact.Name = newName;
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(EntityListViewArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Get parent entity for property/relation selection
            var entity = artifact.FindAncesterOfType<EntityArtifact>();

            // Add Column command with property selection
            var addColumnCommand = new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "add_column",
                Text = "Add Column",
                IconKey = "plus",
                SubCommands = new List<ArtifactTreeNodeCommand>()
            };

            if (entity != null)
            {
                // Add properties from DefaultState
                var defaultState = entity.DefaultState;
                if (defaultState != null)
                {
                    foreach (var property in defaultState.Properties)
                    {
                        var propertyPath = property.Name;
                        // Check if column already exists for this property
                        var existingColumn = artifact.GetColumns().FirstOrDefault(c => c.PropertyPath == propertyPath);
                        if (existingColumn == null)
                        {
                            addColumnCommand.SubCommands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
                            {
                                Id = $"add_column_{propertyPath}",
                                Text = propertyPath,
                                IconKey = "columns-3",
                                Execute = async (a) =>
                                {
                                    var column = new EntityListViewColumnArtifact(propertyPath)
                                    {
                                        HeaderText = propertyPath,
                                        ColumnType = ListViewColumnType.Text,
                                        IsVisible = true,
                                        IsSortable = true,
                                        IsFilterable = true,
                                        DisplayOrder = artifact.GetColumns().Count()
                                    };
                                    artifact.AddColumn(column);
                                    TreeViewController.OnArtifactAdded(artifact, column);
                                    await Task.CompletedTask;
                                }
                            });
                        }
                    }
                }



                // Add relation properties
                foreach (var relation in entity.GetRelations())
                {
                    var relationPropertyPath = $"{relation.SourcePropertyName}";
                    addColumnCommand.SubCommands.Add(new ArtifactTreeNodeCommand("Relation")
                    {
                        Id = $"add_column_rel_{relation.Id}",
                        Text = $"[Relation] {relationPropertyPath}",
                        IconKey = "link",
                        Execute = async (a) =>
                        {
                            var column = new EntityListViewColumnArtifact(relationPropertyPath)
                            {
                                HeaderText = relationPropertyPath,
                                ColumnType = ListViewColumnType.Text,
                                IsVisible = true,
                                IsSortable = true,
                                IsFilterable = true,
                                DisplayOrder = artifact.GetColumns().Count()
                            };
                            artifact.AddColumn(column);
                            TreeViewController.OnArtifactAdded(artifact, column);
                            await Task.CompletedTask;
                        }
                    });
                }
            }

            if (addColumnCommand.SubCommands.Count == 0)
            {
                addColumnCommand.SubCommands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
                {
                    Id = "no_properties",
                    Text = "(No available properties - set DefaultState first)",
                    IconKey = "alert-circle"
                });
            }

            commands.Add(addColumnCommand);

            // Add all columns command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "add_all_columns",
                Text = "Add All Columns",
                IconKey = "layers",
                Execute = async (a) =>
                {
                    var ent = artifact.FindAncesterOfType<EntityArtifact>();
                    if (ent?.DefaultState != null)
                    {
                        int order = artifact.GetColumns().Count();
                        foreach (var property in ent.DefaultState.Properties)
                        {
                            var existingColumn = artifact.GetColumns().FirstOrDefault(c => c.PropertyPath == property.Name);
                            if (existingColumn == null)
                            {
                                var column = new EntityListViewColumnArtifact(property.Name)
                                {
                                    HeaderText = property.Name,
                                    ColumnType = ListViewColumnType.Text,
                                    IsVisible = true,
                                    IsSortable = true,
                                    IsFilterable = true,
                                    DisplayOrder = order++
                                };
                                artifact.AddColumn(column);
                                TreeViewController.OnArtifactAdded(artifact, column);
                            }
                        }
                    }
                    await Task.CompletedTask;
                }
            });

            // Rename command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_RENAME)
            {
                Id = "rename_listview",
                Text = "Rename",
                IconKey = "edit",
                Execute = async (a) =>
                {
                    TreeViewController.RequestBeginRename(artifact);
                    await Task.CompletedTask;
                }
            });


            // Properties command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "listview_properties",
                Text = "Properties",
                IconKey = "settings",
                Execute = async (a) => await ShowPropertiesAsync(artifact)
            });

            return commands;
        }

        #region Clipboard Operations

        public override bool CanDelete(EntityListViewArtifact artifact)
        {
            return artifact.Parent != null;
        }

        public override void Delete(EntityListViewArtifact artifact)
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

        protected override Task OnSelectedInternalAsync(EntityListViewArtifact artifact, CancellationToken cancellationToken)
        {
            return ShowPropertiesAsync(artifact);
        }

        private void EnsureEditViewModel(EntityListViewArtifact artifact)
        {
            if (_editViewModel == null)
            {
                _editViewModel = new EntityListViewEditViewModel();
                _editViewModel.ValueChanged += OnEditViewModelValueChanged;
            }

            _editViewModel.ListView = artifact;
        }

        private void OnEditViewModelValueChanged(object? sender, ArtifactPropertyChangedEventArgs e)
        {
            TreeViewController.OnArtifactPropertyChanged(e.Artifact, e.PropertyName, e.NewValue);
        }

        private Task ShowPropertiesAsync(EntityListViewArtifact listView)
        {
            EnsureEditViewModel(listView);
            TreeViewController.ShowArtifactDetailsView(_editViewModel!);
            return Task.CompletedTask;
        }
    }
}
