using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.ViewModels;

/// <summary>
/// Base ViewModel for editing CodeElementArtifactBase properties.
/// Provides Name, AccessModifier, Modifiers, Documentation and RawCode fields.
/// </summary>
public class CodeElementEditViewModel : ViewModelBase
{
    private CodeElementArtifactBase? _baseArtifact;
    protected bool _isLoading;

    public CodeElementEditViewModel()
    {
        NameField = new SingleLineTextFieldModel { Label = "Name", Name = nameof(CodeElementArtifactBase.Name) };
        AccessModifierField = new ComboboxFieldModel { Label = "Access Modifier", Name = nameof(CodeElementArtifactBase.AccessModifier) };
        ModifiersField = new MultiSelectFieldModel { Label = "Modifiers", Name = nameof(CodeElementArtifactBase.Modifiers) };
        DocumentationField = new SingleLineTextFieldModel { Label = "Documentation", Name = nameof(CodeElementArtifactBase.Documentation) };
        RawCodeField = new SingleLineTextFieldModel { Label = "Raw Code", Name = nameof(CodeElementArtifactBase.RawCode) };

        InitializeAccessModifierItems();
        InitializeModifiersItems();

        NameField.PropertyChanged += OnFieldChanged;
        AccessModifierField.PropertyChanged += OnComboboxFieldChanged;
        ModifiersField.PropertyChanged += OnMultiSelectFieldChanged;
        DocumentationField.PropertyChanged += OnFieldChanged;
        RawCodeField.PropertyChanged += OnFieldChanged;
    }

    /// <summary>
    /// The base artifact being edited. Subclasses should expose a typed Artifact property
    /// that sets this via SetBaseArtifact.
    /// </summary>
    protected CodeElementArtifactBase? BaseArtifact => _baseArtifact;

    public SingleLineTextFieldModel NameField { get; }
    public ComboboxFieldModel AccessModifierField { get; }
    public MultiSelectFieldModel ModifiersField { get; }
    public SingleLineTextFieldModel DocumentationField { get; }
    public SingleLineTextFieldModel RawCodeField { get; }

    public event EventHandler<ArtifactPropertyChangedEventArgs>? ValueChanged;

    protected void RaiseValueChanged(string propertyName, object? newValue)
    {
        if (_baseArtifact != null)
            ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_baseArtifact, propertyName, newValue));
    }

    /// <summary>
    /// Sets the base artifact and loads the base fields.
    /// Subclasses should call this from their Artifact setter.
    /// </summary>
    protected void SetBaseArtifact(CodeElementArtifactBase? artifact)
    {
        if (_baseArtifact != null)
            _baseArtifact.PropertyChanged -= BaseArtifact_PropertyChanged;

        _baseArtifact = artifact;

        if (_baseArtifact != null)
            _baseArtifact.PropertyChanged += BaseArtifact_PropertyChanged;
    }

    private void InitializeAccessModifierItems()
    {
        var items = new List<ComboboxItem>();
        foreach (var modifier in Enum.GetValues<AccessModifier>())
            items.Add(new ComboboxItem { DisplayName = modifier.ToString(), Value = modifier });
        AccessModifierField.Items = items;
    }

    private void InitializeModifiersItems()
    {
        ModifiersField.LoadFromFlagsEnum(ElementModifiers.None);
    }

    private void BaseArtifact_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_isLoading) return;
        OnBaseArtifactPropertyChanged(e.PropertyName);
    }

    /// <summary>
    /// Called when the base artifact raises PropertyChanged.
    /// Override to handle additional properties.
    /// </summary>
    protected virtual void OnBaseArtifactPropertyChanged(string? propertyName)
    {
        if (propertyName == nameof(CodeElementArtifactBase.Name))
            NameField.Value = _baseArtifact?.Name;
    }

    /// <summary>
    /// Loads all base fields from the artifact. Called during SetBaseArtifact.
    /// Subclasses should override and call base.LoadBaseFields() first.
    /// </summary>
    public virtual void LoadBaseFields()
    {
        if (_baseArtifact == null) return;

        NameField.Value = _baseArtifact.Name;
        AccessModifierField.SelectedItem = AccessModifierField.Items
            .FirstOrDefault(i => i.Value is AccessModifier m && m == _baseArtifact.AccessModifier);
        ModifiersField.SetFlagsEnumValue(_baseArtifact.Modifiers);
        DocumentationField.Value = _baseArtifact.Documentation ?? string.Empty;
        RawCodeField.Value = _baseArtifact.RawCode ?? string.Empty;
    }

    /// <summary>
    /// Saves all base fields to the artifact.
    /// Subclasses should override and call base.SaveBaseFields() first.
    /// </summary>
    public virtual void SaveBaseFields()
    {
        if (_baseArtifact == null) return;

        _baseArtifact.Name = NameField.Value as string ?? string.Empty;
        if (AccessModifierField.SelectedItem?.Value is AccessModifier mod)
            _baseArtifact.AccessModifier = mod;
        _baseArtifact.Modifiers = ModifiersField.GetFlagsEnumValue<ElementModifiers>();
        _baseArtifact.Documentation = DocumentationField.Value as string;
        _baseArtifact.RawCode = string.IsNullOrEmpty(RawCodeField.Value as string) ? null : RawCodeField.Value as string;
    }

    protected void OnFieldChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_isLoading || _baseArtifact == null) return;
        if (e.PropertyName == nameof(FieldViewModelBase.Value) && sender is FieldViewModelBase field)
        {
            SaveBaseFields();
            SaveDerivedFields();
            RaiseValueChanged(field.Name, field.Value);
        }
    }

    protected void OnComboboxFieldChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_isLoading || _baseArtifact == null) return;
        if (e.PropertyName == nameof(ComboboxFieldModel.SelectedItem) && sender is ComboboxFieldModel field)
        {
            SaveBaseFields();
            SaveDerivedFields();
            RaiseValueChanged(field.Name, field.SelectedItem?.Value);
        }
    }

    protected void OnMultiSelectFieldChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_isLoading || _baseArtifact == null) return;
        if (e.PropertyName == nameof(MultiSelectFieldModel.SelectedItems) && sender is MultiSelectFieldModel field)
        {
            SaveBaseFields();
            SaveDerivedFields();
            RaiseValueChanged(field.Name, field.SelectedItems);
        }
    }

    /// <summary>
    /// Override in subclasses to save subclass-specific fields to the artifact.
    /// </summary>
    protected virtual void SaveDerivedFields() { }
}
