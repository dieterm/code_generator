using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Generators.MessageBus;
using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.DotNet;
using CodeGenerator.Generators.DotNet.Events;
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
        public DomainLayerSharedScopeDotNetProjectGenerator(ILogger<DomainLayerSharedScopeDotNetProjectGenerator> logger)
            : base(CodeArchitectureLayerArtifact.DOMAIN_LAYER, CodeArchitectureLayerArtifact.SHARED_SCOPE, logger)
        {
        }

        //protected override void OnCreatingRootArtifact(CreatingArtifactEventArgs e)
        //{
        //    // we should exist on our own
        //    // no need to track references from other projects
        //}

        //protected override void OnDotNetProjectCreated(DotNetProjectArtifactCreatedEventArgs e)
        //{
        //    // we should exist on our own
        //    // no need to track references from other projects
        //}

        protected override async Task<DotNetProjectArtifact> OnLayerScopeCreatedAsync(CreatedArtifactEventArgs args)
        {
            var dotNetProjectArtifact = await base.OnLayerScopeCreatedAsync(args);
            // Additional configuration for Application Scope projects can be added here
            return dotNetProjectArtifact;
        }
    }
}
