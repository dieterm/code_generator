using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Workspaces.Artifacts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Application.Controllers.Workspace
{
    public class DomainsContainerController : ArtifactControllerBase<WorkspaceTreeViewController, DomainsContainerArtifact>
    {
        public DomainsContainerController(WorkspaceTreeViewController treeViewController, ILogger<DomainsContainerController> logger) 
            : base(treeViewController, logger)
        {
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(DomainsContainerArtifact artifact)
        {
            yield return new ArtifactTreeNodeCommand
            {
                Id = $"add_domain",
                Text = "Add Domain",
                IconKey = "box",
                Execute = async (a) => await AddDomainAsync(artifact)
            };
        }

        private async Task AddDomainAsync(DomainsContainerArtifact artifact)
        {
            
            var datasource = TreeViewController.AddDomain($"New domain");
            if (datasource != null)
            {
                await TreeViewController.SaveWorkspaceAsync();
            }
        }
    }
}
