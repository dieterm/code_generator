using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
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
            var codeArchitecture = scopesContainer.Workspace!.CodeArchitecture;
            if (codeArchitecture != null)
            {
                var newScope = codeArchitecture.ScopeFactory.CreateScopeArtifact("New scope");
                scopesContainer.AddChild(newScope);
            }
            return Task.CompletedTask;
        }
    }
}
