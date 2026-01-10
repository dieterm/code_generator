using CodeGenerator.Core.Artifacts.TreeNode;
using System;
using System.Collections.Generic;
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

        public ArtifactDecoratorCollection Decorators { get { return Artifact.Decorators; } }

        public string Id { get { return Artifact.Id; } }

        public IArtifact? Parent { get { return Artifact.Parent; } }

        public Dictionary<string, object?> Properties { get { return Artifact.Properties; } }

        public virtual ITreeNodeIcon TreeNodeIcon { get { return Artifact.TreeNodeIcon; } }

        public virtual string TreeNodeText { get { return Artifact.TreeNodeText; } }
        public ArtifactHost(Artifact artifact)
        {
            if(artifact == null) throw new ArgumentNullException(nameof(artifact));

            Artifact = artifact;
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

        public T? GetProperty<T>(IArtifactDecorator decorator, string name)
        {
            return Artifact.GetProperty<T>(decorator, name);
        }

        public T? GetProperty<T>(string name)
        {
            return Artifact.GetProperty<T>(name);
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

        public IArtifact SetProperty(IArtifactDecorator decorator, string name, object? value)
        {
            return ((IArtifact)Artifact).SetProperty(decorator, name, value);
        }

        public IArtifact SetProperty(string name, object? value)
        {
            return ((IArtifact)Artifact).SetProperty(name, value);
        }
    }
}
