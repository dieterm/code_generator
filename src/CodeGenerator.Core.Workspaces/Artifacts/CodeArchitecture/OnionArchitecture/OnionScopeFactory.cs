using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Domain.CodeArchitecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Artifacts.CodeArchitecture.OnionArchitecture
{
    public class OnionScopeFactory : IScopeArtifactFactory
    {
        private readonly CodeArchitectureManager _codeArchitectureManager;

        public OnionScopeFactory(CodeArchitectureManager codeArchitectureManager) 
        {
            _codeArchitectureManager = codeArchitectureManager;
        }

        public IArtifact CreateScopeArtifact(string scopeName)
        {
            var scopeArtifact = new ScopeArtifact(scopeName);
            foreach (var layerFactory in _codeArchitectureManager.OnionArchitecture.Layers)
            {
                scopeArtifact.AddChild(layerFactory.CreateLayer(scopeName));
            }
            scopeArtifact.AddChild(new SubScopesContainerArtifact());
            return scopeArtifact;
        }


    }
}
