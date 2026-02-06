using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.VVMC;
using CodeGenerator.Core.Workspaces.Artifacts.VVMC.Controllers;
using CodeGenerator.Core.Workspaces.MessageBus.EventHandlers;
using CodeGenerator.Core.Workspaces.MessageBus.Events;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Generators.DotNet.WinformsRibbonApplication.Workspace.Artifacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.WinformsRibbonApplication.Workspace.Subscribers
{
    public class ControllersContainerToApplicationLayerArtifactChildAddedSubscriber : WorkspaceArtifactChildAddedSubscriber<OnionApplicationLayerArtifact, ControllersContainerArtifact>
    {
        protected override void HandleArtifactChildAdded(ArtifactChildAddedEventArgs args, OnionApplicationLayerArtifact layerArtifact, ControllersContainerArtifact controllersContainerArtifact)
        {
            if (layerArtifact.ScopeName == CodeArchitectureScopes.APPLICATION_SCOPE)
            {
                controllersContainerArtifact.AddChild(new ApplicationControllerArtifact());
            }
            else
            {
                controllersContainerArtifact.AddChild(new ScopeControllerArtifact());
            }
        }
    }
}
