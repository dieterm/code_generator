using CodeGenerator.Core.Enums;
using CodeGenerator.Core.Generators;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.Presentation
{
    public class PresentationProjectGenerator : BaseProjectGenerator
    {
        public PresentationProjectGenerator(ILogger<PresentationProjectGenerator> logger) 
            : base(logger, ArchitectureLayer.Presentation, DotNetProjectType.WinFormsLib, nameof(PresentationProjectGenerator), "Presentation Project Generator", "Generates a presentation project structure")
        {

        }
    }
}
