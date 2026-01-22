using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Generators
{
    public class DomainArtifactRefDecorator : ArtifactDecorator
    {
        public const string KeyPrefix = "DomainArtifactCodeArchitectureLayerDecorator";
        public DomainArtifactRefDecorator(DomainArtifact domainArtifact)
            : base(KeyPrefix)
        {
            this.DomainArtifact = domainArtifact;
        }
        public DomainArtifactRefDecorator(ArtifactDecoratorState state) : base(state)
        {
            throw new NotImplementedException("DomainArtifactCodeArchitectureLayerDecorator does not support (deserialization yet)");
        }
        public DomainArtifact? DomainArtifact { 
            get { return GetValue<DomainArtifact>(nameof(DomainArtifact)); }
            set { SetValue<DomainArtifact>(nameof(DomainArtifact), value); }
        }
    }
}
