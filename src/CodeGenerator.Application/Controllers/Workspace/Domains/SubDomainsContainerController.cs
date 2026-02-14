using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Core.Workspaces.Operations.Domains;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Application.Controllers.Workspace.Domains
{
    public class SubDomainsContainerController : WorkspaceArtifactControllerBase<SubDomainsContainerArtifact>
    {
        public SubDomainsContainerController(OperationExecutor operationExecutor, WorkspaceTreeViewController treeViewController, ILogger<SubDomainsContainerController> logger) 
            : base(operationExecutor, treeViewController, logger)
        {
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(SubDomainsContainerArtifact artifact)
        {
            // Add Domain Command
            yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "AddSubDomain",
                Text = "Add Sub-Domain",
                IconKey = "plus-circle",
                Execute = async (a) => await AddSubDomainAsync(artifact)
            };
        }

        private Task AddSubDomainAsync(SubDomainsContainerArtifact artifact)
        {
            var parentDomain = artifact.Parent as DomainArtifact;
            if (parentDomain == null) return Task.CompletedTask;

            var addSubDomainOperation = ServiceProviderHolder.GetRequiredService<AddSubDomainOperation>();
            var parameters = new AddSubDomainParams
            {
                ParentDomainId = parentDomain.Id,
                NewDomainName = "New Domain"
            };

            var result = OperationExecutor.Execute(addSubDomainOperation, parameters);
            if (result.Success)
            {
                var createdDomain = parameters.CreatedDomain;
                if (createdDomain != null)
                {
                    TreeViewController.OnArtifactAdded(artifact, createdDomain);
                    TreeViewController.RequestBeginRename(createdDomain);
                }
            }

            return Task.CompletedTask;
        }
    }
}
