using CodeGenerator.Core.Artifacts.Events;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Memento;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts
{
    public abstract class ArtifactHost : IArtifact
    {
        public Artifact Artifact { get; }

        public bool CanPreview { get {return Artifact.CanPreview; } }

        public IEnumerable<IArtifact> Children { get { return Artifact.Children; } }

        public IEnumerable<IArtifactDecorator> Decorators { get { return Artifact.Decorators; } }

        public string Id { get { return Artifact.Id; } }

        public IArtifact? Parent { get { return Artifact.Parent; } }

        public virtual ITreeNodeIcon TreeNodeIcon { get { return Artifact.TreeNodeIcon; } }

        public virtual string TreeNodeText { get { return Artifact.TreeNodeText; } }

        public bool IsStateChanged => ((IMementoObject)Artifact).IsStateChanged;

        public Dictionary<string, object?> Properties => ((IMementoObject)Artifact).Properties;

        public ArtifactHost(Artifact artifact)
        {
            if(artifact == null) throw new ArgumentNullException(nameof(artifact));

            Artifact = artifact;
        }

        public event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                ((IMementoObject)Artifact).PropertyChanged += value;
            }

            remove
            {
                ((IMementoObject)Artifact).PropertyChanged -= value;
            }
        }

        public event PropertyChangingEventHandler? PropertyChanging
        {
            add
            {
                ((IMementoObject)Artifact).PropertyChanging += value;
            }

            remove
            {
                ((IMementoObject)Artifact).PropertyChanging -= value;
            }
        }

        public event EventHandler<ParentChangedEventArgs>? ParentChanged
        {
            add
            {
                ((IArtifact)Artifact).ParentChanged += value;
            }

            remove
            {
                ((IArtifact)Artifact).ParentChanged -= value;
            }
        }

        public event EventHandler<ChildAddedEventArgs>? ChildAdded
        {
            add
            {
                ((IArtifact)Artifact).ChildAdded += value;
            }

            remove
            {
                ((IArtifact)Artifact).ChildAdded -= value;
            }
        }

        public event EventHandler<ChildRemovedEventArgs>? ChildRemoved
        {
            add
            {
                ((IArtifact)Artifact).ChildRemoved += value;
            }

            remove
            {
                ((IArtifact)Artifact).ChildRemoved -= value;
            }
        }

        public event EventHandler<DecoratorAddedEventArgs>? DecoratorAdded
        {
            add
            {
                ((IArtifact)Artifact).DecoratorAdded += value;
            }

            remove
            {
                ((IArtifact)Artifact).DecoratorAdded -= value;
            }
        }

        public event EventHandler<DecoratorRemovedEventArgs>? DecoratorRemoved
        {
            add
            {
                ((IArtifact)Artifact).DecoratorRemoved += value;
            }

            remove
            {
                ((IArtifact)Artifact).DecoratorRemoved -= value;
            }
        }

        public void AddChild(IArtifact child)
        {
            Artifact.AddChild(child);
        }

        public T AddDecorator<T>(T decorator) where T : IArtifactDecorator
        {
            return Artifact.AddDecorator<T>(decorator);
        }

        T IArtifact.AddOrGetDecorator<T>(Func<T> decoratorFactory)
        {
            return ((IArtifact)Artifact).AddOrGetDecorator<T>(decoratorFactory);
        }

        public object? CreatePreview()
        {
            return Artifact.CreatePreview();
        }

        T? IArtifact.FindAncestor<T>() where T : class
        {
            return ((IArtifact)Artifact).FindAncestor<T>();
        }

        IArtifact? IArtifact.FindAncestorArtifact<T>()
        {
            return ((IArtifact)Artifact).FindAncestorArtifact<T>();
        }

        IEnumerable<T> IArtifact.FindDescendantDecorators<T>()
        {
            return ((IArtifact)Artifact).FindDescendantDecorators<T>();
        }

        IEnumerable<IArtifact> IArtifact.FindDescendants<T>()
        {
            return ((IArtifact)Artifact).FindDescendants<T>();
        }

        public Task GenerateAsync(IProgress<ArtifactGenerationProgress> progress, CancellationToken cancellationToken = default)
        {
            return Artifact.GenerateAsync(progress, cancellationToken);
        }

        public Task GenerateSelfAsync(IProgress<ArtifactGenerationProgress> progress, CancellationToken cancellationToken = default)
        {
            return Artifact.GenerateSelfAsync(progress, cancellationToken);
        }

        public IEnumerable<IArtifact> GetAllDescendants()
        {
            return Artifact.GetAllDescendants();
        }

        T? IArtifact.GetDecorator<T>() where T : class
        {
            return ((IArtifact)Artifact).GetDecorator<T>();
        }

        IEnumerable<T> IArtifact.GetDecorators<T>()
        {
            return ((IArtifact)Artifact).GetDecorators<T>();
        }

        bool IArtifact.HasDecorator<T>()
        {
            return ((IArtifact)Artifact).HasDecorator<T>();
        }

        bool IArtifact.Is<T>()
        {
            return ((IArtifact)Artifact).Is<T>();
        }

        public void RemoveChild(IArtifact child)
        {
            ((IArtifact)Artifact).RemoveChild(child);
        }

        public void RemoveDecorator(IArtifactDecorator decorator)
        {
            ((IArtifact)Artifact).RemoveDecorator(decorator);
        }

        public T? GetValue<T>(string name, T? defaultValue = default)
        {
            return ((IMementoObject)Artifact).GetValue(name, defaultValue);
        }

        public bool SetValue<T>(string name, T? value)
        {
            return ((IMementoObject)Artifact).SetValue(name, value);
        }

        public void RestoreState(IMementoState state)
        {
            ((IMementoObject)Artifact).RestoreState(state);
        }

        public IMementoState CaptureState()
        {
            return ((IMementoObject)Artifact).CaptureState();
        }
    }
}
