using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Controllers.Template;
using CodeGenerator.Application.ViewModels.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.CodeGeneration;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Templates;

namespace CodeGenerator.Application.ViewModels.Template;

/// <summary>
/// ViewModel for the template tree view
/// </summary>
public class TemplateTreeViewModel : ArtifactTreeViewModel<TemplateTreeViewController>
{
    private string _templateFolder = string.Empty;

    public TemplateTreeViewModel(TemplateTreeViewController treeViewController) 
        : base(treeViewController)
    {
    }

    ///// <summary>
    ///// Path to the template folder
    ///// </summary>
    //public string TemplateFolder
    //{
    //    get => _templateFolder;
    //    set => SetProperty(ref _templateFolder, value);
    //}

    /// <summary>
    /// Event raised when a template is selected
    /// </summary>
    public event EventHandler<TemplateArtifact>? TemplateSelected;

    /// <summary>
    /// Event raised when context menu commands are needed
    /// </summary>
    public event Func<IArtifact, IEnumerable<ArtifactTreeNodeCommand>>? GetContextMenuCommandsRequested;

    /// <summary>
    /// Event raised when double-click handling is needed
    /// </summary>
    public event Func<IArtifact, CancellationToken, Task>? DoubleClickRequested;

    /// <summary>
    /// Raises the TemplateSelected event
    /// </summary>
    public void OnTemplateSelected(TemplateArtifact template)
    {
        TemplateSelected?.Invoke(this, template);
    }

    /// <summary>
    /// Get context menu commands for an artifact
    /// </summary>
    public override IEnumerable<ArtifactTreeNodeCommand> GetContextMenuCommands(IArtifact artifact)
    {
        // Default commands if no controller provides them
        return TreeViewController.GetContextMenuCommands(artifact);
    }

    /// <summary>
    /// Handle artifact double-click
    /// </summary>
    public override async Task HandleDoubleClickAsync(IArtifact artifact, CancellationToken cancellationToken = default)
    {
        if (DoubleClickRequested != null)
        {
            await DoubleClickRequested.Invoke(artifact, cancellationToken);
        }
        else if (artifact is TemplateArtifact templateArtifact)
        {
            OnTemplateSelected(templateArtifact);
        }
    }

}
