using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts.Statements;
using CodeGenerator.Core.CodeElements.ViewModels;
using CodeGenerator.Core.CodeElements.ViewModels.Statements;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.CodeElements.Controllers.Statements
{
    public class AssignmentStatementArtifactController : StatementArtifactControllerBase<AssignmentStatementArtifact>
    {
        private AssignmentStatementEditViewModel? _editViewModel;

        public AssignmentStatementArtifactController(OperationExecutor operationExecutor, CodeElementsTreeViewController treeViewController, ILogger<AssignmentStatementArtifactController> logger) 
            : base(operationExecutor, treeViewController, logger)
        {
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(AssignmentStatementArtifact artifact)
        {
            yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_RENAME)
            {
                Id = "rename_attribute",
                Text = "Rename",
                Execute = async (a) => TreeViewController.RequestBeginRename(artifact)
            };
            yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "attribute_properties",
                Text = "Properties",
                Execute = async (a) => await ShowPropertiesAsync(artifact)
            };
           
        }

        public override bool CanDelete(AssignmentStatementArtifact artifact)
        {
            return artifact.Parent != null; // Can delete if it has a parent to remove from
        }

        public override void Delete(AssignmentStatementArtifact artifact)
        {
            if(artifact.Parent is CompositeStatementArtifact parent)
            {
                parent.Statements.Remove(artifact.StatementElement);
                parent.RemoveChild(artifact);
            } else
            {
                throw new NotImplementedException("TODO: Deletion of AssignmentStatementArtifact is only implemented for those with a CompositeStatementArtifact parent.");
            }
        }

        protected override async Task OnSelectedInternalAsync(AssignmentStatementArtifact artifact, CancellationToken cancellationToken)
        {
            await ShowPropertiesAsync(artifact);
        }

        private Task ShowPropertiesAsync(AssignmentStatementArtifact artifact)
        {
            if (_editViewModel == null)
            {
                _editViewModel = new AssignmentStatementEditViewModel();
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
