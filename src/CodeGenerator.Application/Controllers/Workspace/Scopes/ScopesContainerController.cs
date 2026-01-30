using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Application.Controllers.Workspace.Scopes
{
    public class ScopesContainerController : ArtifactControllerBase<WorkspaceTreeViewController, ScopesContainerArtifact>
    {
        public ScopesContainerController(WorkspaceTreeViewController treeViewController, ILogger<ScopesContainerController> logger)
            : base(treeViewController, logger)
        {
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(ScopesContainerArtifact artifact)
        {
            yield return new ArtifactTreeNodeCommand
            {
                Id = $"add_scope",
                Text = "Add Scope",
                IconKey = "plus-circle",
                Execute = async (a) => await AddScopeAsync(artifact)
            };
        }

        private Task AddScopeAsync(ScopesContainerArtifact artifact)
        {
            artifact.AddScope("New Scope");
            return Task.CompletedTask;
        }
    }
}
