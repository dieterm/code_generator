using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Application.ViewModels.Workspace.Domains;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Domains
{
    /// <summary>
    /// Controller for PropertyArtifact
    /// </summary>
    public class PropertyController : ArtifactControllerBase<WorkspaceTreeViewController, PropertyArtifact>
    {
        private PropertyEditViewModel? _editViewModel;

        public PropertyController(
            WorkspaceTreeViewController workspaceController,
            ILogger<PropertyController> logger)
            : base(workspaceController, logger)
        {
        }

        protected override void OnArtifactRenamedInternal(PropertyArtifact artifact, string oldName, string newName)
        {
            artifact.Name = newName;
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(PropertyArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Rename command
            commands.Add(new ArtifactTreeNodeCommand
            {
                Id = "rename_property",
                Text = "Rename",
                IconKey = "edit",
                Execute = async (a) =>
                {
                    TreeViewController.RequestBeginRename(artifact);
                    await Task.CompletedTask;
                }
            });

            commands.Add(ArtifactTreeNodeCommand.Separator);

            // Delete command
            commands.Add(new ArtifactTreeNodeCommand
            {
                Id = "delete_property",
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

            commands.Add(ArtifactTreeNodeCommand.Separator);

            // Properties command
            commands.Add(new ArtifactTreeNodeCommand
            {
                Id = "property_properties",
                Text = "Properties",
                IconKey = "settings",
                Execute = async (a) => await ShowPropertiesAsync(artifact)
            });

            return commands;
        }

        protected override Task OnSelectedInternalAsync(PropertyArtifact artifact, CancellationToken cancellationToken)
        {
            return ShowPropertiesAsync(artifact);
        }

        private void EnsureEditViewModel(PropertyArtifact artifact)
        {
            if (_editViewModel == null)
            {
                _editViewModel = new PropertyEditViewModel();
                _editViewModel.ValueChanged += OnEditViewModelValueChanged;
            }

            _editViewModel.Property = artifact;
        }

        private void OnEditViewModelValueChanged(object? sender, ArtifactPropertyChangedEventArgs e)
        {
            TreeViewController.OnArtifactPropertyChanged(e.Artifact, e.PropertyName, e.NewValue);
        }

        private Task ShowPropertiesAsync(PropertyArtifact property)
        {
            EnsureEditViewModel(property);
            TreeViewController.ShowArtifactDetailsView(_editViewModel!);
            return Task.CompletedTask;
        }
    }
}
