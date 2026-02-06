using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Core.Workspaces.Settings;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Application.Controllers.Workspace.Scopes
{
    public class ScopesContainerController : WorkspaceArtifactControllerBase<ScopesContainerArtifact>
    {
        public ScopesContainerController(WorkspaceTreeViewController treeViewController, ILogger<ScopesContainerController> logger)
            : base(treeViewController, logger)
        {
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(ScopesContainerArtifact artifact)
        {
            yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = $"add_scope",
                Text = "Add Scope",
                IconKey = "plus-circle",
                Execute = async (a) => await AddScopeAsync(artifact)
            };
        }

        private Task AddScopeAsync(ScopesContainerArtifact scopesContainer)
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
