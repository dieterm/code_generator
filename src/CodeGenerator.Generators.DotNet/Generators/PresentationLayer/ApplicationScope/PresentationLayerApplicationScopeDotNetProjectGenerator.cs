using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Generators.MessageBus;
using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Core.Templates;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.DotNet;
using CodeGenerator.Generators.DotNet.Generators.InfrastructureLayer.SharedScope;
using CodeGenerator.TemplateEngines.DotNetProject;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.Generators.PresentationLayer.ApplicationScope
{
    public class PresentationLayerApplicationScopeDotNetProjectGenerator : DotNetProjectGenerator<PresentationLayerArtifact>
    {
        //  $"{generator.Layer}Layer.{generator.Scope}Scope.DotNetProject"
        // eg. "PresentationLayerApplicationScopeDotNetProject"
        public const string GENERATOR_ID = $"{CodeArchitectureLayerArtifact.PRESENTATION_LAYER}Layer.{CodeArchitectureLayerArtifact.APPLICATION_SCOPE}Scope.DotNetProject";
        public PresentationLayerApplicationScopeDotNetProjectGenerator(ILogger<PresentationLayerApplicationScopeDotNetProjectGenerator> logger)
            : base(CodeArchitectureLayerArtifact.PRESENTATION_LAYER, CodeArchitectureLayerArtifact.APPLICATION_SCOPE, logger)
        {
        }

        protected override async Task<DotNetProjectArtifact> OnLayerScopeCreatedAsync(CreatedArtifactEventArgs args)
        {
            var dotNetProjectArtifact = await base.OnLayerScopeCreatedAsync(args);
            // Customize the project artifact as needed for the presentation layer application scope
            return dotNetProjectArtifact;
        }
        protected override GeneratorSettingsDescription ConfigureSettingsDescription()
        {
            return base.ConfigureSettingsDescription();
        }
    }
}
