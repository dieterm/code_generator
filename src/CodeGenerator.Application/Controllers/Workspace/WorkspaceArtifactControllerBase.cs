using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Application.Controllers.Workspace
{
    public  abstract class WorkspaceArtifactControllerBase<TArtifact> : ArtifactControllerBase<WorkspaceTreeViewController, TArtifact>, IWorkspaceArtifactController
        where TArtifact : WorkspaceArtifactBase
    {
        public WorkspaceArtifactControllerBase(OperationExecutor operationExecutor, WorkspaceTreeViewController treeViewController, ILogger logger)
            : base(operationExecutor, treeViewController, logger)
        {

        }
    }
}
