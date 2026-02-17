using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.ViewModels.CodeElements;

public class GenericTypeParameterElementEditViewModel : CodeElementEditViewModel<GenericTypeParameterElement>
{
    private GenericTypeParameterElementArtifact? _artifact;

    public GenericTypeParameterElementEditViewModel()
    {
        VarianceField = new ComboboxFieldModel { Label = "Variance", Name = nameof(GenericTypeParameterElementArtifact.Variance) };

        InitializeVarianceItems();

        VarianceField.PropertyChanged += OnComboboxFieldChanged;
    }

    public GenericTypeParameterElementArtifact? Artifact
    {
        get => _artifact;
        set
        {
            _artifact = value;
            SetBaseArtifact(value);
            LoadFromArtifact();
        }
    }

    public ComboboxFieldModel VarianceField { get; }

    private void InitializeVarianceItems()
    {
        var items = new List<ComboboxItem>();
        foreach (var variance in Enum.GetValues<GenericVariance>())
            items.Add(new ComboboxItem { DisplayName = variance.ToString(), Value = variance });
        VarianceField.Items = items;
    }

    private void LoadFromArtifact()
    {
        if (_artifact == null) return;
        _isLoading = true;
        try
        {
            LoadBaseFields();
            VarianceField.SelectedItem = VarianceField.Items
                .FirstOrDefault(i => i.Value is GenericVariance v && v == _artifact.Variance);
        }
        finally { _isLoading = false; }
    }

    protected override void SaveDerivedFields()
    {
        if (_artifact == null) return;
        if (VarianceField.SelectedItem?.Value is GenericVariance variance)
            _artifact.Variance = variance;
    }
}
