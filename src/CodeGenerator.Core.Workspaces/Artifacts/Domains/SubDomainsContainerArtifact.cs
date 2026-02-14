using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Artifacts.Domains
{
    public class SubDomainsContainerArtifact : WorkspaceArtifactBase, IEnumerable<DomainArtifact>
    {
        public SubDomainsContainerArtifact() { }

        public SubDomainsContainerArtifact(ArtifactState state)
            : base(state)
        {
            
        }
        public override string TreeNodeText => "Sub-Domains";

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("boxes");

        public IEnumerator<DomainArtifact> GetEnumerator()
        {
            return Children.OfType<DomainArtifact>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
