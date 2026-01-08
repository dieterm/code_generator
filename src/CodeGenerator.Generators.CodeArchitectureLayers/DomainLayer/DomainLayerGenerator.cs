using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Generators.CodeArchitectureLayers.ApplicationLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.CodeArchitectureLayers.DomainLayer
{
    public class DomainLayerGenerator : CodeArchitectureLayerGenerator
    {
        protected override GeneratorSettingsDescription ConfigureSettingsDescription()
        {
            var id = nameof(DomainLayerGenerator);
            var name = "Domain Layer Generator";
            var description = "Generates LayerArtifacts for common scopes (shared, application) and domain scopes.";
            
            return new GeneratorSettingsDescription(id, name, description);
        }

        protected override CodeArchitectureLayerArtifact CreateLayerArtifact(string scope)
        {
            return new DomainLayerArtifact(scope);
        }

        protected override string LayerName { get { return CodeArchitectureLayerArtifact.DOMAIN_LAYER; } }
    }
}
