using CodeGenerator.Application.ViewModels;
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
    public class ColumnArtifactController : ArtifactControllerBase<ColumnArtifact>
    {
        private readonly IDatasourceFactory _datasourceFactory;
        private ColumnEditViewModel? _editViewModel;

        public ColumnArtifactController(
            WorkspaceController workspaceController,
            IDatasourceFactory datasourceFactory,
            ILogger<ColumnArtifactController> logger)
            : base(workspaceController, logger)
        {
            _datasourceFactory = datasourceFactory;
        }

        protected override IEnumerable<WorkspaceCommand> GetCommands(ColumnArtifact artifact)
        {
            var commands = new List<WorkspaceCommand>();

            // Toggle Primary Key command
            commands.Add(new WorkspaceCommand
            {
                Id = "toggle_primary_key",
                Text = artifact.IsPrimaryKey ? "Remove Primary Key" : "Set as Primary Key",
                IconKey = "key",
                Execute = async (a) =>
                {
                    artifact.IsPrimaryKey = !artifact.IsPrimaryKey;
                    WorkspaceController.OnArtifactPropertyChanged(artifact, nameof(ColumnArtifact.IsPrimaryKey), artifact.IsPrimaryKey);
                    await Task.CompletedTask;
                }
            });

            commands.Add(WorkspaceCommand.Separator);

            // Rename command
            commands.Add(new WorkspaceCommand
            {
                Id = "rename_column",
                Text = "Rename",
                IconKey = "edit",
                Execute = async (a) =>
                {
                    WorkspaceController.RequestBeginRename(artifact);
                    await Task.CompletedTask;
                }
            });

            commands.Add(WorkspaceCommand.Separator);

            // Delete command
            commands.Add(new WorkspaceCommand
            {
                Id = "delete_column",
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

        protected override Task OnSelectedInternalAsync(ColumnArtifact artifact, CancellationToken cancellationToken)
        {
            EnsureEditViewModel(artifact);
            
            // Populate data types from datasource
            PopulateDataTypes(artifact);

            WorkspaceController.ShowWorkspaceDetailsView(_editViewModel!);
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
                        UseScale = m.SupportsScale
                    }).ToList();

                    _editViewModel.SetAvailableDataTypes(items);
                }
            }
        }

        private void OnEditViewModelValueChanged(object? sender, ArtifactPropertyChangedEventArgs e)
        {
            WorkspaceController.OnArtifactPropertyChanged(e.Artifact, e.PropertyName, e.NewValue);
        }
    }
}
