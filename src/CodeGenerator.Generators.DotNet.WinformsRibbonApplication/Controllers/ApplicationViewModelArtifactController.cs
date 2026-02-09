using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Generators.DotNet.WinformsRibbonApplication.Workspace.Artifacts;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.WinformsRibbonApplication.Controllers
{
    public class ApplicationViewModelArtifactController : WorkspaceArtifactControllerBase<ApplicationViewModelArtifact>
    {
        public ApplicationViewModelArtifactController(OperationExecutor operationExecutor, WorkspaceTreeViewController treeViewController, ILogger logger) 
            : base(operationExecutor, treeViewController, logger)
        {
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(ApplicationViewModelArtifact artifact)
        {
            return Enumerable.Empty<ArtifactTreeNodeCommand>();
        }
    }
}
