using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Core.Workspaces.Operations.Scopes;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Application.Controllers.Workspace.Scopes
{
    public class SubScopesContainerController : WorkspaceArtifactControllerBase<SubScopesContainerArtifact>
    {
        public SubScopesContainerController(OperationExecutor operationExecutor, WorkspaceTreeViewController treeViewController, ILogger<SubScopesContainerController> logger)
            : base(operationExecutor, treeViewController, logger)
        {
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(SubScopesContainerArtifact artifact)
        {
            yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = $"add_scope",
                Text = "Add Scope",
                IconKey = "plus-circle",
                Execute = async (a) => await AddScopeAsync(artifact)
            };
        }

        private Task AddScopeAsync(SubScopesContainerArtifact scopesContainer)
        {
            var parentScope = scopesContainer.Parent as ScopeArtifact;
            var addSubScopeOperation = ServiceProviderHolder.GetRequiredService<AddSubScopeToScopeOperation>();
            var addSubScopeParams = new AddSubScopeToScopeParams
            {
                NewScopeName = "New scope",
                ParentScopeId = parentScope!.Id
            };
            var result = OperationExecutor.Execute(addSubScopeOperation, addSubScopeParams);

            if (result.Success)
            {
                var createdScope = addSubScopeParams.CreatedScope;
                if (createdScope != null)
                {
                    TreeViewController.OnArtifactAdded(scopesContainer, createdScope);
                    TreeViewController.RequestBeginRename(createdScope);
                }
            }

            return Task.CompletedTask;
        }
    }
}
