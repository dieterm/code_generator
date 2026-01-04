using CodeGenerator.Core.Models.Schema;
using CodeGenerator.Shared.ViewModels;

namespace CodeGenerator.WinForms.ViewModels;

public class PropertyDefinitionEditViewModel : ValidationViewModelBase
{
    private PropertyDefinition? _property;
    private string _type = "string";
    private string _ref = string.Empty;
    private string _format = string.Empty;
    private string _description = string.Empty;
    private bool _isNullable;
    private int _minLength;
    private int _maxLength;

    public PropertyDefinitionEditViewModel()
    {
    }

    public PropertyDefinitionEditViewModel(PropertyDefinition property)
    {
        LoadFromProperty(property);
    }

    public string Type
    {
        get => _type;
        set => SetProperty(ref _type, value);
    }
    public string Ref
    {
        get => _ref;
        set => SetProperty(ref _ref, value);
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

    public void LoadFromProperty(PropertyDefinition property)
    {
        _property = property;
        Ref = property.Type=="array"? property.Items?.Ref : property.Ref;
        Type = string.IsNullOrWhiteSpace(property.Type) && !string.IsNullOrWhiteSpace(property.Ref) ? "ref" : property.Type;
        Format = property.Format ?? string.Empty;
        Description = property.Description ?? string.Empty;
        IsNullable = property.CodeGenMetadata?.IsNullable ?? false;
        MinLength = property.MinLength ?? 0;
        MaxLength = property.MaxLength ?? 0;
    }

    public void UpdateProperty()
    {
        if (_property != null)
        {
            _property.Type = Type;
            if (Type == "ref")
            {
                _property.Ref = Ref;
                _property.Items = null;

            }
            else if (Type == "array")
            {
                if (_property.Items == null)
                    _property.Items = new PropertyDefinition();
                _property.Items.Ref = Ref;
                _property.Ref = null;
            }
            else
            {
                _property.Ref = null;
                _property.Items = null;
            }
            
            _property.Format = string.IsNullOrWhiteSpace(Format) ? null : Format;
            _property.Description = string.IsNullOrWhiteSpace(Description) ? null : Description;
            _property.MinLength = MinLength > 0 ? MinLength : null;
            _property.MaxLength = MaxLength > 0 ? MaxLength : null;

            if (_property.CodeGenMetadata == null)
                _property.CodeGenMetadata = new PropertyCodeGenMetadata();

            _property.CodeGenMetadata.IsNullable = IsNullable;
        }
    }

    public override bool Validate()
    {
        ClearValidationErrors();
        return IsValid;
    }
}
