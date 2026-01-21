using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Artifacts
{
    public class DomainsContainerArtifact : Artifact
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

        /// <summary>
        /// Add a domain
        /// </summary>
        public void AddDomain(DomainArtifact domainArtifact)
        {
           AddChild(domainArtifact);
        }

        /// <summary>
        /// Remove a domain
        /// </summary>
        public void RemoveDomain(DomainArtifact domainArtifact)
        {
            RemoveChild(domainArtifact);
        }
    }
}
