using CodeGenerator.Core.Generators;
using CodeGenerator.Domain.DotNet;
using CodeGenerator.Domain.DotNet.ProjectType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.Events
{
    public class DotNetSolutionArtifactGeneratedEventArgs : GeneratorContextEventArgs
    {
        public DotNetSolutionArtifact SolutionArtifact { get; }
        public DotNetSolution Solution { get; }


        public DotNetSolutionArtifactGeneratedEventArgs(DotNetSolutionArtifact solutionArtifact, DotNetSolution solution, GenerationResult result) : base(result)
        {
            SolutionArtifact = solutionArtifact;
            Solution = solution;
        }
    }
}
