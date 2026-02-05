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

namespace CodeGenerator.Application.Controllers.Workspace.Domains
{
    public class DomainLayerController : WorkspaceArtifactControllerBase<WorkspaceTreeViewController, OnionDomainLayerArtifact>
    {
        public DomainLayerController(WorkspaceTreeViewController treeViewController, ILogger<DomainLayerController> logger)
            : base(treeViewController, logger)
        {
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(OnionDomainLayerArtifact artifact)
        {
            yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = $"add_domain",
                Text = "Add Domain",
                IconKey = "plus-circle",
                Execute = async (a) => await AddDomainAsync(artifact)
            };
        }

        private Task AddDomainAsync(OnionDomainLayerArtifact artifact)
        {
            artifact.AddDomain("New Domain");
            return Task.CompletedTask;
        }
    }
}
