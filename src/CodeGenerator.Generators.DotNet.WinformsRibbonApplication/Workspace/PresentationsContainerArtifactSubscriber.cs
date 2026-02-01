using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Core.Workspaces.MessageBus;
using CodeGenerator.Core.Workspaces.MessageBus.EventHandlers;
using CodeGenerator.Core.Workspaces.MessageBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.WinformsRibbonApplication
{
    public class PresentationsContainerArtifactSubscriber : WorkspaceArtifactContextMenuOpeningSubscriber<PresentationsContainerArtifact>
    {
        protected override void HandleArtifactContextMenuOpening(ArtifactContextMenuOpeningEventArgs args, PresentationsContainerArtifact artifact)
        {
            if(artifact.Parent is ScopeArtifact scopeArtifact && scopeArtifact.Name == ScopeArtifact.DEFAULT_SCOPE_APPLICATION)
            {
                args.Commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
                {
                    Id = "add_new_winforms_presentation",
                    Text = "Add New Winforms Application",
                    Execute = (art) =>
                    {
                        var newPresentation = new WinformsPresentationArtifact();
                        art.AddChild(newPresentation);
                        return Task.CompletedTask;
                    }
                });
     
            }
        }
    }
}
