using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.CodeArchitecture
{
    public class CodeArchitecture
    {
        public CodeArchitecture(string id, string name, IEnumerable<ICodeArchitectureLayerFactory> layers)
        {
            Id = id;
            Name = name;
            Layers = layers;
        }
        public string Id { get; }
        public string Name { get;  }
        public IEnumerable<ICodeArchitectureLayerFactory> Layers { get; }

    }
}
