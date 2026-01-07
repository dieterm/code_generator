using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.CodeArchitecture
{
    public abstract class CodeArchitectureLayerArtifact : Artifact
    {
        public const string APPLICATION_LAYER = "Application";
        public const string DOMAIN_LAYER = "Domain";
        public const string INFRASTRUCTURE_LAYER = "Infrastructure";
        public const string PRESENTATION_LAYER = "Presentation";
        public const string SHARED_SCOPE = "Shared";
        public const string APPLICATION_SCOPE = "Application";

        public CodeArchitectureLayerArtifact(string layer, string scope)
        {
            Scope = scope;
            Layer = layer;
        }

        public override string Id { get { return $"{Layer}Layer:{Scope}"; } }
        /// <summary>
        /// Name of the layer (e.g., "Application", "Domain", "Infrastructure")
        /// </summary>
        public string Layer
        {
            get { return GetProperty<string>(nameof(Layer)); }
            private set { SetProperty(nameof(Layer), value); }
        }
        /// <summary>
        /// Scope of the layer (e.g., "Shared", "Application", "ProjectA", "ModuleB")
        /// </summary>
        public string Scope
        {
            get { return GetProperty<string>(nameof(Scope)); }
            private set { SetProperty(nameof(Scope), value); }
        }

        public override string TreeNodeText {  get { return $"{Layer} Layer ({Scope})"; } }

        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("layers");
}
}
