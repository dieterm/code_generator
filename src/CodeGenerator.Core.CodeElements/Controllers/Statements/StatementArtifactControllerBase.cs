using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts.Statements;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.CodeElements.Controllers.Statements
{
    public abstract class StatementArtifactControllerBase<TArtifact> : ArtifactControllerBase<CodeElementsTreeViewController, TArtifact>, ICodeElementArtifactController
        where TArtifact : CodeElementArtifactBase
    {
        protected StatementArtifactControllerBase(OperationExecutor operationExecutor, CodeElementsTreeViewController treeViewController, ILogger logger)
            : base(operationExecutor, treeViewController, logger)
        {
        }
    }
}