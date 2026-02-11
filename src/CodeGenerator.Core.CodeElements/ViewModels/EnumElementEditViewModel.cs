using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.ViewModels;

public class EnumElementEditViewModel : CodeElementEditViewModel<EnumElement>
{
    private EnumElementArtifact? _artifact;

    public EnumElementEditViewModel()
    {
        IsFlagsField = new BooleanFieldModel { Label = "Is Flags", Name = nameof(EnumElementArtifact.IsFlags) };
        IsFlagsField.PropertyChanged += OnFieldChanged;
    }

    public EnumElementArtifact? Artifact
    {
        get => _artifact;
        set
        {
            _artifact = value;
            SetBaseArtifact(value);
            LoadFromArtifact();
        }
    }

    public BooleanFieldModel IsFlagsField { get; }

    private void LoadFromArtifact()
    {
        if (_artifact == null) return;
        _isLoading = true;
        try
        {
            LoadBaseFields();
            IsFlagsField.Value = _artifact.IsFlags;
        }
        finally { _isLoading = false; }
    }

    protected override void SaveDerivedFields()
    {
        if (_artifact == null) return;
        _artifact.IsFlags = IsFlagsField.Value is bool b && b;
    }
}
