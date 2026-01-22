using CodeGenerator.Shared.ViewModels;
using CodeGenerator.TemplateEngines.Scriban;

namespace CodeGenerator.Application.ViewModels.Template;

/// <summary>
/// ViewModel for editing a Scriban template file
/// </summary>
public class ScribanTemplateEditViewModel : ViewModelBase
{
    private string _templateFilePath = string.Empty;
    private string _tabLabel = "Template Editor";
    private ScribanTemplateInstance? _templateInstance;

    /// <summary>
    /// Path to the Scriban template file
    /// </summary>
    public string TemplateFilePath
    {
        get => _templateFilePath;
        set => SetProperty(ref _templateFilePath, value);
    }

    /// <summary>
    /// Label to display on the tab
    /// </summary>
    public string TabLabel
    {
        get => _tabLabel;
        set => SetProperty(ref _tabLabel, value);
    }
        
    public ScribanTemplateInstance? TemplateInstance
    {
        get { return _templateInstance; }
        set { SetProperty(ref _templateInstance, value); }
    }

}
