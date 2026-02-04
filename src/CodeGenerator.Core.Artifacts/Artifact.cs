using CodeGenerator.Core.Artifacts.Events;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.Memento;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace CodeGenerator.Core.Artifacts;

/// <summary>
/// Represents any generated artifact in the system.
/// The type and behavior of the artifact is determined by its decorators.
/// </summary>
public abstract class Artifact : MementoObjectBase<ArtifactState>, IArtifact
{
    public event EventHandler<ParentChangedEventArgs>? ParentChanged;
    public event EventHandler<ChildAddedEventArgs>? ChildAdded;
    public event EventHandler<ChildRemovedEventArgs>? ChildRemoved;
    public event EventHandler<DecoratorAddedEventArgs>? DecoratorAdded;
    public event EventHandler<DecoratorRemovedEventArgs>? DecoratorRemoved;
    public event EventHandler? Generated;
    protected void OnChildAdded(IArtifact child)
    {
        ChildAdded?.Invoke(this, new ChildAddedEventArgs(child));
    }
    
    protected void OnChildRemoved(IArtifact child)
    {
        ChildRemoved?.Invoke(this, new ChildRemovedEventArgs(child));
    }
    protected void OnGenerated()
    {
        Generated?.Invoke(this, EventArgs.Empty);
    }

    protected void OnDecoratorAdded(IArtifactDecorator decorator)
    {
        DecoratorAdded?.Invoke(this, new DecoratorAddedEventArgs(decorator));
    }

    protected void OnDecoratorRemoved(IArtifactDecorator decorator)
    {
        DecoratorRemoved?.Invoke(this, new DecoratorRemovedEventArgs(decorator));
    }

    protected void OnParentChanged(IArtifact? oldParent, IArtifact? newParent)
    {
        ParentChanged?.Invoke(this, new ParentChangedEventArgs(oldParent, newParent));
    }

    public string Id { 
        get { return GetValue<string>(nameof(Id)); } 
        protected set { SetValue(nameof(Id), value); }
    }
    private IArtifact? _parent;
    public IArtifact? Parent {
        get { return _parent; }
        private set { 
            if (_parent != value)
            {
                var oldParent = _parent;
                _parent = value;
                OnParentChanged(oldParent, _parent);
            }
        }
    }
    private readonly List<IArtifact> _children = new();

    protected Artifact(ArtifactState state) 
        : base(state) // will call RestoreState, which is overridden in this class below
    {

    }

    public override void RestoreState(IMementoState state)
    {
        base.RestoreState(state);
        var artifactState = (ArtifactState)state;
        // Initialize decorators
        foreach (var decoratorState in artifactState.Decorators)
        {
            var decorator = ArtifactDecoratorFactory.CreateArtifactDecorator(decoratorState);
            AddDecorator(decorator);
        }
        // Initialize children
        foreach (var childState in artifactState.Children)
        {
            var childArtifact = ArtifactFactory.CreateArtifact(childState);
            AddChild(childArtifact);
        }
    }

    protected Artifact()
    {
        Id = Guid.NewGuid().ToString();
    }

    public IEnumerable<IArtifact> Children { get { return _children; } } 
    private readonly ArtifactDecoratorCollection _decorators = new ArtifactDecoratorCollection();
    public IEnumerable<IArtifactDecorator> Decorators { get { return _decorators; } }

    /// <inheritdoc/>
    public abstract string TreeNodeText { get; }
    /// <inheritdoc/>
    public abstract ITreeNodeIcon TreeNodeIcon { get; }

