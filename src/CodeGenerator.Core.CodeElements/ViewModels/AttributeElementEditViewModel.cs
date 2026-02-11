using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.ViewModels;

public class AttributeElementEditViewModel : CodeElementEditViewModel<AttributeElement>
{
    private AttributeElementArtifact? _artifact;

    public AttributeElementEditViewModel()
    {
        AttributeNameField = new SingleLineTextFieldModel { Label = "Attribute Name", Name = nameof(AttributeElementArtifact.AttributeName) };
        TargetField = new ComboboxFieldModel { Label = "Target", Name = nameof(AttributeElementArtifact.Target) };
        ArgumentsField = new StringListFieldModel { Label = "Arguments", Name = nameof(AttributeElementArtifact.Arguments) };
        NamedArgumentsField = new StringDictionaryFieldModel { Label = "Named Arguments", Name = nameof(AttributeElementArtifact.NamedArguments) };

        InitializeTargetItems();

        AttributeNameField.PropertyChanged += OnFieldChanged;
        TargetField.PropertyChanged += OnComboboxFieldChanged;
        ArgumentsField.PropertyChanged += OnFieldChanged;
        NamedArgumentsField.PropertyChanged += OnFieldChanged;
    }

    public AttributeElementArtifact? Artifact
    {
        get => _artifact;
        set
        {
            _artifact = value;
            SetBaseArtifact(value);
            LoadFromArtifact();
        }
    }

    public SingleLineTextFieldModel AttributeNameField { get; }
    public ComboboxFieldModel TargetField { get; }
    public StringListFieldModel ArgumentsField { get; }
    public StringDictionaryFieldModel NamedArgumentsField { get; }

    private void InitializeTargetItems()
    {
        var items = new List<ComboboxItem>();
        foreach (var target in Enum.GetValues<AttributeTarget>())
            items.Add(new ComboboxItem { DisplayName = target.ToString(), Value = target });
        TargetField.Items = items;
    }

    protected override void OnBaseArtifactPropertyChanged(string? propertyName)
    {
        base.OnBaseArtifactPropertyChanged(propertyName);
        if (propertyName == nameof(AttributeElementArtifact.AttributeName))
            AttributeNameField.Value = _artifact?.AttributeName;
    }

    private void LoadFromArtifact()
    {
        if (_artifact == null) return;
        _isLoading = true;
        try
        {
            LoadBaseFields();
            AttributeNameField.Value = _artifact.AttributeName;
            TargetField.SelectedItem = TargetField.Items
                .FirstOrDefault(i => i.Value is AttributeTarget t && t == _artifact.Target);
            ArgumentsField.Value = _artifact.Arguments;
            NamedArgumentsField.Value = _artifact.NamedArguments;
        }
        finally { _isLoading = false; }
    }

    protected override void SaveDerivedFields()
    {
        if (_artifact == null) return;
        _artifact.AttributeName = AttributeNameField.Value as string ?? "Attribute";
        if (TargetField.SelectedItem?.Value is AttributeTarget target)
            _artifact.Target = target;

        if (ArgumentsField.Value is List<string> args)
        {
            _artifact.Arguments.Clear();
            _artifact.Arguments.AddRange(args);
        }

        if (NamedArgumentsField.Value is Dictionary<string, string> namedArgs)
        {
            _artifact.NamedArguments.Clear();
            foreach (var kvp in namedArgs)
                _artifact.NamedArguments[kvp.Key] = kvp.Value;
        }
    }
}
