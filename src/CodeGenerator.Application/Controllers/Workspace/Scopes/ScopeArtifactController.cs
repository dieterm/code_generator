using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.ViewModels.Workspace.Domains;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Application.Controllers.Workspace.Scopes
{
    /// <summary>
    /// Controller for ScopeArtifact
    /// </summary>
    public class ScopeArtifactController : WorkspaceArtifactControllerBase<WorkspaceTreeViewController, ScopeArtifact>
    {
        private ScopeEditViewModel? _editViewModel;

        public ScopeArtifactController(WorkspaceTreeViewController treeViewController, ILogger<ScopeArtifactController> logger)
            : base(treeViewController, logger)
        {

        }

        /// <summary>
        /// Handle Treeview EditLabel complete event
        /// </summary>
        protected override void OnArtifactRenamedInternal(ScopeArtifact artifact, string oldName, string newName)
        {
            artifact.Name = newName;
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(ScopeArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Rename command (only for non-default scopes)
            if (artifact.CanBeginEdit()) { 
                commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_RENAME)
                {
                    Id = "rename_scope",
                    Text = "Rename",
                    IconKey = "edit",
                    Execute = async (a) =>
                    {
                        TreeViewController.RequestBeginRename(artifact);
                        await Task.CompletedTask;
                    }
                });
            }

            // Properties command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "scope_properties",
                Text = "Properties",
                IconKey = "settings",
                Execute = async (a) => await ShowPropertiesAsync(artifact)
            });

            return commands;
        }

        #region Clipboard Operations

        public override bool CanDelete(ScopeArtifact artifact)
        {
            return !artifact.IsDefaultScope();
        }

        public override void Delete(ScopeArtifact artifact)
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

        protected override Task OnSelectedInternalAsync(ScopeArtifact artifact, CancellationToken cancellationToken)
        {
            return ShowPropertiesAsync(artifact);
        }

        private void EnsureEditViewModel(ScopeArtifact artifact)
        {
            if (_editViewModel == null)
            {
                _editViewModel = new ScopeEditViewModel();
                _editViewModel.ValueChanged += OnEditViewModelValueChanged;
            }

            _editViewModel.Scope = artifact;
        }

        private void OnEditViewModelValueChanged(object? sender, ArtifactPropertyChangedEventArgs e)
        {
            TreeViewController.OnArtifactPropertyChanged(e.Artifact, e.PropertyName, e.NewValue);
        }

        private Task ShowPropertiesAsync(ScopeArtifact scope)
        {
            EnsureEditViewModel(scope);
            TreeViewController.ShowArtifactDetailsView(_editViewModel!);
            return Task.CompletedTask;
        }
    }
}
