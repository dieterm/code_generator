using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Generators.MessageBus;
using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.DotNet;
using CodeGenerator.Generators.DotNet.Generators.DomainLayer.ApplicationScope;
using CodeGenerator.TemplateEngines.DotNetProject;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.Generators.DomainLayer.DomainScope
{
    public class DomainLayerDomainScopeDotNetProjectGenerator : DotNetProjectGenerator<DomainLayerArtifact>
    {
        public DomainLayerDomainScopeDotNetProjectGenerator(ILogger<DomainLayerDomainScopeDotNetProjectGenerator> logger, DotNetProjectTemplateEngine dotNetProjectTemplateEngine)
            : base(CodeArchitectureLayerArtifact.DOMAIN_LAYER, CodeArchitectureLayerArtifact.DOMAIN_SCOPE, dotNetProjectTemplateEngine, logger)
        {
        }

        public override bool LayerArtifactFilter(CreatedArtifactEventArgs e)
        {
            return e.Artifact is DomainLayerArtifact a && a.Scope != CodeArchitectureLayerArtifact.APPLICATION_SCOPE && a.Scope != CodeArchitectureLayerArtifact.SHARED_SCOPE;
        }
        protected override async Task<DotNetProjectArtifact> OnLayerScopeCreatedAsync(CreatedArtifactEventArgs args)
        {
            var dotNetProjectArtifact = await base.OnLayerScopeCreatedAsync(args);
            // Additional configuration for Application Scope projects can be added here
            return dotNetProjectArtifact;
        }
    }
}
