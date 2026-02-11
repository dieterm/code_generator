using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.ViewModels;

public class ParameterElementEditViewModel : CodeElementEditViewModel<ParameterElement>
{
    private ParameterElementArtifact? _artifact;

    public ParameterElementEditViewModel()
    {
        TypeNameField = new SingleLineTextFieldModel { Label = "Type", Name = nameof(ParameterElementArtifact.TypeName) };
        ModifierField = new ComboboxFieldModel { Label = "Modifier", Name = nameof(ParameterElementArtifact.Modifier) };
        DefaultValueField = new SingleLineTextFieldModel { Label = "Default Value", Name = nameof(ParameterElementArtifact.DefaultValue) };
        IsExtensionMethodThisField = new BooleanFieldModel { Label = "Is Extension Method This", Name = nameof(ParameterElementArtifact.IsExtensionMethodThis) };

        InitializeModifierItems();

        TypeNameField.PropertyChanged += OnFieldChanged;
        ModifierField.PropertyChanged += OnComboboxFieldChanged;
        DefaultValueField.PropertyChanged += OnFieldChanged;
        IsExtensionMethodThisField.PropertyChanged += OnFieldChanged;
    }

    public ParameterElementArtifact? Artifact
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
    public ComboboxFieldModel ModifierField { get; }
    public SingleLineTextFieldModel DefaultValueField { get; }
    public BooleanFieldModel IsExtensionMethodThisField { get; }

    private void InitializeModifierItems()
    {
        var items = new List<ComboboxItem>();
        foreach (var mod in Enum.GetValues<ParameterModifier>())
            items.Add(new ComboboxItem { DisplayName = mod.ToString(), Value = mod });
        ModifierField.Items = items;
    }

    private void LoadFromArtifact()
    {
        if (_artifact == null) return;
        _isLoading = true;
        try
        {
            LoadBaseFields();
            TypeNameField.Value = _artifact.TypeName;
            ModifierField.SelectedItem = ModifierField.Items
                .FirstOrDefault(i => i.Value is ParameterModifier m && m == _artifact.Modifier);
            DefaultValueField.Value = _artifact.DefaultValue ?? string.Empty;
            IsExtensionMethodThisField.Value = _artifact.IsExtensionMethodThis;
        }
        finally { _isLoading = false; }
    }

    protected override void SaveDerivedFields()
    {
        if (_artifact == null) return;
        _artifact.TypeName = TypeNameField.Value as string ?? "object";
        if (ModifierField.SelectedItem?.Value is ParameterModifier mod)
            _artifact.Modifier = mod;
        _artifact.DefaultValue = string.IsNullOrEmpty(DefaultValueField.Value as string) ? null : DefaultValueField.Value as string;
        _artifact.IsExtensionMethodThis = IsExtensionMethodThisField.Value is bool b && b;
    }
}
