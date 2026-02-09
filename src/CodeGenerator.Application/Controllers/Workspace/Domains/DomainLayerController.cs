using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
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
    public class DomainLayerController : WorkspaceArtifactControllerBase<OnionDomainLayerArtifact>
    {
        public DomainLayerController(OperationExecutor operationExecutor, WorkspaceTreeViewController treeViewController, ILogger<DomainLayerController> logger)
            : base(operationExecutor, treeViewController, logger)
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
            var parentScope = artifact.Parent as ScopeArtifact;
            var addDomainOperation = ServiceProviderHolder.GetRequiredService<AddDomainToScopeOperation>();
            var addDomainParams = new AddDomainToScopeParams {
                DomainName = "New Domain",
                ScopeId = parentScope!.Id
            };
            var result = OperationExecutor.Execute(addDomainOperation, addDomainParams);
            if (result.Success)
            {
                var createdDomain = addDomainParams.CreatedDomain;
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
