using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Artifacts
{
    public class DomainsContainerArtifact : WorkspaceArtifactBase
    {
        public DomainsContainerArtifact()
            : base()
        {
            Id = "domains_container";
            EnsureApplicationDomainExists();
            EnsureSharedDomainExists();
        }

        public DomainsContainerArtifact(ArtifactState state)
            : base(state)
        {
            Id = "domains_container";
            EnsureApplicationDomainExists();
            EnsureSharedDomainExists();
        }
        public override string TreeNodeText => "Domains";

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("boxes");
        public DomainArtifact SharedDomain => Children.OfType<DomainArtifact>().First(d => d.Name == DomainArtifact.DEFAULT_DOMAIN_NAME_SHARED);

        private DomainArtifact EnsureSharedDomainExists()
        {
            var newOrExisting = Children.OfType<DomainArtifact>().FirstOrDefault(d => d.Name == DomainArtifact.DEFAULT_DOMAIN_NAME_SHARED);
            if (newOrExisting == null) {
                newOrExisting = new DomainArtifact(DomainArtifact.DEFAULT_DOMAIN_NAME_SHARED);
                // Default pattern: {WorkspaceNamespace}.{Shared}
                newOrExisting.DefaultNamespacePattern = $"{{{WorkspaceArtifact.ProjectNamePattern_WorkspaceNamespaceParameter}}}.{{{DomainArtifact.DEFAULT_DOMAIN_NAME_SHARED}}}";

                AddChild(newOrExisting);
            }
            
            return newOrExisting;
        }
        
        public DomainArtifact ApplicationDomain => Children.OfType<DomainArtifact>().First(d => d.Name == DomainArtifact.DEFAULT_DOMAIN_NAME_APPLICATION);

        private DomainArtifact EnsureApplicationDomainExists()
        {
            var newOrExisting = Children.OfType<DomainArtifact>().FirstOrDefault(d => d.Name == DomainArtifact.DEFAULT_DOMAIN_NAME_APPLICATION);
            if (newOrExisting == null)
            {
                newOrExisting = new DomainArtifact(DomainArtifact.DEFAULT_DOMAIN_NAME_APPLICATION);
                // Default pattern: {WorkspaceNamespace}.{Application}
                newOrExisting.DefaultNamespacePattern = $"{{{WorkspaceArtifact.ProjectNamePattern_WorkspaceNamespaceParameter}}}.{{{DomainArtifact.DEFAULT_DOMAIN_NAME_APPLICATION}}}";

                AddChild(newOrExisting);
            }

            return newOrExisting;
        }

        /// <summary>
        /// Add a domain
        /// </summary>
        public void AddDomain(DomainArtifact domainArtifact)
        {
           AddChild(domainArtifact);
        }

        public IEnumerable<DomainArtifact> GetDomains()
        {
            return Children.OfType<DomainArtifact>().ToArray();
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
