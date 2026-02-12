using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.ViewModels;

public class EnumMemberElementEditViewModel : CodeElementEditViewModel<EnumMemberElement>
{
    private EnumMemberElementArtifact? _artifact;

    public EnumMemberElementEditViewModel()
    {
        ValueField = new SingleLineTextFieldModel { Label = "Value", Name = nameof(EnumMemberElementArtifact.Value) };
        ValueField.PropertyChanged += OnFieldChanged;
    }

    public EnumMemberElementArtifact? Artifact
    {
        get => _artifact;
        set
        {
            _artifact = value;
            SetBaseArtifact(value);
            LoadFromArtifact();
        }
    }

    public SingleLineTextFieldModel ValueField { get; }

    private void LoadFromArtifact()
    {
        if (_artifact == null) return;
        _isLoading = true;
        try
        {
            LoadBaseFields();
            ValueField.Value = _artifact.Value?.ToString() ?? string.Empty;
        }
        finally { _isLoading = false; }
    }

    protected override void SaveDerivedFields()
    {
        if (_artifact == null) return;
        _artifact.Value = string.IsNullOrEmpty(ValueField.Value as string) ? null : ValueField.Value as string;
    }
}
