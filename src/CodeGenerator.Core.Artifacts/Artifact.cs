using CodeGenerator.Core.Artifacts.TreeNode;

namespace CodeGenerator.Core.Artifacts;

/// <summary>
/// Represents any generated artifact in the system.
/// The type and behavior of the artifact is determined by its decorators.
/// </summary>
public abstract class Artifact : IArtifact
{
    public abstract string Id { get; }
    public IArtifact? Parent { get; private set; }
    private readonly List<IArtifact> _children = new();
    public IEnumerable<IArtifact> Children { get { return _children; } } 
    public ArtifactDecoratorCollection Decorators { get; } = new();

    public abstract string TreeNodeText { get; }
    public abstract ITreeNodeIcon TreeNodeIcon { get; }
    /// <summary>
    /// Arbitrary metadata/properties for the artifact and it's decorators<br />
    /// Artifact Property Key: "{PropertyName}"<br />
    /// Decorator Property Key: "{DecoratorKey}.{PropertyName}"
    /// </summary>
    public Dictionary<string, object?> Properties { get; } = new();

    /// <summary>
    /// Materialize the artifact and child-artifacts (generate its content, files, etc.)
    /// </summary>
    public virtual async Task GenerateAsync(IProgress<ArtifactGenerationProgress> progress, CancellationToken cancellationToken = default)
    {
        int totalSteps = 1 + _children.Count;
        int currentStep = 0;

        progress.Report(new ArtifactGenerationProgress(this, "Generating artifact", currentStep++, totalSteps));
        await GenerateSelfAsync(progress, cancellationToken);

        foreach (var child in _children)
        {
            progress.Report(new ArtifactGenerationProgress(this, "Generating child artifact", currentStep++, totalSteps));
            await child.GenerateAsync(progress, cancellationToken);
        }

        progress.Report(new ArtifactGenerationProgress(this, "Generated artifact", currentStep, totalSteps));
    }
    /// <summary>
    /// Materialize the artifact (generate its content, files, etc.)
    /// </summary>
    public virtual async Task GenerateSelfAsync(IProgress<ArtifactGenerationProgress> progress, CancellationToken cancellationToken = default)
    {
        foreach (var decorator in Decorators.Where(d => d.CanGenerate()))
        {
            progress.Report(new ArtifactGenerationProgress(this, $"Generating artifact via decorator '{decorator.Key}'", 0, 1));
            await decorator.GenerateAsync(progress, cancellationToken);
        }
    }


    public void AddChild(IArtifact child)
    {
        ((Artifact)child).Parent = this;
        _children.Add(child);
    }

    public void RemoveChild(IArtifact child)
    {
        if (_children.Remove(child))
        {
            ((Artifact)child).Parent = null;
        }
    }

    public T AddDecorator<T>(T decorator) where T : IArtifactDecorator
    {
        decorator.Attach(this);
        Decorators.Add(decorator);
        return decorator;
    }

    public T AddOrGetDecorator<T>(Func<T> decoratorFactory) where T : class, IArtifactDecorator
    {
        var decorator = GetDecorator<T>();
        if (decorator == null)
        {
            decorator = decoratorFactory();
            AddDecorator(decorator);
        }
        return decorator;
    }

    public void RemoveDecorator(IArtifactDecorator decorator)
    {
        if (Decorators.Remove(decorator))
        {
            decorator.Detach();
        }
    }

    /// <summary>
    /// Get a decorator of a specific type
    /// </summary>
    public T? GetDecorator<T>() where T : class, IArtifactDecorator
    {
        return Decorators.OfType<T>().FirstOrDefault();
    }

    /// <summary>
    /// Get all decorators of a specific type
    /// </summary>
    public IEnumerable<T> GetDecorators<T>() where T : class, IArtifactDecorator
    {
        return Decorators.OfType<T>();
    }

    /// <summary>
    /// Check if the artifact has a specific decorator type
    /// </summary>
    public bool HasDecorator<T>() where T : class, IArtifactDecorator
    {
        return Decorators.OfType<T>().Any();
    }

    /// <summary>
    /// Check if this artifact represents a specific type (has the decorator)
    /// </summary>
    public bool Is<T>() where T : class, IArtifactDecorator => HasDecorator<T>();

    /// <summary>
    /// Get a property value for this artifact
    /// </summary>
    public T? GetProperty<T>(string name)
    {
        if (Properties.TryGetValue(name, out var value) && value is T typed)
            return typed;
        return default;
    }
    /// <summary>
    /// Set a property value for this artifact
    /// </summary>
    public IArtifact SetProperty(string name, object? value)
    {
        Properties[name] = value;
        return this;
    }

    /// <summary>
    /// Get a property value for a decorator
    /// </summary>
    public T? GetProperty<T>(IArtifactDecorator decorator, string name)
    {
        if (Properties.TryGetValue($"{decorator.Key}.{name}", out var value) && value is T typed)
            return typed;
        return default;
    }

    /// <summary>
    /// Set a property value for a decorator
    /// </summary>
    public IArtifact SetProperty(IArtifactDecorator decorator, string name, object? value)
    {
        Properties[$"{decorator.Key}.{name}"] = value;
        return this;
    }

    public T? FindAncestor<T>() where T : class, IArtifactDecorator
    {
        var current = Parent;
        while (current != null)
        {
            var decorator = current.GetDecorator<T>();
            if (decorator != null)
                return decorator;
            current = current.Parent;
        }
        return null;
    }

    public IArtifact? FindAncestorArtifact<T>() where T : class, IArtifactDecorator
    {
        var current = Parent;
        while (current != null)
        {
            if (current.HasDecorator<T>())
                return current;
            current = current.Parent;
        }
        return null;
    }

    public IEnumerable<T> FindDescendantDecorators<T>() where T : class, IArtifactDecorator
    {
        foreach (var child in Children)
        {
            var decorator = child.GetDecorator<T>();
            if (decorator != null)
                yield return decorator;

            foreach (var descendant in child.FindDescendantDecorators<T>())
                yield return descendant;
        }
    }

    public IEnumerable<IArtifact> FindDescendants<T>() where T : class, IArtifactDecorator
    {
        foreach (var child in Children)
        {
            if (child.HasDecorator<T>())
                yield return child;

            foreach (var descendant in child.FindDescendants<T>())
                yield return descendant;
        }
    }

    public IEnumerable<IArtifact> GetAllDescendants()
    {
        foreach (var child in Children)
        {
            yield return child;
            foreach (var descendant in child.GetAllDescendants())
                yield return descendant;
        }
    }

    /// <summary>
    /// Create preview if any decorator supports it
    /// </summary>
    public object? CreatePreview()
    {
        foreach (var decorator in Decorators.OfType<IPreviewableDecorator>())
        {
            var preview = decorator.CreatePreview();
            if (preview != null)
                return preview;
        }
        return null;
    }

    /// <summary>
    /// Check if this artifact can be previewed
    /// </summary>
    public bool CanPreview => Decorators.OfType<IPreviewableDecorator>().Any(d => d.CanPreview);
}
