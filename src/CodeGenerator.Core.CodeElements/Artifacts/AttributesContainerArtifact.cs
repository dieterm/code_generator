using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CodeGenerator.Core.CodeElements.Artifacts
{
    public class AttributesContainerArtifact : CodeElementArtifactBase
    {
        private readonly List<AttributeElement> _attributes;
        public AttributesContainerArtifact(List<AttributeElement> attributes) : base()
        {
            _attributes = attributes;
            foreach(var attribute in attributes)
            {
                AddChild(new AttributeElementArtifact(attribute));
            }
        }

        public AttributesContainerArtifact(ArtifactState artifactState) : base(artifactState)
        {
        }

        public override string TreeNodeText => "Attributes";
        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("folder");

        public AttributeElementArtifact AddAttributeElement(AttributeElement attributeElement)
        {
            var artifact = new AttributeElementArtifact(attributeElement);
            _attributes.Add(attributeElement);
            AddChild(artifact);
            return artifact;
        }
    }
}
