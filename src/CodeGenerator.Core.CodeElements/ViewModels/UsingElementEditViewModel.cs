using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.ViewModels;

public class UsingElementEditViewModel : CodeElementEditViewModel<UsingElement>
{
    private UsingElementArtifact? _artifact;

    public UsingElementEditViewModel()
    {
        NamespaceField = new SingleLineTextFieldModel { Label = "Namespace", Name = nameof(UsingElementArtifact.Namespace) };
        AliasField = new SingleLineTextFieldModel { Label = "Alias", Name = nameof(UsingElementArtifact.Alias) };
        IsGlobalField = new BooleanFieldModel { Label = "Is Global", Name = nameof(UsingElementArtifact.IsGlobal) };

        NamespaceField.PropertyChanged += OnFieldChanged;
        AliasField.PropertyChanged += OnFieldChanged;
        IsGlobalField.PropertyChanged += OnFieldChanged;
    }

    public UsingElementArtifact? Artifact
    {
        get => _artifact;
        set
        {
            _artifact = value;
            SetBaseArtifact(value);
            LoadFromArtifact();
        }
    }

    public SingleLineTextFieldModel NamespaceField { get; }
    public SingleLineTextFieldModel AliasField { get; }
    public BooleanFieldModel IsGlobalField { get; }

    protected override void OnBaseArtifactPropertyChanged(string? propertyName)
    {
        base.OnBaseArtifactPropertyChanged(propertyName);
        if (propertyName == nameof(UsingElementArtifact.Namespace))
            NamespaceField.Value = _artifact?.Namespace;
    }

    private void LoadFromArtifact()
    {
        if (_artifact == null) return;
        _isLoading = true;
        try
        {
            LoadBaseFields();
            NamespaceField.Value = _artifact.Namespace;
            AliasField.Value = _artifact.Alias;
            IsGlobalField.Value = _artifact.IsGlobal;
        }
        finally { _isLoading = false; }
    }

    protected override void SaveDerivedFields()
    {
        if (_artifact == null) return;
        _artifact.Namespace = NamespaceField.Value as string ?? "System";
        _artifact.Alias = AliasField.Value as string;
        _artifact.IsGlobal = IsGlobalField.Value is bool b && b;
    }
}
