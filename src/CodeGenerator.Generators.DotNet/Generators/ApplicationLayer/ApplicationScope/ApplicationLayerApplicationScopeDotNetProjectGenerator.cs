using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Generators.MessageBus;
using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.DotNet;
using CodeGenerator.TemplateEngines.DotNetProject;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.Generators.ApplicationLayer.ApplicationScope
{
    public class ApplicationLayerApplicationScopeDotNetProjectGenerator : DotNetProjectGenerator<ApplicationLayerArtifact>
    {
        public ApplicationLayerApplicationScopeDotNetProjectGenerator(ILogger<ApplicationLayerApplicationScopeDotNetProjectGenerator> logger, DotNetProjectTemplateEngine dotNetProjectTemplateEngine)
            : base(CodeArchitectureLayerArtifact.APPLICATION_LAYER, CodeArchitectureLayerArtifact.APPLICATION_SCOPE, dotNetProjectTemplateEngine, logger)
        {
        }

        protected override async Task<DotNetProjectArtifact> OnLayerScopeCreatedAsync(CreatedArtifactEventArgs args)
        {
            var dotNetProjectArtifact = await base.OnLayerScopeCreatedAsync(args);
            // Additional configuration for Application Scope projects can be added here
            return dotNetProjectArtifact;
        }
    }
}
