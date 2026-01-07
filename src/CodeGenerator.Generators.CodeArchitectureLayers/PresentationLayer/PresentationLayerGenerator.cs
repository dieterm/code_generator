using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Domain.CodeArchitecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.CodeArchitectureLayers.PresentationLayer
{
    public class PresentationLayerGenerator : CodeArchitectureLayerGenerator
    {
        public PresentationLayerGenerator()
        {
            var templateRequirements = new List<TemplateRequirement>();

            // FolderProjectTemplateRequirement for Application Layer
            // templateRequirements.Add(new FolderProjectTemplateRequirement("PresentationLayer"));

            SettingsDescription = new GeneratorSettingsDescription(nameof(PresentationLayerGenerator), "Presentation Layer Generator Settings", "Generates LayerArtifacts for common scopes (shared, application) and domain scopes.", templateRequirements);

        }

        protected override CodeArchitectureLayerArtifact CreateLayerArtifact(string scope)
        {
            return new PresentationLayerArtifact(scope);
        }

        protected override string LayerName { get { return CodeArchitectureLayerArtifact.PRESENTATION_LAYER; } }
        public override GeneratorSettingsDescription SettingsDescription { get; }
    }
}
