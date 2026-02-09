using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Core.Workspaces.Operations.Scopes;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Scopes
{
    public class ScopesContainerController : WorkspaceArtifactControllerBase<ScopesContainerArtifact>
    {
        public ScopesContainerController(
            OperationExecutor operationExecutor,
            WorkspaceTreeViewController treeViewController,
            ILogger<ScopesContainerController> logger)
            : base(operationExecutor, treeViewController, logger)
        {
            
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(ScopesContainerArtifact artifact)
        {
            yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "add_scope",
                Text = "Add Scope",
                IconKey = "plus-circle",
                Execute = async (a) =>
                {
                    var addScopeOperation = ServiceProviderHolder.GetRequiredService<AddScopeToWorkspaceOperation>();
                    var addScopeParams = new AddScopeToWorkspaceParams
                    {
                        ScopeName = "New scope",
                        ParentContainer = artifact
                    };
                    var result = OperationExecutor.Execute(addScopeOperation, addScopeParams);

                    if(result.Success)
                    {
                        var createdScope = addScopeParams.CreatedScope;
                        if (createdScope != null)
                        {
                            TreeViewController.OnArtifactAdded(artifact, createdScope);
                            TreeViewController.RequestBeginRename(createdScope);
                        }
                    }
                    
                    await Task.CompletedTask;
                }
            };
        }
    }
}
