using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Domain.CodeArchitecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Artifacts.CodeArchitecture.HexagonArchitecture
{
    public class HexagonScopeFactory : IScopeArtifactFactory
    {
        private readonly CodeArchitectureManager _codeArchitectureManager;
        public HexagonScopeFactory(CodeArchitectureManager codeArchitectureManager)
        {
            _codeArchitectureManager = codeArchitectureManager;
        }

        public IArtifact CreateScopeArtifact(string scopeName)
        {
            return new HexagonScopeArtifact(scopeName, _codeArchitectureManager.HexagonArchitecture.Layers);
        }
    }
}
