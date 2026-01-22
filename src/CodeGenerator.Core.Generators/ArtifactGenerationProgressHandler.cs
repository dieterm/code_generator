using CodeGenerator.Core.Artifacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Generators
{
    public class ArtifactGenerationProgressHandler : IProgress<ArtifactGenerationProgress>
    {
        private readonly IProgress<GenerationProgress>? _generationProgress;
        public ArtifactGenerationProgressHandler(IProgress<GenerationProgress>? generationProgress)
        {
            _generationProgress = generationProgress;
        }
        public void Report(ArtifactGenerationProgress artifactProgress)
        {
            _generationProgress?.Report(new GenerationProgress
            {
                TotalItems = artifactProgress.TotalStepsCount,
                PercentComplete = artifactProgress.PercentComplete,
                Message = artifactProgress.Message,
            });
        }
    }
}
