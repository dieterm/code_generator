using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.ViewModels;

public class ClassElementEditViewModel : CodeElementEditViewModel<ClassElement>
{
    private ClassElementArtifact? _artifact;

    public ClassElementEditViewModel()
    {
        IsRecordField = new BooleanFieldModel { Label = "Is Record", Name = nameof(ClassElementArtifact.IsRecord) };
        IsRecordField.PropertyChanged += OnFieldChanged;
    }

    public ClassElementArtifact? Artifact
    {
        get => _artifact;
        set
        {
            _artifact = value;
            SetBaseArtifact(value);
            LoadFromArtifact();
        }
    }

    public BooleanFieldModel IsRecordField { get; }

    private void LoadFromArtifact()
    {
        if (_artifact == null) return;
        _isLoading = true;
        try
        {
            LoadBaseFields();
            IsRecordField.Value = _artifact.IsRecord;
        }
        finally { _isLoading = false; }
    }

    protected override void SaveDerivedFields()
    {
        if (_artifact == null) return;
        _artifact.IsRecord = IsRecordField.Value is bool b && b;
    }
}
