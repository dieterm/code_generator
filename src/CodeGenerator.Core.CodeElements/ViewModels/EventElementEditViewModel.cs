using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.ViewModels;

public class EventElementEditViewModel : CodeElementEditViewModel
{
    private EventElementArtifact? _artifact;

    public EventElementEditViewModel()
    {
        TypeNameField = new SingleLineTextFieldModel { Label = "Event Type", Name = nameof(EventElementArtifact.TypeName) };
        IsFieldLikeField = new BooleanFieldModel { Label = "Is Field-Like", Name = nameof(EventElementArtifact.IsFieldLike) };

        TypeNameField.PropertyChanged += OnFieldChanged;
        IsFieldLikeField.PropertyChanged += OnFieldChanged;
    }

    public EventElementArtifact? Artifact
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
    public BooleanFieldModel IsFieldLikeField { get; }

    private void LoadFromArtifact()
    {
        if (_artifact == null) return;
        _isLoading = true;
        try
        {
            LoadBaseFields();
            TypeNameField.Value = _artifact.TypeName;
            IsFieldLikeField.Value = _artifact.IsFieldLike;
        }
        finally { _isLoading = false; }
    }

    protected override void SaveDerivedFields()
    {
        if (_artifact == null) return;
        _artifact.TypeName = TypeNameField.Value as string ?? "EventHandler";
        _artifact.IsFieldLike = IsFieldLikeField.Value is bool b && b;
    }
}
