using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.ViewModels;

public class OperatorElementEditViewModel : CodeElementEditViewModel<OperatorElement>
{
    private OperatorElementArtifact? _artifact;

    public OperatorElementEditViewModel()
    {
        OperatorTypeField = new ComboboxFieldModel { Label = "Operator Type", Name = nameof(OperatorElementArtifact.OperatorType) };
        ReturnTypeField = new SingleLineTextFieldModel { Label = "Return Type", Name = nameof(OperatorElementArtifact.ReturnTypeName) };

        InitializeOperatorTypeItems();

        OperatorTypeField.PropertyChanged += OnComboboxFieldChanged;
        ReturnTypeField.PropertyChanged += OnFieldChanged;
    }

    public OperatorElementArtifact? Artifact
    {
        get => _artifact;
        set
        {
            _artifact = value;
            SetBaseArtifact(value);
            LoadFromArtifact();
        }
    }

    public ComboboxFieldModel OperatorTypeField { get; }
    public SingleLineTextFieldModel ReturnTypeField { get; }

    private void InitializeOperatorTypeItems()
    {
        var items = new List<ComboboxItem>();
        foreach (var opType in Enum.GetValues<OperatorType>())
            items.Add(new ComboboxItem { DisplayName = opType.ToString(), Value = opType });
        OperatorTypeField.Items = items;
    }

    protected override void OnBaseArtifactPropertyChanged(string? propertyName)
    {
        base.OnBaseArtifactPropertyChanged(propertyName);
        if (propertyName == nameof(OperatorElementArtifact.OperatorType))
        {
            OperatorTypeField.SelectedItem = OperatorTypeField.Items
                .FirstOrDefault(i => i.Value is OperatorType t && t == _artifact?.OperatorType);
        }
    }

    private void LoadFromArtifact()
    {
        if (_artifact == null) return;
        _isLoading = true;
        try
        {
            LoadBaseFields();
            OperatorTypeField.SelectedItem = OperatorTypeField.Items
                .FirstOrDefault(i => i.Value is OperatorType t && t == _artifact.OperatorType);
            ReturnTypeField.Value = _artifact.ReturnTypeName;
        }
        finally { _isLoading = false; }
    }

    protected override void SaveDerivedFields()
    {
        if (_artifact == null) return;
        if (OperatorTypeField.SelectedItem?.Value is OperatorType opType)
            _artifact.OperatorType = opType;
        _artifact.ReturnTypeName = ReturnTypeField.Value as string ?? "object";
    }
}
