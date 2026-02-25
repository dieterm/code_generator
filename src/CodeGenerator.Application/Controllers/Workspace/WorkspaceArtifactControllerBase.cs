using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities;
using CodeGenerator.Core.Workspaces.MessageBus;
using CodeGenerator.Core.Workspaces.ViewModels.Common;
using CodeGenerator.Core.Workspaces.ViewModels.Domain;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Application.Controllers.Workspace
{
    public abstract class WorkspaceArtifactControllerBase<TArtifact> : ArtifactControllerBase<WorkspaceTreeViewController, TArtifact>, IWorkspaceArtifactController
        where TArtifact : WorkspaceArtifactBase
    {
        public WorkspaceArtifactControllerBase(OperationExecutor operationExecutor, WorkspaceTreeViewController treeViewController, ILogger logger)
            : base(operationExecutor, treeViewController, logger)
        {

        }

        public override IEnumerable<ArtifactTreeNodeCommand> GetContextMenuCommands(IArtifact artifact)
        {
            var commands = base.GetContextMenuCommands(artifact).ToList();

            var messageBus = ServiceProviderHolder.GetRequiredService<WorkspaceMessageBus>().PublishArtifactContextMenuOpening(artifact, commands);

            return messageBus.Commands;
        }
    }

    public abstract class WorkspaceArtifactControllerBase<TArtifact, TEditViewModel> : WorkspaceArtifactControllerBase<TArtifact>
        where TArtifact : WorkspaceArtifactBase
        where TEditViewModel : IArtifactEditViewModel
    {
        public TEditViewModel? EditViewModel { get; private set; }
        public WorkspaceArtifactControllerBase(OperationExecutor operationExecutor, WorkspaceTreeViewController treeViewController, ILogger logger)
            : base(operationExecutor, treeViewController, logger)
        {
        }

        protected override Task OnSelectedInternalAsync(TArtifact artifact, CancellationToken cancellationToken)
        {
            return ShowPropertiesAsync(artifact);
        }

        protected void EnsureEditViewModel(TArtifact artifact)
        {
            if (EditViewModel == null)
            {
                EditViewModel = Activator.CreateInstance<TEditViewModel>();
                EditViewModel.ValueChanged += OnEditViewModelValueChanged;
            }

            EditViewModel.Artifact = artifact;
        }

        private void OnEditViewModelValueChanged(object? sender, ArtifactPropertyChangedEventArgs e)
        {
            TreeViewController.OnArtifactPropertyChanged(e.Artifact, e.PropertyName, e.NewValue);
        }

        protected Task ShowPropertiesAsync(TArtifact artifact)
        {
            EnsureEditViewModel(artifact);
            TreeViewController.ShowArtifactDetailsView(EditViewModel!);
            return Task.CompletedTask;
        }
    }
}