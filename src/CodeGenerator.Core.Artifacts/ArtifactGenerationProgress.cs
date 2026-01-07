using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts
{
    public class ArtifactGenerationProgress
    {
        public ArtifactGenerationProgress(Artifact artifact, string message, int currentStep, int totalStepsCount)
        {
            Artifact = artifact;
            Message = message;
            CurrentStep = currentStep;
            TotalStepsCount = totalStepsCount;
        }
        public Artifact Artifact { get; }
        public string Message { get; }
        public int CurrentStep { get; }
        public int TotalStepsCount { get; }
        public int PercentComplete { get { return (int)((CurrentStep / (float)TotalStepsCount) * 100); } }
    }
}
