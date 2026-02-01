using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.ViewModels.Workspace.Domains;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Domains
{
    /// <summary>
    /// Controller for EntityEditViewFieldArtifact
    /// </summary>
    public class EntityEditViewFieldController : ArtifactControllerBase<WorkspaceTreeViewController, EntityEditViewFieldArtifact>
    {
        private EntityEditViewFieldEditViewModel? _editViewModel;

        public EntityEditViewFieldController(
            WorkspaceTreeViewController workspaceController,
            ILogger<EntityEditViewFieldController> logger)
            : base(workspaceController, logger)
        {
        }

        protected override void OnArtifactRenamedInternal(EntityEditViewFieldArtifact artifact, string oldName, string newName)
        {
            artifact.PropertyName = newName;
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(EntityEditViewFieldArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Control Type submenu
            var controlTypeCommand = new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "set_control_type",
                Text = "Set Control Type",
                IconKey = "text-cursor-input",
                SubCommands = new List<ArtifactTreeNodeCommand>()
            };

            foreach (var controlType in Enum.GetValues<DataFieldControlType>())
            {
                var ct = controlType;
                controlTypeCommand.SubCommands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
                {
                    Id = $"set_control_{ct}",
                    Text = ct.ToString(),
                    IconKey = GetIconForControlType(ct),
                    Execute = async (a) =>
                    {
                        artifact.ControlType = ct;
                        await Task.CompletedTask;
                    }
                });
            }

            commands.Add(controlTypeCommand);
            var UP_DOWN_GROUP = "Move";
            // Move Up command
            commands.Add(new ArtifactTreeNodeCommand(UP_DOWN_GROUP)
            {
                Id = "move_up",
                Text = "Move Up",
                IconKey = "arrow-up",
                Execute = async (a) =>
                {
                    var fields = artifact.Parent?.Children.OfType<EntityEditViewFieldArtifact>()
                        .OrderBy(f => f.DisplayOrder).ToList();
                    if (fields != null)
                    {
                        var index = fields.IndexOf(artifact);
                        if (index > 0)
                        {
                            var prevField = fields[index - 1];
                            var tempOrder = artifact.DisplayOrder;
                            artifact.DisplayOrder = prevField.DisplayOrder;
                            prevField.DisplayOrder = tempOrder;
                        }
                    }
                    await Task.CompletedTask;
                }
            });

            // Move Down command
            commands.Add(new ArtifactTreeNodeCommand(UP_DOWN_GROUP)
            {
                Id = "move_down",
                Text = "Move Down",
                IconKey = "arrow-down",
                Execute = async (a) =>
                {
                    var fields = artifact.Parent?.Children.OfType<EntityEditViewFieldArtifact>()
                        .OrderBy(f => f.DisplayOrder).ToList();
                    if (fields != null)
                    {
                        var index = fields.IndexOf(artifact);
                        if (index < fields.Count - 1)
                        {
                            var nextField = fields[index + 1];
                            var tempOrder = artifact.DisplayOrder;
                            artifact.DisplayOrder = nextField.DisplayOrder;
                            nextField.DisplayOrder = tempOrder;
                        }
                    }
                    await Task.CompletedTask;
                }
            });

            // Properties command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "field_properties",
                Text = "Properties",
                IconKey = "settings",
                Execute = async (a) => await ShowPropertiesAsync(artifact)
            });

            return commands;
        }

        private string GetIconForControlType(DataFieldControlType controlType)
        {
            return controlType switch
            {
                DataFieldControlType.SingleLineTextField => "type",
                DataFieldControlType.MultiLineTextField => "align-left",
                DataFieldControlType.IntegerField => "hash",
                DataFieldControlType.DecimalField => "percent",
                DataFieldControlType.BooleanField => "toggle-left",
                DataFieldControlType.DateField => "calendar",
                DataFieldControlType.DateTimeField => "calendar-clock",
                DataFieldControlType.TimeField => "clock",
                DataFieldControlType.ComboboxField => "chevrons-up-down",
                DataFieldControlType.RadioButtonField => "circle-dot",
                DataFieldControlType.FileField => "file",
                DataFieldControlType.ImageField => "image",
                DataFieldControlType.RichTextField => "file-text",
                DataFieldControlType.ColorField => "palette",
                DataFieldControlType.PasswordField => "key",
                DataFieldControlType.EmailField => "mail",
                DataFieldControlType.PhoneField => "phone",
                DataFieldControlType.UrlField => "link",
                _ => "text-cursor-input"
            };
        }

        #region Clipboard Operations

        public override bool CanDelete(EntityEditViewFieldArtifact artifact)
        {
            return artifact.Parent != null;
        }

        public override void Delete(EntityEditViewFieldArtifact artifact)
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

        protected override Task OnSelectedInternalAsync(EntityEditViewFieldArtifact artifact, CancellationToken cancellationToken)
        {
            return ShowPropertiesAsync(artifact);
        }

        private void EnsureEditViewModel(EntityEditViewFieldArtifact artifact)
        {
            if (_editViewModel == null)
            {
                _editViewModel = new EntityEditViewFieldEditViewModel();
                _editViewModel.ValueChanged += OnEditViewModelValueChanged;
            }

            _editViewModel.Field = artifact;
        }

        private void OnEditViewModelValueChanged(object? sender, ArtifactPropertyChangedEventArgs e)
        {
            TreeViewController.OnArtifactPropertyChanged(e.Artifact, e.PropertyName, e.NewValue);
        }

        private Task ShowPropertiesAsync(EntityEditViewFieldArtifact field)
        {
            EnsureEditViewModel(field);
            TreeViewController.ShowArtifactDetailsView(_editViewModel!);
            return Task.CompletedTask;
        }
    }
}
