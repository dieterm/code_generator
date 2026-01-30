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
        public ScopeArtifact SharedDomain => Children.OfType<ScopeArtifact>().First(d => d.Name == ScopeArtifact.DEFAULT_DOMAIN_NAME_SHARED);

        private ScopeArtifact EnsureSharedDomainExists()
        {
            var newOrExisting = Children.OfType<ScopeArtifact>().FirstOrDefault(d => d.Name == ScopeArtifact.DEFAULT_DOMAIN_NAME_SHARED);
            if (newOrExisting == null) {
                newOrExisting = new ScopeArtifact(ScopeArtifact.DEFAULT_DOMAIN_NAME_SHARED);
                // Default pattern: {WorkspaceNamespace}.{Shared}
                newOrExisting.DefaultNamespacePattern = $"{{{WorkspaceArtifact.ProjectNamePattern_WorkspaceNamespaceParameter}}}.{{{ScopeArtifact.DEFAULT_DOMAIN_NAME_SHARED}}}";

                AddChild(newOrExisting);
            }
            
            return newOrExisting;
        }
        
        public ScopeArtifact ApplicationDomain => Children.OfType<ScopeArtifact>().First(d => d.Name == ScopeArtifact.DEFAULT_DOMAIN_NAME_APPLICATION);

        private ScopeArtifact EnsureApplicationDomainExists()
        {
            var newOrExisting = Children.OfType<ScopeArtifact>().FirstOrDefault(d => d.Name == ScopeArtifact.DEFAULT_DOMAIN_NAME_APPLICATION);
            if (newOrExisting == null)
            {
                newOrExisting = new ScopeArtifact(ScopeArtifact.DEFAULT_DOMAIN_NAME_APPLICATION);
                // Default pattern: {WorkspaceNamespace}.{Application}
                newOrExisting.DefaultNamespacePattern = $"{{{WorkspaceArtifact.ProjectNamePattern_WorkspaceNamespaceParameter}}}.{{{ScopeArtifact.DEFAULT_DOMAIN_NAME_APPLICATION}}}";

                AddChild(newOrExisting);
            }

            return newOrExisting;
        }

        /// <summary>
        /// Add a domain
        /// </summary>
        public void AddDomain(ScopeArtifact domainArtifact)
        {
           AddChild(domainArtifact);
        }

        public IEnumerable<ScopeArtifact> GetDomains()
        {
            return Children.OfType<ScopeArtifact>().ToArray();
        }

        /// <summary>
        /// Remove a domain
        /// </summary>
        public void RemoveDomain(ScopeArtifact domainArtifact)
        {
            RemoveChild(domainArtifact);
        }
    }
}
