using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Generation
{
    public abstract class GenerationArtifactControllerBase<TArtifact> : ArtifactControllerBase<GenerationTreeViewController, TArtifact>, IGenerationArtifactController
        where TArtifact : class, IArtifact
    {
        protected GenerationArtifactControllerBase(GenerationTreeViewController treeViewController, ILogger logger)
            : base(treeViewController, logger)
        {
        }
    }
}
