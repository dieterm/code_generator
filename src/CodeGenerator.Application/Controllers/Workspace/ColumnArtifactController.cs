using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Core.Workspaces.Models;
using CodeGenerator.Core.Workspaces.Services;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace
{
    /// <summary>
    /// Controller for ColumnArtifact
    /// </summary>
    public class ColumnArtifactController : ArtifactControllerBase<WorkspaceTreeViewController, ColumnArtifact>
    {
        private readonly IDatasourceFactory _datasourceFactory;
        private ColumnEditViewModel? _editViewModel;

        public ColumnArtifactController(
            WorkspaceTreeViewController workspaceController,
            IDatasourceFactory datasourceFactory,
            ILogger<ColumnArtifactController> logger)
            : base(workspaceController, logger)
        {
            _datasourceFactory = datasourceFactory;
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(ColumnArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Toggle Primary Key command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "toggle_primary_key",
                Text = artifact.IsPrimaryKey ? "Remove Primary Key" : "Set as Primary Key",
                IconKey = "key",
                Execute = async (a) =>
                {
                    artifact.IsPrimaryKey = !artifact.IsPrimaryKey;
                    TreeViewController.OnArtifactPropertyChanged(artifact, nameof(ColumnArtifact.IsPrimaryKey), artifact.IsPrimaryKey);
                    await Task.CompletedTask;
                }
            });


            // Rename command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_RENAME)
            {
                Id = "rename_column",
                Text = "Rename",
                IconKey = "edit",
                Execute = async (a) =>
                {
                    TreeViewController.RequestBeginRename(artifact);
                    await Task.CompletedTask;
                }
            });

            // Note: Delete command is now added automatically by base class via GetClipboardCommands()

            return commands;
        }

        #region Clipboard Operations

        public override bool CanDelete(ColumnArtifact artifact)
        {
            return artifact.Parent != null;
        }

        public override void Delete(ColumnArtifact artifact)
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

        protected override Task OnSelectedInternalAsync(ColumnArtifact artifact, CancellationToken cancellationToken)
        {
            EnsureEditViewModel(artifact);
            PopulateDataTypes(artifact);
            TreeViewController.ShowArtifactDetailsView(_editViewModel!);
            return Task.CompletedTask;
        }

        private void EnsureEditViewModel(ColumnArtifact artifact)
        {
            if (_editViewModel == null)
            {
                _editViewModel = new ColumnEditViewModel();
                _editViewModel.ValueChanged += OnEditViewModelValueChanged;
            }

            _editViewModel.Column = artifact;
        }
        
        private void PopulateDataTypes(ColumnArtifact column)
        {
            if (_editViewModel == null) return;

            // Find parent datasource
            IArtifact? current = column.Parent;
            while (current != null && !(current is DatasourceArtifact))
            {
                current = current.Parent;
            }

            if (current is DatasourceArtifact datasourceArtifact)
            {
                var provider = _datasourceFactory.GetProvider(datasourceArtifact.DatasourceType);
                if (provider != null)
                {
                    var mappings = provider.GetSupportedColumnDataTypes();
                    var items = mappings.Select(m => new DataTypeComboboxItem
                    {
                        DisplayName = m.Name,
                        Value = m.Id,
                        TypeDescription = m.Description,
                        UseMaxLength = m.SupportsMaxLength,
                        UsePrecision = m.SupportsPrecision,
                        UseScale = m.SupportsScale,
                        UseAllowedValues = m.SupportsAllowedValues
                    }).ToList();

                    _editViewModel.SetAvailableDataTypes(items);
                }
            }
        }

        private void OnEditViewModelValueChanged(object? sender, ArtifactPropertyChangedEventArgs e)
        {
            TreeViewController.OnArtifactPropertyChanged(e.Artifact, e.PropertyName, e.NewValue);
        }
    }
}
