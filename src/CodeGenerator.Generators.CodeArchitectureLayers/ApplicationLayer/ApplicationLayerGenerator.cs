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
        public ApplicationLayerGenerator()
        {
            var templateRequirements = new List<TemplateRequirement>();

            // FolderProjectTemplateRequirement for Application Layer
            // templateRequirements.Add(new FolderProjectTemplateRequirement("ApplicationLayer"));

            SettingsDescription = new GeneratorSettingsDescription(nameof(ApplicationLayerGenerator), "Application Layer Generator Settings", "Generates LayerArtifacts for common scopes (shared, application) and domain scopes.", templateRequirements);

        }

        protected override CodeArchitectureLayerArtifact CreateLayerArtifact(string scope)
        {
            return new ApplicationLayerArtifact(scope);
        }

        protected override string LayerName { get { return CodeArchitectureLayerArtifact.APPLICATION_SCOPE; } }
        public override GeneratorSettingsDescription SettingsDescription { get; }
    }
}
