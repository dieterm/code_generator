using CodeGenerator.Core.Enums;
using CodeGenerator.Core.Generators;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Generators.Infrastructure
{
    public class InfrastructureProjectGenerator : BaseProjectGenerator
    {
        public InfrastructureProjectGenerator(ILogger<DomainProjectGenerator> logger) 
            : base(logger, ArchitectureLayer.Infrastructure, DotNetProjectType.ClassLib, nameof(DomainProjectGenerator), "Infrastructure Project Generator", "Generates an infrastructure project structure")
        {
        }
    }
}
