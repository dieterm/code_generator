using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Domain.CodeArchitecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Artifacts.CodeArchitecture.CleanArchitecture
{
    public class CleanCodeScopeFactory : IScopeArtifactFactory
    {
        private readonly CodeArchitectureManager _codeArchitectureManager;
        public CleanCodeScopeFactory(CodeArchitectureManager codeArchitectureManager)
        {
            _codeArchitectureManager = codeArchitectureManager;
        }

        public IArtifact CreateScopeArtifact(string scopeName)
        {
            var scopeArtifact = new ScopeArtifact(scopeName);
            foreach (var layerFactory in _codeArchitectureManager.CleanArchitecture.Layers)
            {
                scopeArtifact.AddChild(layerFactory.CreateLayer(scopeName));
            }
            scopeArtifact.AddChild(new SubScopesContainerArtifact());
            return scopeArtifact;
        }
    }
}
