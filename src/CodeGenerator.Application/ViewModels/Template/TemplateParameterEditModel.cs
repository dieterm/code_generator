using CodeGenerator.Core.Templates;
using CodeGenerator.Shared.ViewModels;

namespace CodeGenerator.Application.ViewModels.Template;

/// <summary>
/// Edit model for a template parameter definition
/// </summary>
public class TemplateParameterEditModel : ViewModelBase
{
    private string _name = string.Empty;
    private string? _description;
    private string _typeName = "System.String";
    private bool _required;
    private string? _defaultValue;
    private string? _allowedValuesText;
    private string? _label;
    private string? _tooltip;
    private int _displayOrder;
    private string? _tableDataFilter;
    private int? _tableDataMaxRows;

    /// <summary>
    /// Parameter name (used as key in template)
    /// </summary>
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    /// <summary>
    /// Description of the parameter
    /// </summary>
    public string? Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    /// <summary>
    /// Type name (e.g., "System.String", "System.Int32", "System.Boolean")
    /// </summary>
    public string TypeName
    {
        get => _typeName;
        set => SetProperty(ref _typeName, value);
    }

    /// <summary>
    /// Whether this parameter is required
    /// </summary>
    public bool Required
    {
        get => _required;
        set => SetProperty(ref _required, value);
    }

    /// <summary>
    /// Default value for the parameter
    /// </summary>
    public string? DefaultValue
    {
        get => _defaultValue;
        set => SetProperty(ref _defaultValue, value);
    }

    /// <summary>
    /// Allowed values as semicolon-separated text
    /// </summary>
    public string? AllowedValuesText
    {
        get => _allowedValuesText;
        set => SetProperty(ref _allowedValuesText, value);
    }

    /// <summary>
    /// Display label for the parameter
    /// </summary>
    public string? Label
    {
        get => _label;
        set => SetProperty(ref _label, value);
    }

    /// <summary>
    /// Tooltip for the parameter
    /// </summary>
    public string? Tooltip
    {
        get => _tooltip;
        set => SetProperty(ref _tooltip, value);
    }

    /// <summary>
    /// Display order
    /// </summary>
    public int DisplayOrder
    {
        get => _displayOrder;
        set => SetProperty(ref _displayOrder, value);
    }

    /// <summary>
    /// SQL WHERE clause filter for TableArtifactData
    /// </summary>
    public string? TableDataFilter
    {
        get => _tableDataFilter;
        set => SetProperty(ref _tableDataFilter, value);
    }

    /// <summary>
    /// Maximum rows to load for TableArtifactData
    /// </summary>
    public int? TableDataMaxRows
    {
        get => _tableDataMaxRows;
        set => SetProperty(ref _tableDataMaxRows, value);
    }

    /// <summary>
    /// Available type options for the dropdown
    /// </summary>
    public static List<string> AvailableTypes => new()
    {
        "System.String",
        "System.Int32",
        "System.Int64",
        "System.Boolean",
        "System.DateTime",
        "System.DateOnly",
        "System.Decimal",
        "System.Double",
        TemplateParameter.TemplateDatasourceArtifactDataTypeName
    };

    /// <summary>
    /// Convert to TemplateParameter
    /// </summary>
    public TemplateParameter ToTemplateParameter()
    {
        return new TemplateParameter
        {
            Name = Name,
            Description = Description,
            FullyQualifiedAssemblyTypeName = TypeName,
            Required = Required,
            DefaultValue = DefaultValue,
            AllowedValues = string.IsNullOrWhiteSpace(AllowedValuesText) 
                ? null 
                : AllowedValuesText.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList(),
            Label = Label,
            Tooltip = Tooltip,
            DisplayOrder = DisplayOrder,
            TableDataFilter = TableDataFilter,
            TableDataMaxRows = TableDataMaxRows
        };
    }

    /// <summary>
    /// Create from TemplateParameter
    /// </summary>
    public static TemplateParameterEditModel FromTemplateParameter(TemplateParameter param)
    {
        return new TemplateParameterEditModel
        {
            Name = param.Name,
            Description = param.Description,
            TypeName = param.FullyQualifiedAssemblyTypeName,
            Required = param.Required,
            DefaultValue = param.DefaultValue,
            AllowedValuesText = param.AllowedValues != null ? string.Join("; ", param.AllowedValues) : null,
            Label = param.Label,
            Tooltip = param.Tooltip,
            DisplayOrder = param.DisplayOrder,
            TableDataFilter = param.TableDataFilter,
            TableDataMaxRows = param.TableDataMaxRows
        };
    }
}
