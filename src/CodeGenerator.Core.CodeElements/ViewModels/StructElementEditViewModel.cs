using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.ViewModels;

public class StructElementEditViewModel : CodeElementEditViewModel<StructElement>
{
    private StructElementArtifact? _artifact;

    public StructElementEditViewModel()
    {
        IsRecordField = new BooleanFieldModel { Label = "Is Record", Name = nameof(StructElementArtifact.IsRecord) };
        IsReadonlyField = new BooleanFieldModel { Label = "Is Readonly", Name = nameof(StructElementArtifact.IsReadonly) };
        IsRefField = new BooleanFieldModel { Label = "Is Ref", Name = nameof(StructElementArtifact.IsRef) };

        IsRecordField.PropertyChanged += OnFieldChanged;
        IsReadonlyField.PropertyChanged += OnFieldChanged;
        IsRefField.PropertyChanged += OnFieldChanged;
    }

    public StructElementArtifact? Artifact
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
    public BooleanFieldModel IsReadonlyField { get; }
    public BooleanFieldModel IsRefField { get; }

    private void LoadFromArtifact()
    {
        if (_artifact == null) return;
        _isLoading = true;
        try
        {
            LoadBaseFields();
            IsRecordField.Value = _artifact.IsRecord;
            IsReadonlyField.Value = _artifact.IsReadonly;
            IsRefField.Value = _artifact.IsRef;
        }
        finally { _isLoading = false; }
    }

    protected override void SaveDerivedFields()
    {
        if (_artifact == null) return;
        _artifact.IsRecord = IsRecordField.Value is bool r && r;
        _artifact.IsReadonly = IsReadonlyField.Value is bool ro && ro;
        _artifact.IsRef = IsRefField.Value is bool rf && rf;
    }
}
