using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Domain.CodeArchitecture;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Artifacts
{
    public class OnionDomainLayerArtifact : CodeArchitectureLayerArtifact, IEnumerable<DomainArtifact>
    {
        public OnionDomainLayerArtifact(string initialScopeName) : base(OnionCodeArchitecture.DOMAIN_LAYER, initialScopeName)
        {
        }

        public OnionDomainLayerArtifact(ArtifactState state) : base(state)
        {
        }

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("boxes");

        public void AddDomain(string domainName)
        {
            AddChild(new DomainArtifact(domainName));
        }

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
