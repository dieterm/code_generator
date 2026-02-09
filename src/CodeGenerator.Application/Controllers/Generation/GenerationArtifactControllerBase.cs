using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Generation
{
    public abstract class GenerationArtifactControllerBase<TArtifact> : ArtifactControllerBase<GenerationTreeViewController, TArtifact>, IGenerationArtifactController
        where TArtifact : class, IArtifact
    {
        protected GenerationArtifactControllerBase(OperationExecutor operationExecutor, GenerationTreeViewController treeViewController, ILogger logger)
            : base(operationExecutor, treeViewController, logger)
        {
        }
    }
}
