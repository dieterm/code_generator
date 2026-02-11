using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.ViewModels;

public class IndexerElementEditViewModel : CodeElementEditViewModel
{
    private IndexerElementArtifact? _artifact;

    public IndexerElementEditViewModel()
    {
        TypeNameField = new SingleLineTextFieldModel { Label = "Type", Name = nameof(IndexerElementArtifact.TypeName) };
        HasGetterField = new BooleanFieldModel { Label = "Has Getter", Name = nameof(IndexerElementArtifact.HasGetter) };
        HasSetterField = new BooleanFieldModel { Label = "Has Setter", Name = nameof(IndexerElementArtifact.HasSetter) };

        TypeNameField.PropertyChanged += OnFieldChanged;
        HasGetterField.PropertyChanged += OnFieldChanged;
        HasSetterField.PropertyChanged += OnFieldChanged;
    }

    public IndexerElementArtifact? Artifact
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
    public BooleanFieldModel HasGetterField { get; }
    public BooleanFieldModel HasSetterField { get; }

    private void LoadFromArtifact()
    {
        if (_artifact == null) return;
        _isLoading = true;
        try
        {
            LoadBaseFields();
            TypeNameField.Value = _artifact.TypeName;
            HasGetterField.Value = _artifact.HasGetter;
            HasSetterField.Value = _artifact.HasSetter;
        }
        finally { _isLoading = false; }
    }

    protected override void SaveDerivedFields()
    {
        if (_artifact == null) return;
        _artifact.TypeName = TypeNameField.Value as string ?? "object";
        _artifact.HasGetter = HasGetterField.Value is bool g && g;
        _artifact.HasSetter = HasSetterField.Value is bool s && s;
    }
}
