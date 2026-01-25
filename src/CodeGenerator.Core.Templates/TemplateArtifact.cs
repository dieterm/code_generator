using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Interfaces;
using CodeGenerator.Shared;

namespace CodeGenerator.Core.Templates;

/// <summary>
/// Artifact representing a template file in the template tree view
/// </summary>
public class TemplateArtifact : Artifact
{
    private readonly ITemplate? _template;
    private TemplateDefinition? _definition;

    public TemplateArtifact(string filePath, ITemplateEngine? templateEngine = null)
    {
        FilePath = filePath;
        //FileName = Path.GetFileName(filePath);
        
        // Try to create template from file if engine is provided
        if (templateEngine != null)
        {
            try
            {
                _template = templateEngine.CreateTemplateFromFile(filePath);
            }
            catch
            {
                // Template creation failed, will show as unloadable
                _template = null;
            }
        }

        // Try to load definition file
        _definition = TemplateDefinition.LoadForTemplate(filePath);
    }

    public TemplateArtifact(ArtifactState state) : base(state)
    {
        FilePath = GetValue<string>(nameof(FilePath)) ?? string.Empty;
        _definition = TemplateDefinition.LoadForTemplate(FilePath);
        var fileExtention = Path.GetExtension(FilePath);
        var templateEngineManager = ServiceProviderHolder.GetRequiredService<TemplateEngineManager>();
        var templateEngine = templateEngineManager.GetTemplateEngineByFileExtension(fileExtention);
        _template = templateEngine!.CreateTemplateFromFile(FilePath);
    }

    /// <summary>
    /// Full path to the template file
    /// </summary>
    public string FilePath
    {
        get => GetValue<string>(nameof(FilePath)) ?? string.Empty;
        private set { 
            if (SetValue(nameof(FilePath), value)) { 
                RaisePropertyChangedEvent(nameof(FileName));
                RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }
    }

    /// <summary>
    /// Name of the template file
    /// </summary>
    public string FileName { get { return Path.GetFileName(FilePath); } }

    /// <summary>
    /// The loaded template instance (null if loading failed)
    /// </summary>
    public ITemplate? Template => _template;

    /// <summary>
    /// The template definition (parameters, metadata)
    /// </summary>
    public TemplateDefinition? Definition => _definition;

    /// <summary>
    /// Whether this template has a valid definition file
    /// </summary>
    public bool HasDefinition => _definition != null;

    /// <summary>
    /// Display name from definition or file name
    /// </summary>
    public string DisplayName => _definition?.DisplayName ?? FileName;

    /// <summary>
    /// Description from definition
    /// </summary>
    public string? Description => _definition?.Description;

    /// <summary>
    /// Category from definition
    /// </summary>
    public string? Category => _definition?.Category;

    /// <summary>
    /// Parameters from definition
    /// </summary>
    public IReadOnlyList<TemplateParameter> Parameters => 
        _definition?.Parameters ?? new List<TemplateParameter>();

    public override string TreeNodeText => FileName;

    public override ITreeNodeIcon TreeNodeIcon => Template?.Icon;

    /// <summary>
    /// Creates or gets the definition for this template
    /// </summary>
    public TemplateDefinition GetOrCreateDefinition()
    {
        if (_definition != null)
            return _definition;

        return TemplateDefinition.CreateDefault(FilePath);
    }

    /// <summary>
    /// Save the template definition to disk
    /// </summary>
    public void SaveDefinition(TemplateDefinition definition)
    {
        definition.SaveForTemplate(this);
        _definition = definition;
        RaisePropertyChangedEvent(nameof(Definition));
        RaisePropertyChangedEvent(nameof(Parameters));
        RaisePropertyChangedEvent(nameof(DisplayName));
        RaisePropertyChangedEvent(nameof(Description));
        RaisePropertyChangedEvent(nameof(TreeNodeText));
    }

    /// <summary>
    /// Reload the definition from disk
    /// </summary>
    public void ReloadDefinition()
    {
        _definition = TemplateDefinition.LoadForTemplate(FilePath);
        RaisePropertyChangedEvent(nameof(Definition));
        RaisePropertyChangedEvent(nameof(Parameters));
        RaisePropertyChangedEvent(nameof(HasDefinition));
        RaisePropertyChangedEvent(nameof(DisplayName));
        RaisePropertyChangedEvent(nameof(Description));
        RaisePropertyChangedEvent(nameof(Category));
        RaisePropertyChangedEvent(nameof(TreeNodeText));
    }

    public void RenameTemplate(string templateId)
    {
        var oldFilePath = FilePath;
        var newFilePath = Path.Combine(Path.GetDirectoryName(oldFilePath) ?? string.Empty, templateId + Path.GetExtension(oldFilePath));
        if (File.Exists(newFilePath))
            return;

        if (File.Exists(oldFilePath))
        {
            File.Move(oldFilePath, newFilePath);
            FilePath = newFilePath;
        }
        if (_definition != null)
        {
            _definition.RenameDefinitionFile(oldFilePath, newFilePath);
            RaisePropertyChangedEvent(nameof(Definition));
        }
    }

}