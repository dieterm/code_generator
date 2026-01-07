
using CodeGenerator.Core.Artifacts.TreeNode;

namespace CodeGenerator.Core.Artifacts
{
    public interface IArtifact
    {
        bool CanPreview { get; }
        List<Artifact> Children { get; }
        ArtifactDecoratorCollection Decorators { get; }
        string Id { get; }
        Artifact? Parent { get; }
        Dictionary<string, object?> Properties { get; }
        ITreeNodeIcon TreeNodeIcon { get; }
        string TreeNodeText { get; }

        void AddChild(Artifact child);
        T AddDecorator<T>(T decorator) where T : IArtifactDecorator;
        T AddOrGetDecorator<T>(Func<T> decoratorFactory) where T : class, IArtifactDecorator;
        object? CreatePreview();
        T? FindAncestor<T>() where T : class, IArtifactDecorator;
        Artifact? FindAncestorArtifact<T>() where T : class, IArtifactDecorator;
        IEnumerable<T> FindDescendantDecorators<T>() where T : class, IArtifactDecorator;
        IEnumerable<Artifact> FindDescendants<T>() where T : class, IArtifactDecorator;
        Task GenerateAsync(IProgress<ArtifactGenerationProgress> progress, CancellationToken cancellationToken = default);
        Task GenerateSelfAsync(IProgress<ArtifactGenerationProgress> progress, CancellationToken cancellationToken = default);
        IEnumerable<Artifact> GetAllDescendants();
        T? GetDecorator<T>() where T : class, IArtifactDecorator;
        IEnumerable<T> GetDecorators<T>() where T : class, IArtifactDecorator;
        T? GetProperty<T>(IArtifactDecorator decorator, string name);
        T? GetProperty<T>(string name);
        bool HasDecorator<T>() where T : class, IArtifactDecorator;
        bool Is<T>() where T : class, IArtifactDecorator;
        void RemoveChild(Artifact child);
        void RemoveDecorator(IArtifactDecorator decorator);
        Artifact SetProperty(IArtifactDecorator decorator, string name, object? value);
        Artifact SetProperty(string name, object? value);
    }
}