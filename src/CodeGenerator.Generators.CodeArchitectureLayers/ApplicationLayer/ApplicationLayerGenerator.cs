using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Domain.CodeArchitecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.CodeArchitectureLayers.ApplicationLayer
{
    public class ApplicationLayerGenerator : CodeArchitectureLayerGenerator
    {
        protected override GeneratorSettingsDescription ConfigureSettingsDescription()
        {
            var id = nameof(ApplicationLayerGenerator);
            var name = "Application Layer Generator";
            var description = "Generates LayerArtifacts for common scopes (shared, application) and domain scopes.";
            
            return new GeneratorSettingsDescription(id, name, description);
        }

        protected override CodeArchitectureLayerArtifact CreateLayerArtifact(string scope)
        {
            return new ApplicationLayerArtifact(scope);
        }

        protected override string LayerName { get { return CodeArchitectureLayerArtifact.APPLICATION_SCOPE; } }
    }
}
