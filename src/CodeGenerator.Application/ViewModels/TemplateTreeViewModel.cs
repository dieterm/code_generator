using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.CodeGeneration;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Templates;
using CodeGenerator.Shared.ViewModels;

namespace CodeGenerator.Application.ViewModels;

/// <summary>
/// ViewModel for the template tree view
/// </summary>
public class TemplateTreeViewModel : ViewModelBase
{
    private RootArtifact? _rootArtifact;
    private IArtifact? _selectedArtifact;
    private string _templateFolder = string.Empty;

    /// <summary>
    /// Root artifact containing the template folder structure
    /// </summary>
    public RootArtifact? RootArtifact
    {
        get => _rootArtifact;
        set => SetProperty(ref _rootArtifact, value);
    }

    /// <summary>
    /// Currently selected artifact in the tree
    /// </summary>
    public IArtifact? SelectedArtifact
    {
        get => _selectedArtifact;
        set => SetProperty(ref _selectedArtifact, value);
    }

    /// <summary>
    /// Path to the template folder
    /// </summary>
    public string TemplateFolder
    {
        get => _templateFolder;
        set => SetProperty(ref _templateFolder, value);
    }

    /// <summary>
    /// Event raised when a template is selected
    /// </summary>
    public event EventHandler<TemplateArtifact>? TemplateSelected;

    /// <summary>
    /// Raises the TemplateSelected event
    /// </summary>
    public void OnTemplateSelected(TemplateArtifact template)
    {
        TemplateSelected?.Invoke(this, template);
    }

    /// <summary>
    /// Event raised when context menu is requested
    /// </summary>
    public event EventHandler<TemplateContextMenuEventArgs>? ContextMenuRequested;

    /// <summary>
    /// Request context menu for an artifact
    /// </summary>
    public void RequestContextMenu(IArtifact artifact, int x, int y)
    {
        ContextMenuRequested?.Invoke(this, new TemplateContextMenuEventArgs(artifact, x, y));
    }
}

/// <summary>
/// Event args for template context menu request
/// </summary>
public class TemplateContextMenuEventArgs : EventArgs
{
    public IArtifact Artifact { get; }
    public int X { get; }
    public int Y { get; }

    public TemplateContextMenuEventArgs(IArtifact artifact, int x, int y)
    {
        Artifact = artifact;
        X = x;
        Y = y;
    }
}
