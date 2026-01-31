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

namespace CodeGenerator.Generators.DotNet.Generators.ApplicationLayer.SharedScope
{
    public class ApplicationLayerSharedScopeDotNetProjectGenerator : DotNetProjectGenerator<ApplicationLayerArtifact>
    {
        
        public ApplicationLayerSharedScopeDotNetProjectGenerator(ILogger<ApplicationLayerSharedScopeDotNetProjectGenerator> logger) 
            : base(CodeArchitectureLayerArtifact.APPLICATION_LAYER, CodeArchitectureLayerArtifact.SHARED_SCOPE, logger)
        {
        }


        protected override async Task<DotNetProjectArtifact> OnLayerScopeCreatedAsync(CreatedArtifactEventArgs args)
        {
            var dotNetProjectArtifact = await base.OnLayerScopeCreatedAsync(args);

            // Additional configuration for Shared Scope projects can be added here
            return dotNetProjectArtifact;
        }
        
    }
}
