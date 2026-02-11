using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.ViewModels;

public class FieldElementEditViewModel : CodeElementEditViewModel
{
    private FieldElementArtifact? _artifact;

    public FieldElementEditViewModel()
    {
        TypeNameField = new SingleLineTextFieldModel { Label = "Type", Name = nameof(FieldElementArtifact.TypeName) };
        InitialValueField = new SingleLineTextFieldModel { Label = "Initial Value", Name = nameof(FieldElementArtifact.InitialValue) };

        TypeNameField.PropertyChanged += OnFieldChanged;
        InitialValueField.PropertyChanged += OnFieldChanged;
    }

    public FieldElementArtifact? Artifact
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
    public SingleLineTextFieldModel InitialValueField { get; }

    private void LoadFromArtifact()
    {
        if (_artifact == null) return;
        _isLoading = true;
        try
        {
            LoadBaseFields();
            TypeNameField.Value = _artifact.TypeName;
            InitialValueField.Value = _artifact.InitialValue ?? string.Empty;
        }
        finally { _isLoading = false; }
    }

    protected override void SaveDerivedFields()
    {
        if (_artifact == null) return;
        _artifact.TypeName = TypeNameField.Value as string ?? "object";
        _artifact.InitialValue = string.IsNullOrEmpty(InitialValueField.Value as string) ? null : InitialValueField.Value as string;
    }
}
