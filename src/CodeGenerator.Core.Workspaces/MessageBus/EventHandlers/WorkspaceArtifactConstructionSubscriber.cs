using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.MessageBus.EventHandlers;
using CodeGenerator.Core.Workspaces.MessageBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.MessageBus.EventHandlers
{
    public class WorkspaceArtifactConstructionSubscriber : ArtifactConstructionSubscriberBase<WorkspaceArtifact>
    {
        protected override void HandleArtifactCreation(ArtifactConstructionEventArgs args, WorkspaceArtifact artifact)
        {
            //EnsureContainerExists<DatasourcesContainerArtifact>(artifact);
            //EnsureContainerExists<DomainsContainerArtifact>(artifact);
            //EnsureContainerExists<InfrastructuresContainerArtifact>(artifact);
            //EnsureContainerExists<ApplicationsContainerArtifact>(artifact);
            //EnsureContainerExists<PresentationsContainerArtifact>(artifact);
        }

        //public static T EnsureContainerExists<T>(WorkspaceArtifact artifact) where T : WorkspaceArtifactBase, new()
        //{
        //    var container = artifact.Children.OfType<T>().FirstOrDefault();
        //    if (container == null)
        //    {
        //        container = new T();
        //        artifact.AddChild(container);
        //    }
        //    return container;
        //}
    }


}

//namespace CodeGenerator.Core.Workspaces.Artifacts
//{
//    public partial class WorkspaceArtifact
//    {
        
//        /// <summary>
//        /// Gets the domains container
//        /// </summary>
//        public DomainsContainerArtifact Domains { get { return WorkspaceArtifactConstructionSubscriber.EnsureContainerExists<DomainsContainerArtifact>(this); } }

//        public InfrastructuresContainerArtifact Infrastructures { get { return WorkspaceArtifactConstructionSubscriber.EnsureContainerExists<InfrastructuresContainerArtifact>(this); } }
//        public ApplicationsContainerArtifact Applications { get { return WorkspaceArtifactConstructionSubscriber.EnsureContainerExists<ApplicationsContainerArtifact>(this); } }
//        public PresentationsContainerArtifact Presentations { get { return WorkspaceArtifactConstructionSubscriber.EnsureContainerExists<PresentationsContainerArtifact>(this); } }
//    }
//}
