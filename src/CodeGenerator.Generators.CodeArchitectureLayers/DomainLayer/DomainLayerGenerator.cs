using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Domain.CodeArchitecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.CodeArchitectureLayers.DomainLayer
{
    public class DomainLayerGenerator : CodeArchitectureLayerGenerator
    {
        public DomainLayerGenerator()
        {
            var templateRequirements = new List<TemplateRequirement>();

            // FolderProjectTemplateRequirement for Application Layer
            // templateRequirements.Add(new FolderProjectTemplateRequirement("DomainLayer"));

            SettingsDescription = new GeneratorSettingsDescription(nameof(DomainLayerGenerator), "Domain Layer Generator Settings", "Generates LayerArtifacts for common scopes (shared, application) and domain scopes.", templateRequirements);

        }

        protected override CodeArchitectureLayerArtifact CreateLayerArtifact(string scope)
        {
            return new DomainLayerArtifact(scope);
        }

        protected override string LayerName { get { return CodeArchitectureLayerArtifact.DOMAIN_LAYER; } }
        public override GeneratorSettingsDescription SettingsDescription { get; }
    }
}
