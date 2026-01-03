using CodeGenerator.Core.Enums;
using CodeGenerator.Core.Generators;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Generators.Infrastructure
{
    public class DomainProjectGenerator : BaseProjectGenerator
    {
        public DomainProjectGenerator(ILogger<DomainProjectGenerator> logger) 
            : base(logger, ArchitectureLayer.Domain, DotNetProjectType.ClassLib, nameof(DomainProjectGenerator), "Domain Project Generator", "Generates a domain project structure")
        {
        }
    }
}
