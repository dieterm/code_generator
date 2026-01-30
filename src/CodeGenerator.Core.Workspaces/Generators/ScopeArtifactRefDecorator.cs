using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Generators
{
    public class ScopeArtifactRefDecorator : ArtifactDecorator
    {
        public const string KeyPrefix = "ScopeArtifactCodeArchitectureLayerDecorator";
        public ScopeArtifactRefDecorator(ScopeArtifact domainArtifact)
            : base(KeyPrefix)
        {
            this.ScopeArtifact = domainArtifact;
        }
        public ScopeArtifactRefDecorator(ArtifactDecoratorState state) : base(state)
        {
            throw new NotImplementedException("ScopeArtifactCodeArchitectureLayerDecorator does not support (deserialization yet)");
        }
        public ScopeArtifact? ScopeArtifact { 
            get { return GetValue<ScopeArtifact>(nameof(ScopeArtifact)); }
            set { SetValue<ScopeArtifact>(nameof(ScopeArtifact), value); }
        }
    }
}
