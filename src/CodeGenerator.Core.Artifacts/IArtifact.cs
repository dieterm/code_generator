
using CodeGenerator.Core.Artifacts.Events;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Memento;
using CodeGenerator.Shared.Views.TreeNode;
using System.ComponentModel;

namespace CodeGenerator.Core.Artifacts
{
    public interface IArtifact : IMementoObject, ITreeNode
    {
        event EventHandler<ParentChangedEventArgs>? ParentChanged;
        event EventHandler<ChildAddedEventArgs>? ChildAdded;
        event EventHandler<ChildRemovedEventArgs>? ChildRemoved;
        event EventHandler<DecoratorAddedEventArgs>? DecoratorAdded;
        event EventHandler<DecoratorRemovedEventArgs>? DecoratorRemoved;
        bool CanPreview { get; }
        IEnumerable<IArtifact> Children { get; }
        IEnumerable<IArtifactDecorator> Decorators { get; }
        string Id { get; }
        IArtifact? Parent { get; }
        //ITreeNodeIcon TreeNodeIcon { get; }
        //string TreeNodeText { get; }

        void AddChild(IArtifact child);
        T AddDecorator<T>(T decorator) where T : IArtifactDecorator;
        T AddOrGetDecorator<T>(Func<T> decoratorFactory) where T : class, IArtifactDecorator;
        object? CreatePreview();
        T? FindAncestor<T>() where T : class, IArtifactDecorator;
        IArtifact? FindAncestorArtifact<T>() where T : class, IArtifactDecorator;
        IEnumerable<T> FindDescendantDecorators<T>() where T : class, IArtifactDecorator;
        IEnumerable<T> FindDescendants<T>() where T : class, IArtifact;
        Task GenerateAsync(IProgress<ArtifactGenerationProgress> progress, CancellationToken cancellationToken = default);
        Task GenerateSelfAsync(IProgress<ArtifactGenerationProgress> progress, CancellationToken cancellationToken = default);
        IEnumerable<IArtifact> GetAllDescendants();
        T? GetDecoratorOfType<T>() where T : class, IArtifactDecorator;
        T? GetDecoratorLikeType<T>() where T : class, IArtifactDecorator;
        IEnumerable<T> GetDecorators<T>() where T : class, IArtifactDecorator;
        bool HasDecorator<T>() where T : class, IArtifactDecorator;
        bool Is<T>() where T : class, IArtifactDecorator;
        void RemoveChild(IArtifact child);
        void RemoveDecorator(IArtifactDecorator decorator);
    }
}