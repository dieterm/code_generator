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
    public abstract class WorkspaceArtifactBase : Artifact, IDisposable
    {
        public event EventHandler? AttachedToWorkspace;
        public event EventHandler? DetachedFromWorkspace;
        private WorkspaceArtifactBase? _observingParent;
        private WorkspaceArtifact? _workspace;

        public WorkspaceArtifactBase()
        {
            ParentChanged += WorkspaceArtifactBase_ParentChanged;
            ChildAdded += WorkspaceArtifactBase_ChildAdded;
        }


        protected WorkspaceArtifactBase(ArtifactState state)
    :       base(state)
        {
            ParentChanged += WorkspaceArtifactBase_ParentChanged;
            ChildAdded += WorkspaceArtifactBase_ChildAdded;
        }
        
        private void WorkspaceArtifactBase_ChildAdded(object? sender, Core.Artifacts.Events.ChildAddedEventArgs e)
        {
            var messageBus = ServiceProviderHolder.GetRequiredService<WorkspaceMessageBus>();
            if (e.ChildArtifact is WorkspaceArtifactBase childArtifact && sender is WorkspaceArtifactBase parentArtifact)
            {
                messageBus.PublishArtifactChildAdded(parentArtifact, childArtifact);
            }
        }
        #region Workspace Access
        public virtual WorkspaceArtifact? Workspace { get { return _workspace; } }
        private void WorkspaceArtifactBase_ParentChanged(object? sender, ParentChangedEventArgs e)
        {
            if(_observingParent != null)
            {
                if(_workspace != null)
                {
                    _observingParent.DetachedFromWorkspace -= ObservingParent_DetachedFromWorkspace;
                    _workspace = null;
                    OnDetachedFromWorkspace();
                }
                else 
                { 
                    _observingParent.AttachedToWorkspace -= ObservingParent_AttachedToWorkspace;
                }
                _observingParent = null;
            }
            if (e.NewParent is WorkspaceArtifactBase observableParent)
            {
                _observingParent = observableParent;
                if(_observingParent.Workspace != null)
                {
                    _workspace = _observingParent.Workspace;
                    _observingParent.DetachedFromWorkspace += ObservingParent_DetachedFromWorkspace;
                    RaisePropertyChangedEvent(nameof(Workspace));
                    OnAttachedToWorkspace();
                } 
                else
                {
                    _observingParent.AttachedToWorkspace += ObservingParent_AttachedToWorkspace;
                }
            } 
        }

        private void ObservingParent_AttachedToWorkspace(object? sender, EventArgs e)
        {
            if(_workspace != _observingParent!.Workspace)
            {
                _workspace = _observingParent.Workspace;
                RaisePropertyChangedEvent(nameof(Workspace));
            }
            if (_workspace != null) { 
                OnAttachedToWorkspace();
                _observingParent.AttachedToWorkspace -= ObservingParent_AttachedToWorkspace;
                _observingParent.DetachedFromWorkspace += ObservingParent_DetachedFromWorkspace;
            }
            else
            {
                // This should not happen, but just in case, we won't attach to workspace if it's null
                throw new InvalidOperationException("Parent reported attached to workspace, but workspace is null.");
            }
        }

        private void ObservingParent_DetachedFromWorkspace(object? sender, EventArgs e)
        {
            _workspace = null;
            OnDetachedFromWorkspace();
            _observingParent!.DetachedFromWorkspace -= ObservingParent_DetachedFromWorkspace;
            _observingParent.AttachedToWorkspace += ObservingParent_AttachedToWorkspace;
        }



        protected void OnAttachedToWorkspace()
        {
            AttachedToWorkspace?.Invoke(this, EventArgs.Empty);
        }

        protected void OnDetachedFromWorkspace()
        {
            DetachedFromWorkspace?.Invoke(this, EventArgs.Empty);
        }
        
        #endregion

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

        public void Dispose()
        {
            ParentChanged -= WorkspaceArtifactBase_ParentChanged;
            ChildAdded -= WorkspaceArtifactBase_ChildAdded;
        }
    }
}
