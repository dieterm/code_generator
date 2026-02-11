using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.ViewModels;

public class NamespaceElementEditViewModel : CodeElementEditViewModel
{
    private NamespaceElementArtifact? _artifact;

    public NamespaceElementEditViewModel()
    {
        IsFileScopedField = new BooleanFieldModel { Label = "File Scoped", Name = nameof(NamespaceElementArtifact.IsFileScoped) };
        IsFileScopedField.PropertyChanged += OnFieldChanged;
    }

    public NamespaceElementArtifact? Artifact
    {
        get => _artifact;
        set
        {
            _artifact = value;
            SetBaseArtifact(value);
            LoadFromArtifact();
        }
    }

    public BooleanFieldModel IsFileScopedField { get; }

    protected override void OnBaseArtifactPropertyChanged(string? propertyName)
    {
        base.OnBaseArtifactPropertyChanged(propertyName);
        if (propertyName == nameof(NamespaceElementArtifact.FullName))
            NameField.Value = _artifact?.FullName;
    }

    private void LoadFromArtifact()
    {
        if (_artifact == null) return;
        _isLoading = true;
        try
        {
            LoadBaseFields();
            // Override name field with FullName for namespace display
            NameField.Value = _artifact.FullName;
            IsFileScopedField.Value = _artifact.IsFileScoped;
        }
        finally { _isLoading = false; }
    }

    protected override void SaveDerivedFields()
    {
        if (_artifact == null) return;
        _artifact.FullName = NameField.Value as string ?? "MyNamespace";
        _artifact.IsFileScoped = IsFileScopedField.Value is bool b && b;
    }
}
