using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.VVMC;
using CodeGenerator.Domain.CodeArchitecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Artifacts
{
    public class OnionApplicationLayerFactory : IOnionArchitectureLayerFactory
    {
        public string LayerName => OnionCodeArchitecture.APPLICATION_LAYER;

        public IArtifact CreateLayer(string scope)
        {
            // Implementation for creating an application layer artifact based on the scope
            var applicationLayer = new OnionApplicationLayerArtifact(scope);

            var controllersContainer = applicationLayer.AddChild(new ControllersContainerArtifact());
            
            var viewModelsContainer = applicationLayer.AddChild(new ViewModelsContainerArtifact());

            return applicationLayer;
        }
    }
}
