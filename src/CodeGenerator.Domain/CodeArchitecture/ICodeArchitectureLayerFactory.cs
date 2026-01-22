using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.CodeArchitecture
{
    public interface ICodeArchitectureLayerFactory
    {
        CodeArchitectureLayerArtifact CreateLayer(string scope);
    }
}
