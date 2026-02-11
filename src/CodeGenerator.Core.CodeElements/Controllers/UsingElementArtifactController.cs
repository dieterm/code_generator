using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Core.CodeElements.ViewModels;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.CodeElements.Controllers
{
    public class UsingElementArtifactController : CodeElementArtifactControllerBase<UsingElementArtifact>
    {
        private UsingElementEditViewModel? _editViewModel;

        public UsingElementArtifactController(OperationExecutor operationExecutor, CodeElementsTreeViewController treeViewController, ILogger<UsingElementArtifactController> logger) 
            : base(operationExecutor, treeViewController, logger)
        {
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(UsingElementArtifact artifact)
        {
            yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE) {
                Id = "using_properties",
                Text = "Properties",
                Execute = async (a) => await ShowPropertiesAsync(artifact)
            };
            yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE) {
                Id = "delete_using",
                Text = "Delete using",
                Execute = async (a) => {
                    (artifact.Parent as IUsingsContainerArtifact)?.RemoveUsing(artifact);
                }
            };
        }

        protected override Task OnSelectedInternalAsync(UsingElementArtifact artifact, CancellationToken cancellationToken)
        {
            return ShowPropertiesAsync(artifact);
        }

        private Task ShowPropertiesAsync(UsingElementArtifact artifact)
        {
            if (_editViewModel == null)
            {
                _editViewModel = new UsingElementEditViewModel();
                _editViewModel.ValueChanged += OnEditViewModelValueChanged;
            }

            _editViewModel.Artifact = artifact;
            TreeViewController.ShowArtifactDetailsView(_editViewModel);
            return Task.CompletedTask;
        }

        private void OnEditViewModelValueChanged(object? sender, ArtifactPropertyChangedEventArgs e)
        {
            TreeViewController.OnArtifactPropertyChanged(e.Artifact, e.PropertyName, e.NewValue);
        }
    }
}
