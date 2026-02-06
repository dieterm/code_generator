using CodeGenerator.Core.Artifacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.CodeArchitecture
{
    public interface IScopeArtifactFactory
    {
        IArtifact CreateScopeArtifact(string scopeName);
    }
}
