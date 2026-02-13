using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities
{
    public class PropertiesContainerArtifact : WorkspaceArtifactBase
    {
        public PropertiesContainerArtifact()
        {
        }

        public PropertiesContainerArtifact(ArtifactState state) : base(state)
        {
        }

        public override string TreeNodeText => "Properties";

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("table-properties");

        public PropertyArtifact AddProperty(PropertyArtifact createdProperty)
        {
            AddChild(createdProperty);
            return createdProperty;
        }
    }
}
