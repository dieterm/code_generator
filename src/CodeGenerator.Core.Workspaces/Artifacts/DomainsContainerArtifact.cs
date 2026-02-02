using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Artifacts
{
    public class DomainsContainerArtifact : WorkspaceArtifactBase, IEnumerable<DomainArtifact>
    {
        public DomainsContainerArtifact()
            : base()
        {
            Id = "domains_container";
        }

        public DomainsContainerArtifact(ArtifactState state)
            : base(state)
        {
            Id = "domains_container";
        }

        public override string TreeNodeText => "Domains";

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
