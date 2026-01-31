using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Generators.MessageBus;
using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.DotNet;
using CodeGenerator.Generators.DotNet.Generators.ApplicationLayer.ApplicationScope;
using CodeGenerator.TemplateEngines.DotNetProject;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.Generators.ApplicationLayer.DomainScope
{
    public class ApplicationLayerDomainScopeDotNetProjectGenerator : DotNetProjectGenerator<ApplicationLayerArtifact>
    {
        public ApplicationLayerDomainScopeDotNetProjectGenerator(ILogger<ApplicationLayerDomainScopeDotNetProjectGenerator> logger)
            : base(CodeArchitectureLayerArtifact.APPLICATION_LAYER, CodeArchitectureLayerArtifact.DOMAIN_SCOPE, logger)
        {
            
        }
        public override bool LayerArtifactFilter(CreatedArtifactEventArgs e)
        {
            return e.Artifact is ApplicationLayerArtifact a && a.Scope != CodeArchitectureLayerArtifact.APPLICATION_SCOPE && a.Scope != CodeArchitectureLayerArtifact.SHARED_SCOPE;
        }

        protected override async Task<DotNetProjectArtifact> OnLayerScopeCreatedAsync(CreatedArtifactEventArgs args)
        {
            var dotNetProjectArtifact = await base.OnLayerScopeCreatedAsync(args);
            // Additional customization for Domain Scope projects can be done here
            return dotNetProjectArtifact;
        }

    }
}
