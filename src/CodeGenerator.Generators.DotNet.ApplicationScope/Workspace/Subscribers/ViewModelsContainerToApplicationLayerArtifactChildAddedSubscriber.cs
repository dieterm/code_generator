using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.VVMC;
using CodeGenerator.Core.Workspaces.MessageBus.EventHandlers;
using CodeGenerator.Core.Workspaces.MessageBus.Events;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Generators.DotNet.ApplicationScope.Workspace.Artifacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.ApplicationScope.Workspace.Subscribers
{
    public class ViewModelsContainerToApplicationLayerArtifactChildAddedSubscriber : WorkspaceArtifactChildAddedSubscriber<OnionApplicationLayerArtifact, ViewModelsContainerArtifact>
    {
        protected override void HandleArtifactChildAdded(ArtifactChildAddedEventArgs args, OnionApplicationLayerArtifact layerArtifact, ViewModelsContainerArtifact viewModelsContainerArtifact)
        {
            if (layerArtifact.ScopeName == CodeArchitectureScopes.APPLICATION_SCOPE)
            {
                viewModelsContainerArtifact.AddChild(new ApplicationViewModelArtifact());
            }
        }
    }
}
