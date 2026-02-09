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
        private readonly OperationExecutor _operationExecutor;
        private readonly AddScopeToWorkspaceOperation _addScopeOperation;

        public ScopesContainerController(
            OperationExecutor operationExecutor,
            AddScopeToWorkspaceOperation addScopeOperation,
            WorkspaceTreeViewController treeViewController,
            ILogger<ScopesContainerController> logger)
            : base(operationExecutor, treeViewController, logger)
        {
            _operationExecutor = operationExecutor;
            _addScopeOperation = addScopeOperation;
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
                    _operationExecutor.Execute(_addScopeOperation, new AddScopeToWorkspaceParams
                    {
                        ScopeName = "New scope"
                    });
                    await Task.CompletedTask;
                }
            };
        }
    }
}
