using CodeGenerator.Core.Templates;
using CodeGenerator.Shared.ViewModels;

namespace CodeGenerator.Application.ViewModels.Template;

/// <summary>
/// ViewModel for editing a single template parameter definition
/// </summary>
public class TemplateParameterEditViewModel : ViewModelBase
{
    private TemplateParameterEditModel? _parameter;
    private bool _isDatasourceType;

    /// <summary>
    /// The parameter being edited
    /// </summary>
    public TemplateParameterEditModel? Parameter
    {
        get => _parameter;
        set
        {
            if (SetProperty(ref _parameter, value))
            {
                UpdateDatasourceTypeFlag();
                OnPropertyChanged(nameof(HasParameter));
            }
        }
    }

    /// <summary>
    /// Whether a parameter is currently selected for editing
    /// </summary>
    public bool HasParameter => _parameter != null;

    /// <summary>
    /// Whether the current parameter is a datasource type (shows additional options)
    /// </summary>
    public bool IsDatasourceType
    {
        get => _isDatasourceType;
        private set => SetProperty(ref _isDatasourceType, value);
    }

    /// <summary>
    /// Available type options for the dropdown
    /// </summary>
    public static List<ParameterTypeOption> AvailableTypes => new()
    {
        new ParameterTypeOption("System.String", "String"),
        new ParameterTypeOption("System.Int32", "Integer (32-bit)"),
        new ParameterTypeOption("System.Int64", "Integer (64-bit)"),
        new ParameterTypeOption("System.Boolean", "Boolean"),
        new ParameterTypeOption("System.DateTime", "DateTime"),
        new ParameterTypeOption("System.DateOnly", "Date"),
        new ParameterTypeOption("System.Decimal", "Decimal"),
        new ParameterTypeOption("System.Double", "Double"),
        new ParameterTypeOption(TemplateParameter.TemplateDatasourceArtifactDataTypeName, "Table Data (from Workspace)")
    };

    /// <summary>
    /// Update the datasource type flag based on the current parameter's type
    /// </summary>
    public void UpdateDatasourceTypeFlag()
    {
        IsDatasourceType = _parameter?.TypeName == TemplateParameter.TemplateDatasourceArtifactDataTypeName;
    }

    /// <summary>
    /// Called when the type selection changes
    /// </summary>
    public void OnTypeChanged(string typeName)
    {
        if (_parameter != null)
        {
            _parameter.TypeName = typeName;
            UpdateDatasourceTypeFlag();
        }
    }
}

/// <summary>
/// Represents a parameter type option for the dropdown
/// </summary>
public class ParameterTypeOption
{
    public string TypeName { get; }
    public string DisplayName { get; }

    public ParameterTypeOption(string typeName, string displayName)
    {
        TypeName = typeName;
        DisplayName = displayName;
    }

    public override string ToString() => DisplayName;
}
