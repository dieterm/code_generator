using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.MessageBus;
using CodeGenerator.Core.Workspaces.MessageBus.Events;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Artifacts
{
    public abstract class WorkspaceArtifactBase : Artifact
    {
        public WorkspaceArtifactBase()
        {
            
        }
        protected WorkspaceArtifactBase(ArtifactState state)
            : base(state)
        {

        }

        public WorkspaceArtifactContext? Context { get { return GetResultingContext(); } }

        protected void RaiseContextChanged()
        {
            RaisePropertyChangedEvent(nameof(Context));
            foreach(var child in Children.OfType<WorkspaceArtifactBase>())
            {
                child.RaiseContextChanged();
            }
        }

        private WorkspaceArtifactContext? GetResultingContext()
        {
            var parentContext = (Parent as WorkspaceArtifactBase)?.GetResultingContext();
            var ownContext = GetOwnContext();
            if (parentContext == null && ownContext == null)
                return null;

            if(parentContext == null)
                return ownContext;

            if (ownContext == null)
                return parentContext;

            // Merge contexts
            // First merge parameters
            var namespaceParameters = new Dictionary<string, string>();
            if (parentContext != null)
            {
                foreach(var parameter in parentContext.NamespaceParameters)
                {
                    namespaceParameters.Add(parameter.Key, parameter.Value);
                }
            }
            foreach(var parameter in ownContext.NamespaceParameters) 
            {
                namespaceParameters[parameter.Key] = parameter.Value;
            }
            // Then merge properties
            var mergedContext = new WorkspaceArtifactContext
            {
                NamespaceParameters = new ReadOnlyDictionary<string, string>(namespaceParameters),
                Namespace = ownContext.Namespace ?? parentContext?.Namespace,
                ClassName = ownContext.ClassName ?? parentContext?.ClassName,
                OutputPath = ownContext.OutputPath ?? parentContext?.OutputPath,
                Layer = ownContext.Layer ?? parentContext?.Layer,
                Scope = ownContext.Scope ?? parentContext?.Scope
            };
            return mergedContext;
        }

        /// <summary>
        /// Return null if this artifact doesn't have any contribution to the Context.
        /// Override in derived class to provide context.
        /// </summary>
        protected virtual WorkspaceArtifactContext? GetOwnContext()
        {
            return null;
        }

        protected void PublishArtifactConstructionEvent()
        {
            var messageBus = ServiceProviderHolder.GetRequiredService<WorkspaceMessageBus>();
            
            messageBus.PublishArtifactConstruction( this);
        }

        /// <summary>
        /// Notify any subscribers that the context menu is opening for this artifact.<br />
        /// Subscribers can add custom commands to the context menu via the returned event args.
        /// </summary>
        public ArtifactContextMenuOpeningEventArgs PublishArtifactContextMenuOpeningEvent(List<ArtifactTreeNodeCommand> commands)
        {
            var messageBus = ServiceProviderHolder.GetRequiredService<WorkspaceMessageBus>();

            var eventArgs = messageBus.PublishArtifactContextMenuOpening(this, commands);

            return eventArgs;
        }

        public T EnsureChildArtifactExists<T>(Func<T>? factory = null, Func<T, bool>? predicate = null) where T : WorkspaceArtifactBase
        {
            if(factory == null) factory = () => Activator.CreateInstance<T>();
            if(predicate == null) predicate = (a) => true;
            var childArtifact = Children.OfType<T>().FirstOrDefault(predicate);
            if (childArtifact == null)
            {
                childArtifact = factory();
                AddChild(childArtifact);
            }
            return childArtifact;
        }

        public WorkspaceArtifact GetWorkspace()
        {
            return FindAncesterOfType<WorkspaceArtifact>() 
                ?? throw new InvalidOperationException("Artifact is not part of a workspace.");
        }
    }
}
