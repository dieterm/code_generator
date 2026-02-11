using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.ViewModels;

public class DelegateElementEditViewModel : CodeElementEditViewModel
{
    private DelegateElementArtifact? _artifact;

    public DelegateElementEditViewModel()
    {
        ReturnTypeField = new SingleLineTextFieldModel { Label = "Return Type", Name = nameof(DelegateElementArtifact.ReturnTypeName) };
        ReturnTypeField.PropertyChanged += OnFieldChanged;
    }

    public DelegateElementArtifact? Artifact
    {
        get => _artifact;
        set
        {
            _artifact = value;
            SetBaseArtifact(value);
            LoadFromArtifact();
        }
    }

    public SingleLineTextFieldModel ReturnTypeField { get; }

    private void LoadFromArtifact()
    {
        if (_artifact == null) return;
        _isLoading = true;
        try
        {
            LoadBaseFields();
            ReturnTypeField.Value = _artifact.ReturnTypeName;
        }
        finally { _isLoading = false; }
    }

    protected override void SaveDerivedFields()
    {
        if (_artifact == null) return;
        _artifact.ReturnTypeName = ReturnTypeField.Value as string ?? "void";
    }
}
