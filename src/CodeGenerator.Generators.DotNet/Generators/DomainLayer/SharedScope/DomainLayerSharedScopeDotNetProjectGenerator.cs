using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Generators.MessageBus;
using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.DotNet;
using CodeGenerator.Generators.DotNet.Generators.DomainLayer.DomainScope;
using CodeGenerator.TemplateEngines.DotNetProject;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.Generators.DomainLayer.SharedScope
{
    public class DomainLayerSharedScopeDotNetProjectGenerator : DotNetProjectGenerator<DomainLayerArtifact>
    {
        public DomainLayerSharedScopeDotNetProjectGenerator(ILogger<DomainLayerSharedScopeDotNetProjectGenerator> logger, DotNetProjectTemplateEngine dotNetProjectTemplateEngine)
            : base(CodeArchitectureLayerArtifact.DOMAIN_LAYER, CodeArchitectureLayerArtifact.SHARED_SCOPE, dotNetProjectTemplateEngine, logger)
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
