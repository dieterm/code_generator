using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Generators.CodeArchitectureLayers.DomainLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.CodeArchitectureLayers.InfrastructureLayer
{
    public class InfrastructureLayerGenerator : CodeArchitectureLayerGenerator
    {

        protected override GeneratorSettingsDescription ConfigureSettingsDescription()
        {
            var id = nameof(InfrastructureLayerGenerator);
            var name = "Infrastructure Layer Generator";
            var description = "Generates LayerArtifacts for common scopes (shared, application) and domain scopes.";
            
            return CodeArchitectureLayerGeneratorSettings.CreateDescription(id, name, description, LayerName);
        }

        protected override CodeArchitectureLayerArtifact CreateLayerArtifact(string scope)
        {
            return new InfrastructureLayerArtifact(scope);
        }

        protected override string LayerName { get { return CodeArchitectureLayerArtifact.INFRASTRUCTURE_LAYER; } }
    }
}
