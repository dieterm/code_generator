using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.ViewModels;

public class PropertyElementEditViewModel : CodeElementEditViewModel<PropertyElement>
{
    private PropertyElementArtifact? _artifact;

    public PropertyElementEditViewModel()
    {
        TypeNameField = new SingleLineTextFieldModel { Label = "Type", Name = nameof(PropertyElementArtifact.TypeName) };
        HasGetterField = new BooleanFieldModel { Label = "Has Getter", Name = nameof(PropertyElementArtifact.HasGetter) };
        HasSetterField = new BooleanFieldModel { Label = "Has Setter", Name = nameof(PropertyElementArtifact.HasSetter) };
        IsInitOnlyField = new BooleanFieldModel { Label = "Is Init Only", Name = nameof(PropertyElementArtifact.IsInitOnly) };
        IsAutoImplementedField = new BooleanFieldModel { Label = "Is Auto-Implemented", Name = nameof(PropertyElementArtifact.IsAutoImplemented) };
        InitialValueField = new SingleLineTextFieldModel { Label = "Initial Value", Name = nameof(PropertyElementArtifact.InitialValue) };

        TypeNameField.PropertyChanged += OnFieldChanged;
        HasGetterField.PropertyChanged += OnFieldChanged;
        HasSetterField.PropertyChanged += OnFieldChanged;
        IsInitOnlyField.PropertyChanged += OnFieldChanged;
        IsAutoImplementedField.PropertyChanged += OnFieldChanged;
        InitialValueField.PropertyChanged += OnFieldChanged;
    }

    public PropertyElementArtifact? Artifact
    {
        get => _artifact;
        set
        {
            _artifact = value;
            SetBaseArtifact(value);
            LoadFromArtifact();
        }
    }

    public SingleLineTextFieldModel TypeNameField { get; }
    public BooleanFieldModel HasGetterField { get; }
    public BooleanFieldModel HasSetterField { get; }
    public BooleanFieldModel IsInitOnlyField { get; }
    public BooleanFieldModel IsAutoImplementedField { get; }
    public SingleLineTextFieldModel InitialValueField { get; }

    private void LoadFromArtifact()
    {
        if (_artifact == null) return;
        _isLoading = true;
        try
        {
            LoadBaseFields();
            TypeNameField.Value = _artifact.TypeName;
            HasGetterField.Value = _artifact.HasGetter;
            HasSetterField.Value = _artifact.HasSetter;
            IsInitOnlyField.Value = _artifact.IsInitOnly;
            IsAutoImplementedField.Value = _artifact.IsAutoImplemented;
            InitialValueField.Value = _artifact.InitialValue ?? string.Empty;
        }
        finally { _isLoading = false; }
    }

    protected override void SaveDerivedFields()
    {
        if (_artifact == null) return;
        _artifact.TypeName = TypeNameField.Value as string ?? "object";
        _artifact.HasGetter = HasGetterField.Value is bool g && g;
        _artifact.HasSetter = HasSetterField.Value is bool s && s;
        _artifact.IsInitOnly = IsInitOnlyField.Value is bool io && io;
        _artifact.IsAutoImplemented = IsAutoImplementedField.Value is bool ai && ai;
        _artifact.InitialValue = string.IsNullOrEmpty(InitialValueField.Value as string) ? null : InitialValueField.Value as string;
    }
}
