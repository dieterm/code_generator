using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Generators.MessageBus;
using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.DotNet;
using CodeGenerator.Generators.DotNet.Generators.PresentationLayer.ApplicationScope;
using CodeGenerator.TemplateEngines.DotNetProject;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.Generators.PresentationLayer.DomainScope
{
    public class PresentationLayerDomainScopeDotNetProjectGenerator : DotNetProjectGenerator<PresentationLayerArtifact>
    {
        public PresentationLayerDomainScopeDotNetProjectGenerator(ILogger<PresentationLayerDomainScopeDotNetProjectGenerator> logger, DotNetProjectTemplateEngine dotNetProjectTemplateEngine)
            : base(CodeArchitectureLayerArtifact.PRESENTATION_LAYER, CodeArchitectureLayerArtifact.DOMAIN_SCOPE, dotNetProjectTemplateEngine, logger)
        {
        }
        override public bool LayerArtifactFilter(CreatedArtifactEventArgs e)
        {
            return e.Artifact is PresentationLayerArtifact a && a.Scope != CodeArchitectureLayerArtifact.APPLICATION_SCOPE && a.Scope != CodeArchitectureLayerArtifact.SHARED_SCOPE;
        }
        protected override async Task<DotNetProjectArtifact> OnLayerScopeCreatedAsync(CreatedArtifactEventArgs args)
        {
            var dotNetProjectArtifact = await base.OnLayerScopeCreatedAsync(args);
            // Additional configuration for Application Scope projects can be added here
            return dotNetProjectArtifact;
        }
    }
}
