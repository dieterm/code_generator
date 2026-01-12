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
        public const string DOMAIN_SCOPE = "Domain";

        public CodeArchitectureLayerArtifact(string layer, string scope)
        {
            Scope = scope;
            Layer = layer;
        }
        public CodeArchitectureLayerArtifact(ArtifactState state)
            : base(state)
        {
        }
        //public override string Id { get { return $"{Layer}Layer:{Scope}"; } }

        /// <summary>
        /// Name of the layer (e.g., "Application", "Domain", "Infrastructure")
        /// </summary>
        public string Layer
        {
            get { return GetValue<string>(nameof(Layer)); }
            private set { 
                SetValue<string>(nameof(Layer), value);
                RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }
        /// <summary>
        /// Scope of the layer (e.g., "Shared", "Application", "ProjectA", "ModuleB")
        /// </summary>
        public string Scope
        {
            get { return GetValue<string>(nameof(Scope)); }
            private set { 
                SetValue<string>(nameof(Scope), value);
                RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }

        public override string TreeNodeText {  get { return $"{Layer} Layer ({Scope})"; } }

        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("layers");
}
}
