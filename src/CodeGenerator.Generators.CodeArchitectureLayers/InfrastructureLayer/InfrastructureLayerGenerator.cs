using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Domain.CodeArchitecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.CodeArchitectureLayers.InfrastructureLayer
{
    public class InfrastructureLayerGenerator : CodeArchitectureLayerGenerator
    {
        public InfrastructureLayerGenerator()
        {
            var templateRequirements = new List<TemplateRequirement>();

            // FolderProjectTemplateRequirement for Application Layer
            // templateRequirements.Add(new FolderProjectTemplateRequirement("InfrastructureLayer"));

            SettingsDescription = new GeneratorSettingsDescription(nameof(InfrastructureLayerGenerator), "Infrastructure Layer Generator Settings", "Generates LayerArtifacts for common scopes (shared, application) and domain scopes.", templateRequirements);

        }

        protected override CodeArchitectureLayerArtifact CreateLayerArtifact(string scope)
        {
            return new InfrastructureLayerArtifact(scope);
        }

        protected override string LayerName { get { return CodeArchitectureLayerArtifact.INFRASTRUCTURE_LAYER; } }
        public override GeneratorSettingsDescription SettingsDescription { get; }
    }
}
