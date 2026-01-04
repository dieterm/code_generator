namespace CodeGenerator.Core.Artifacts;

/// <summary>
/// Represents any generated artifact in the system.
/// The type and behavior of the artifact is determined by its decorators.
/// </summary>
public abstract class Artifact
{
    //public string Name { get; set; } = string.Empty;
    public Guid Id { get; } = Guid.NewGuid();
    public Artifact? Parent { get; private set; }
    public List<Artifact> Children { get; } = new();
    public ArtifactDecoratorCollection Decorators { get; } = new();

    /// <summary>
    /// Arbitrary metadata/properties for the artifact
    /// </summary>
    public Dictionary<string, object?> Properties { get; } = new();
    /// <summary>
    /// Materialize the artifact (generate its content, files, etc.)
    /// </summary>
    public async Task GenerateAsync(CancellationToken cancellationToken = default)
    {
        await GenerateSelfAsync(cancellationToken);
        foreach (var child in Children)
        {
            await child.GenerateAsync(cancellationToken);
        }
    }

    public abstract Task GenerateSelfAsync(CancellationToken cancellationToken = default);
    

    public void AddChild(Artifact child)
    {
        child.Parent = this;
        Children.Add(child);
    }

    public void RemoveChild(Artifact child)
    {
        if (Children.Remove(child))
        {
            child.Parent = null;
        }
    }

    public void AddDecorator(IArtifactDecorator decorator)
    {
        decorator.Attach(this);
        Decorators.Add(decorator);
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
    public Artifact SetProperty(string name, object? value)
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
    public Artifact SetProperty(IArtifactDecorator decorator, string name, object? value)
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

    public Artifact? FindAncestorArtifact<T>() where T : class, IArtifactDecorator
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

    public IEnumerable<Artifact> FindDescendants<T>() where T : class, IArtifactDecorator
    {
        foreach (var child in Children)
        {
            if (child.HasDecorator<T>())
                yield return child;

            foreach (var descendant in child.FindDescendants<T>())
                yield return descendant;
        }
    }

    public IEnumerable<Artifact> GetAllDescendants()
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
