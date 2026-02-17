using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.ViewModels.CodeElements;

public class GenericConstraintElementEditViewModel : CodeElementEditViewModel<GenericConstraintElement>
{
    private GenericConstraintElementArtifact? _artifact;

    public GenericConstraintElementEditViewModel()
    {
        TypeParameterNameField = new SingleLineTextFieldModel { Label = "Type Parameter Name", Name = nameof(GenericConstraintElementArtifact.TypeParameterName) };
        ConstraintKindField = new MultiSelectFieldModel { Label = "Constraint Kind", Name = nameof(GenericConstraintElementArtifact.ConstraintKind) };

        ConstraintKindField.LoadFromFlagsEnum(GenericConstraintKind.None);

        TypeParameterNameField.PropertyChanged += OnFieldChanged;
        ConstraintKindField.PropertyChanged += OnMultiSelectFieldChanged;
    }

    public GenericConstraintElementArtifact? Artifact
    {
        get => _artifact;
        set
        {
            _artifact = value;
            SetBaseArtifact(value);
            LoadFromArtifact();
        }
    }

    public SingleLineTextFieldModel TypeParameterNameField { get; }
    public MultiSelectFieldModel ConstraintKindField { get; }

    private void LoadFromArtifact()
    {
        if (_artifact == null) return;
        _isLoading = true;
        try
        {
            LoadBaseFields();
            TypeParameterNameField.Value = _artifact.TypeParameterName;
            ConstraintKindField.SetFlagsEnumValue(_artifact.ConstraintKind);
        }
        finally { _isLoading = false; }
    }

    protected override void SaveDerivedFields()
    {
        if (_artifact == null) return;
        _artifact.TypeParameterName = TypeParameterNameField.Value as string ?? string.Empty;
        _artifact.ConstraintKind = ConstraintKindField.GetFlagsEnumValue<GenericConstraintKind>();
    }
}
