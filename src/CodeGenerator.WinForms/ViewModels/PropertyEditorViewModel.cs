using CodeGenerator.Core.Models.Schema;
using CodeGenerator.Shared.ViewModels;

namespace CodeGenerator.WinForms.ViewModels;

/// <summary>
/// ViewModel for PropertyEditorForm
/// </summary>
public class PropertyEditorViewModel : ValidationViewModelBase
{
    private string _propertyName = string.Empty;
    private string _type = "string";
    private string _format = string.Empty;
    private string _description = string.Empty;
    private bool _isRequired;
    private bool _isNullable;
    private int _minLength;
    private int _maxLength;
    private string _pattern = string.Empty;
    private string _defaultValue = string.Empty;

    // Database settings
    private string _columnName = string.Empty;
    private string _columnType = string.Empty;
    private bool _isPrimaryKey;
    private bool _isIdentity;
    private bool _isForeignKey;

    // Display settings
    private string _label = string.Empty;
    private string _controlType = "(Auto)";

    public bool IsEditMode { get; }

    public string PropertyName
    {
        get => _propertyName;
        set => SetProperty(ref _propertyName, value);
    }

    public string Type
    {
        get => _type;
        set => SetProperty(ref _type, value);
    }

    public string Format
    {
        get => _format;
        set => SetProperty(ref _format, value);
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public bool IsRequired
    {
        get => _isRequired;
        set => SetProperty(ref _isRequired, value);
    }

    public bool IsNullable
    {
        get => _isNullable;
        set => SetProperty(ref _isNullable, value);
    }

    public int MinLength
    {
        get => _minLength;
        set => SetProperty(ref _minLength, value);
    }

    public int MaxLength
    {
        get => _maxLength;
        set => SetProperty(ref _maxLength, value);
    }

    public string Pattern
    {
        get => _pattern;
        set => SetProperty(ref _pattern, value);
    }

    public string DefaultValue
    {
        get => _defaultValue;
        set => SetProperty(ref _defaultValue, value);
    }

    public string ColumnName
    {
        get => _columnName;
        set => SetProperty(ref _columnName, value);
    }

    public string ColumnType
    {
        get => _columnType;
        set => SetProperty(ref _columnType, value);
    }

    public bool IsPrimaryKey
    {
        get => _isPrimaryKey;
        set => SetProperty(ref _isPrimaryKey, value);
    }

    public bool IsIdentity
    {
        get => _isIdentity;
        set => SetProperty(ref _isIdentity, value);
    }

    public bool IsForeignKey
    {
        get => _isForeignKey;
        set => SetProperty(ref _isForeignKey, value);
    }

    public string Label
    {
        get => _label;
        set => SetProperty(ref _label, value);
    }

    public string ControlType
    {
        get => _controlType;
        set => SetProperty(ref _controlType, value);
    }

    public PropertyEditorViewModel(string? existingName = null, PropertyDefinition? existingProperty = null)
    {
        IsEditMode = existingProperty != null;

        if (existingName != null)
        {
            PropertyName = existingName;
        }

        if (existingProperty != null)
        {
            LoadFromProperty(existingProperty);
        }
    }

    private void LoadFromProperty(PropertyDefinition property)
    {
        Type = property.Type ?? "string";
        Format = property.Format ?? string.Empty;
        Description = property.Description ?? string.Empty;
        IsRequired = !(property.CodeGenMetadata?.IsNullable ?? true);
        IsNullable = property.CodeGenMetadata?.IsNullable ?? false;
        MinLength = property.MinLength ?? 0;
        MaxLength = property.MaxLength ?? 0;
        Pattern = property.Pattern ?? string.Empty;
        DefaultValue = property.Default?.ToString() ?? string.Empty;

        ColumnName = property.DatabaseMetadata?.ColumnName ?? string.Empty;
        ColumnType = property.DatabaseMetadata?.ColumnType ?? string.Empty;
        IsPrimaryKey = property.DatabaseMetadata?.IsPrimaryKey ?? false;
        IsIdentity = property.DatabaseMetadata?.IsIdentity ?? false;
        IsForeignKey = property.DatabaseMetadata?.IsForeignKey ?? false;

        Label = property.CodeGenMetadata?.DisplaySettings?.Label ?? string.Empty;
        ControlType = property.CodeGenMetadata?.DisplaySettings?.ControlType ?? "(Auto)";
    }

    public PropertyDefinition ToPropertyDefinition()
    {
        return new PropertyDefinition
        {
            Type = Type,
            Format = string.IsNullOrEmpty(Format) ? null : Format,
            Description = Description,
            MinLength = MinLength > 0 ? MinLength : null,
            MaxLength = MaxLength > 0 ? MaxLength : null,
            Pattern = string.IsNullOrEmpty(Pattern) ? null : Pattern,
            Default = string.IsNullOrEmpty(DefaultValue) ? null : DefaultValue,
            DatabaseMetadata = new PropertyDatabaseMetadata
            {
                ColumnName = string.IsNullOrEmpty(ColumnName) ? PropertyName : ColumnName,
                ColumnType = string.IsNullOrEmpty(ColumnType) ? null : ColumnType,
                IsPrimaryKey = IsPrimaryKey,
                IsIdentity = IsIdentity,
                IsForeignKey = IsForeignKey
            },
            CodeGenMetadata = new PropertyCodeGenMetadata
            {
                PropertyName = PropertyName,
                IsNullable = IsNullable,
                DisplaySettings = new DisplaySettings
                {
                    Label = string.IsNullOrEmpty(Label) ? PropertyName : Label,
                    ControlType = ControlType == "(Auto)" ? null : ControlType
                }
            }
        };
    }

    public override bool Validate()
    {
        ClearValidationErrors();

        if (string.IsNullOrWhiteSpace(PropertyName))
        {
            AddValidationError(nameof(PropertyName), "Property name is required.");
        }

        return IsValid;
    }
}
