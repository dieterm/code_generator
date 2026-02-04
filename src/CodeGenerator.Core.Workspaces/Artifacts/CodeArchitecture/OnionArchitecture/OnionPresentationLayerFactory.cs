using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.VVMC;
using CodeGenerator.Domain.CodeArchitecture;
using System.Configuration;

namespace CodeGenerator.Core.Workspaces.Artifacts
{
    public class OnionPresentationLayerFactory : IOnionArchitectureLayerFactory
    {
        public string LayerName => OnionCodeArchitecture.PRESENTATION_LAYER;

        public IArtifact CreateLayer(string scope)
        {
            // Implementation for creating a presentation layer artifact based on the scope
            var presentationLayer = new OnionPresentationLayerArtifact(scope);

            var viewsContainer = presentationLayer.AddChild(new ViewsContainerArtifact());

            switch (scope)
            {
                case CodeArchitectureScopes.APPLICATION_SCOPE:
                    // TODO: add ApplicationView to viewsContainer
                    break;
                case CodeArchitectureScopes.DOMAIN_SCOPE:
                    // No specific action for shared scope in this example
                    break;
                case CodeArchitectureScopes.SHARED_SCOPE:
                    // No specific action for shared scope in this example
                    break;
                default:
                    break;
            }

            return presentationLayer;
        }
    }
}
