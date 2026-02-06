using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Core.Workspaces.Artifacts.VVMC;
using CodeGenerator.Core.Workspaces.MessageBus;
using CodeGenerator.Core.Workspaces.MessageBus.EventHandlers;
using CodeGenerator.Core.Workspaces.MessageBus.Events;
using CodeGenerator.Domain.DotNet.ProjectType;
using CodeGenerator.Generators.DotNet.WinformsRibbonApplication.Workspace.Artifacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.WinformsRibbonApplication.Workspace.Subscribers
{
    public class OnionPresentationLayerContextMenuOpeningSubscriber : WorkspaceArtifactContextMenuOpeningSubscriber<OnionPresentationLayerArtifact>
    {
        protected override void HandleArtifactContextMenuOpening(ArtifactContextMenuOpeningEventArgs args, OnionPresentationLayerArtifact artifact)
        {
            if(artifact.Parent is ScopeArtifact scopeArtifact)
            {
                if (scopeArtifact.Name == ScopeArtifact.DEFAULT_SCOPE_APPLICATION)
                {
                    if(!artifact.Children.OfType<WinformsPresentationArtifact>().Any(p => p.ProjectType == DotNetProjectTypes.WinFormsExe)) 
                    { 
                        args.Commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
                        {
                            Id = "add_new_winforms_executable",
                            Text = "Add New Winforms Executable",
                            Execute = (art) =>
                            {
                                var newPresentation = new WinformsPresentationArtifact(DotNetProjectTypes.WinFormsExe);
                                art.AddChild(newPresentation);
                                var viewsContainer = newPresentation.AddChild(new ViewsContainerArtifact());
                                return Task.CompletedTask;
                            }
                        });
                    }
                }
                else {
                    if (!artifact.Children.OfType<WinformsPresentationArtifact>().Any(p => p.ProjectType == DotNetProjectTypes.WinFormsLib))
                    {
                        args.Commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
                        {
                            Id = "add_new_winforms_usercontrol_library",
                            Text = "Add New Winforms Usercontrol Library",
                            Execute = (art) =>
                            {
                                var newPresentation = new WinformsPresentationArtifact(DotNetProjectTypes.WinFormsLib);
                                art.AddChild(newPresentation);
                                var viewsContainer = newPresentation.AddChild(new ViewsContainerArtifact());
                                return Task.CompletedTask;
                            }
                        });
                    }
                }
            }
        }
    }
}
