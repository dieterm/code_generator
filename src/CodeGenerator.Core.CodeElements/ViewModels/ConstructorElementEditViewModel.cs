using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.ViewModels;

public class ConstructorElementEditViewModel : CodeElementEditViewModel
{
    private ConstructorElementArtifact? _artifact;

    public ConstructorElementEditViewModel()
    {
        IsPrimaryField = new BooleanFieldModel { Label = "Is Primary", Name = nameof(ConstructorElementArtifact.IsPrimary) };
        IsStaticField = new BooleanFieldModel { Label = "Is Static", Name = nameof(ConstructorElementArtifact.IsStatic) };

        IsPrimaryField.PropertyChanged += OnFieldChanged;
        IsStaticField.PropertyChanged += OnFieldChanged;
    }

    public ConstructorElementArtifact? Artifact
    {
        get => _artifact;
        set
        {
            _artifact = value;
            SetBaseArtifact(value);
            LoadFromArtifact();
        }
    }

    public BooleanFieldModel IsPrimaryField { get; }
    public BooleanFieldModel IsStaticField { get; }

    private void LoadFromArtifact()
    {
        if (_artifact == null) return;
        _isLoading = true;
        try
        {
            LoadBaseFields();
            IsPrimaryField.Value = _artifact.IsPrimary;
            IsStaticField.Value = _artifact.IsStatic;
        }
        finally { _isLoading = false; }
    }

    protected override void SaveDerivedFields()
    {
        if (_artifact == null) return;
        // Body is a CompositeStatement and is read-only; editing individual statements is not supported here
        _artifact.IsPrimary = IsPrimaryField.Value is bool p && p;
        _artifact.IsStatic = IsStaticField.Value is bool s && s;
    }
}
