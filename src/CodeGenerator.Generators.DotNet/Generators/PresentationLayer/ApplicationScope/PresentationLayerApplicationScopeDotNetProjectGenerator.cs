using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Generators.MessageBus;
using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.Generators.PresentationLayer.ApplicationScope
{
    public class PresentationLayerApplicationScopeDotNetProjectGenerator : DotNetProjectGenerator<PresentationLayerArtifact>
    {
        public PresentationLayerApplicationScopeDotNetProjectGenerator()
            : base(CodeArchitectureLayerArtifact.PRESENTATION_LAYER, CodeArchitectureLayerArtifact.APPLICATION_SCOPE)
        {
        }

        protected override DotNetProjectArtifact OnLayerScopeCreated(CreatedArtifactEventArgs args)
        {
            var dotNetProjectArtifact = base.OnLayerScopeCreated(args);
            // Additional configuration for Application Scope projects can be added here
            return dotNetProjectArtifact;
        }
    }
}
