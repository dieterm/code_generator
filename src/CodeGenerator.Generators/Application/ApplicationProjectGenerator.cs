using CodeGenerator.Core.Enums;
using CodeGenerator.Core.Generators;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Generators.Application
{
    public class ApplicationProjectGenerator : BaseProjectGenerator
    {
        public ApplicationProjectGenerator(ILogger<ApplicationProjectGenerator> logger) 
            : base(logger, ArchitectureLayer.Application, DotNetProjectType.ClassLib, nameof(ApplicationProjectGenerator), "Application Project Generator", "Generates an application project structure")
        {
        }
    }
}
