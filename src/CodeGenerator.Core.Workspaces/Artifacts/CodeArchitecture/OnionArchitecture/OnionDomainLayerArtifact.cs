using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Core.Workspaces.Operations.Domains;
using CodeGenerator.Core.Workspaces.Operations.Scopes;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Operations;
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

        public IEnumerator<DomainArtifact> GetEnumerator()
        {
            return Children.OfType<DomainArtifact>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public DomainArtifact? FindDomain(string domainName, bool exceptionIfNotFound = true)
        {
            var scope = (Parent as ScopeArtifact)!;// provider.CurrentWorkspace!.FindScope(scopeName);
            var domain = this.FirstOrDefault(d => d.Name.Equals(domainName, StringComparison.OrdinalIgnoreCase));
            if (domain == null && exceptionIfNotFound)
                 throw new InvalidOperationException($"Domain '{domainName}' not found in scope '{scope.Name}'. Available domains: {string.Join(", ", this.Select(d => d.Name))}");
            return domain;
        }
    }
}