    /// <inheritdoc/>
    public virtual Color? TreeNodeTextColor => null;

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
            if(cancellationToken.IsCancellationRequested)
                break;
            if(currentStep%10 == 0)
            {
                // Report progress every 10 steps to avoid flooding
                progress.Report(new ArtifactGenerationProgress(this, "Generating child artifacts", currentStep, totalSteps));
            }
            await child.GenerateAsync(progress, cancellationToken);
        }

        // raise Generated-event
        OnGenerated();

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

    public virtual T AddChild<T>(T child) where T : class,IArtifact
    {
        ((Artifact)(object)child).Parent = this;
        _children.Add(child);
        OnChildAdded(child);
        return child;
    }

    public virtual void RemoveChild(IArtifact child)
    {
        if (_children.Remove(child))
        {
            ((Artifact)child).Parent = null;
            OnChildRemoved(child);
        }
    }

    public virtual T AddDecorator<T>(T decorator) where T : IArtifactDecorator
    {
        if(decorator.Artifact!=null && decorator.Artifact != this)
        {
            throw new InvalidOperationException("Decorator is already attached to another artifact.");
        }
        decorator.Attach(this);
        _decorators.Add(decorator);
        OnDecoratorAdded(decorator);
        return decorator;
    }

    public T AddOrGetDecorator<T>(Func<T> decoratorFactory) where T : class, IArtifactDecorator
    {
        var decorator = GetDecoratorOfType<T>();
        if (decorator == null)
        {
            decorator = decoratorFactory();
            AddDecorator(decorator);
        }
        return decorator;
    }

    public virtual void RemoveDecorator(IArtifactDecorator decorator)
    {
        if (_decorators.Remove(decorator))
        {
            decorator.Detach();
            OnDecoratorRemoved(decorator);
        } 
        else if(decorator.Artifact == this)
        {
            throw new InvalidOperationException("Decorator was attached to this artifact but not found in the decorators collection.");
        }
        else
        {
            throw new InvalidOperationException("Decorator is not attached to this artifact.");
        }
    }

    /// <summary>
    /// Get a decorator of a specific type
    /// </summary>
    public T? GetDecoratorOfType<T>() where T : class, IArtifactDecorator
    {
        return Decorators.OfType<T>().FirstOrDefault();
    }
    public T? GetDecoratorLikeType<T>() where T : class, IArtifactDecorator
    {
        return Decorators.Where(d => d is T).FirstOrDefault() as T;
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

    public T? FindAncesterOfType<T>() where T : class, IArtifact
    {
        var current = Parent;
        while (current != null)
        {
            var artifact = current as T;
            if (artifact != null)
                return artifact;
            current = current.Parent;
        }
        return null;
    }

    public T? FindAncestor<T>() where T : class, IArtifactDecorator
    {
        var current = Parent;
        while (current != null)
        {
            var decorator = current.GetDecoratorOfType<T>();
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
            var decorator = child.GetDecoratorOfType<T>();
            if (decorator != null)
                yield return decorator;

            foreach (var descendant in child.FindDescendantDecorators<T>())
                yield return descendant;
        }
    }

    //public IEnumerable<IArtifact> FindDescendantDecorators<T>() where T : class, IArtifactDecorator
    //{
    //    foreach (var child in Children)
    //    {
    //        if (child.HasDecorator<T>())
    //            yield return child;

    //        foreach (var descendant in child.FindDescendantDecorators<T>())
    //            yield return descendant;
    //    }
    //}

    public IEnumerable<T> FindDescendants<T>() where T : class, IArtifact
    {
        foreach (var child in Children)
        {
            if (child is T childAsT)
                yield return childAsT;

            foreach (var descendant in child.FindDescendants<T>())
                yield return descendant;
        }
    }
    public T? FindDescendantById<T>(string? id) where T: class, IArtifact
    {
        if (string.IsNullOrWhiteSpace(id)) return null;
        return GetAllDescendants().FirstOrDefault(d => d.Id == id) as T;
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



    public override IMementoState CaptureState()
    {
        var state = (ArtifactState)base.CaptureState();
        // Capture decorators' states
        foreach (var decorator in Decorators)
        {
            var decoratorState = decorator.CaptureState();
            state.Decorators.Add((ArtifactDecoratorState)decoratorState);
        }
        // capture children states
        state.Children.AddRange(Children.Select(c => (ArtifactState)c.CaptureState()));
        return state;
    }

    public override string ToString()
    {
        return TreeNodeText;
    }

}
