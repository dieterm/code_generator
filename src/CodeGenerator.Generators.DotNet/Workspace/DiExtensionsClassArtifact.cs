using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Domain.DesignPatterns.Structural.DependancyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.Workspace
{
    public class DiExtensionsClassArtifact : CodeFileArtifact
    {
        public const string DI_CONTAINER_EXTENSIONS_CLASS_NAME = "DiContainerExtensions";
        public DiExtensionsClassArtifact(ProgrammingLanguage programmingLanguage) 
            : base(DI_CONTAINER_EXTENSIONS_CLASS_NAME, programmingLanguage)
        {

        }
        public List<ServiceRegistration> ServiceRegistrations { get; } = new List<ServiceRegistration>();
    }
}
