using CodeGenerator.Shared;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.CodeArchitecture
{
    public class CodeArchitecture<TFactory> : ICodeArchitecture where TFactory : ICodeArchitectureLayerFactory
    {
        public CodeArchitecture(string id, string name)
        {
            Id = id;
            Name = name;
        }
        public string Id { get; }
        public string Name { get;  }
        public IEnumerable<TFactory> Layers { get { return ServiceProviderHolder.GetServices<TFactory>(); } }

        public IScopeArtifactFactory ScopeFactory { get { return ServiceProviderHolder.GetKeyedService<IScopeArtifactFactory>(Id)!; } }

        IEnumerable<ICodeArchitectureLayerFactory> ICodeArchitecture.Layers => Layers.Cast<ICodeArchitectureLayerFactory>();
    }
}
