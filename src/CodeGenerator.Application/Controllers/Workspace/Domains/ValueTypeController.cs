using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.ViewModels.Workspace.Domains;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Domain.DataTypes;
using CodeGenerator.Domain.NamingConventions;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Domains
{
    /// <summary>
    /// Controller for ValueTypeArtifact
    /// </summary>
    public class ValueTypeController : WorkspaceArtifactControllerBase<ValueTypeArtifact>
    {
        private ValueTypeEditViewModel? _editViewModel;

        public ValueTypeController(
            WorkspaceTreeViewController workspaceController,
            ILogger<ValueTypeController> logger)
            : base(workspaceController, logger)
        {
        }

        protected override void OnArtifactRenamedInternal(ValueTypeArtifact artifact, string oldName, string newName)
        {
            artifact.Name = newName;
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(ValueTypeArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Add Property command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "add_property",
                Text = "Add Property",
                IconKey = "plus",
                Execute = async (a) =>
                {
                    var property = new PropertyArtifact("NewProperty", GenericDataTypes.VarChar.Id, true);
                    artifact.AddProperty(property);
                    TreeViewController.OnArtifactAdded(artifact, property);
                    TreeViewController.RequestBeginRename(property);
                    await Task.CompletedTask;
                }
            });

            // Rename command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_RENAME)
            {
                Id = "rename_valuetype",
                Text = "Rename",
                IconKey = "edit",
                Execute = async (a) =>
                {
                    TreeViewController.RequestBeginRename(artifact);
                    await Task.CompletedTask;
                }
            });

            // Naming Convention Commands
            var namingConventionCommand = new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_RENAME)
            {
                Id = "properties_naming_convention",
                Text = "Properties Naming Convention",
                IconKey = "edit",
                SubCommands = new List<ArtifactTreeNodeCommand>()
            };

            foreach (var style in Enum.GetValues<NamingStyle>())
            {
                namingConventionCommand.SubCommands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
                {
                    Id = $"properties_to_{style.ToString().ToLower()}_property",
                    Text = $"To {style}",
                    IconKey = "edit",
                    Execute = async (a) =>
                    {
                        foreach (var property in artifact.Properties)
                        {
                            property.Name = NamingConventions.Convert(property.Name, style);
                        }
                        await Task.CompletedTask;
                    }
                });
            }

            commands.Add(namingConventionCommand);

            // Properties command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "valuetype_properties",
                Text = "Properties",
                IconKey = "settings",
                Execute = async (a) => await ShowPropertiesAsync(artifact)
            });

            return commands;
        }

        #region Clipboard Operations

        public override bool CanDelete(ValueTypeArtifact artifact)
        {
            return artifact.Parent != null;
        }

        public override void Delete(ValueTypeArtifact artifact)
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

        protected override Task OnSelectedInternalAsync(ValueTypeArtifact artifact, CancellationToken cancellationToken)
        {
            return ShowPropertiesAsync(artifact);
        }

        private void EnsureEditViewModel(ValueTypeArtifact artifact)
        {
            if (_editViewModel == null)
            {
                _editViewModel = new ValueTypeEditViewModel();
                _editViewModel.ValueChanged += OnEditViewModelValueChanged;
            }

            _editViewModel.ValueType = artifact;
        }

        private void OnEditViewModelValueChanged(object? sender, ArtifactPropertyChangedEventArgs e)
        {
            TreeViewController.OnArtifactPropertyChanged(e.Artifact, e.PropertyName, e.NewValue);
        }

        private Task ShowPropertiesAsync(ValueTypeArtifact valueType)
        {
            EnsureEditViewModel(valueType);
            TreeViewController.ShowArtifactDetailsView(_editViewModel!);
            return Task.CompletedTask;
        }
    }
}
