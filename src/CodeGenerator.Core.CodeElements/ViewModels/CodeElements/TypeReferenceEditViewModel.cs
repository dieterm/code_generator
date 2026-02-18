using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.ViewModels.CodeElements;

public class TypeReferenceEditViewModel : ViewModelBase
{
    private TypeReferenceArtifact? _artifact;
    private bool _isLoading;

    public TypeReferenceEditViewModel()
    {
        TypeNameField = new SingleLineTextFieldModel { Label = "Type Name", Name = nameof(TypeReferenceArtifact.TypeName) };
        NamespaceField = new SingleLineTextFieldModel { Label = "Namespace", Name = nameof(TypeReferenceArtifact.Namespace) };
        IsNullableField = new BooleanFieldModel { Label = "Is Nullable", Name = nameof(TypeReferenceArtifact.IsNullable) };
        IsArrayField = new BooleanFieldModel { Label = "Is Array", Name = nameof(TypeReferenceArtifact.IsArray) };
        ArrayRankField = new IntegerFieldModel { Label = "Array Rank", Name = nameof(TypeReferenceArtifact.ArrayRank), Minimum = 1, Maximum = 32 };

        TypeNameField.PropertyChanged += OnFieldChanged;
        NamespaceField.PropertyChanged += OnFieldChanged;
        IsNullableField.PropertyChanged += OnFieldChanged;
        IsArrayField.PropertyChanged += OnFieldChanged;
        ArrayRankField.PropertyChanged += OnFieldChanged;
    }

    public TypeReferenceArtifact? Artifact
    {
        get => _artifact;
        set
        {
            if (_artifact == value) return;

            if (_artifact != null)
                _artifact.PropertyChanged -= Artifact_PropertyChanged;

            if (SetProperty(ref _artifact, value))
            {
                LoadFromArtifact();
                if (_artifact != null)
                    _artifact.PropertyChanged += Artifact_PropertyChanged;
            }
        }
    }

    public SingleLineTextFieldModel TypeNameField { get; }
    public SingleLineTextFieldModel NamespaceField { get; }
    public BooleanFieldModel IsNullableField { get; }
    public BooleanFieldModel IsArrayField { get; }
    public IntegerFieldModel ArrayRankField { get; }

    public event EventHandler<ArtifactPropertyChangedEventArgs>? ValueChanged;

    private void Artifact_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_isLoading) return;
        if (e.PropertyName == nameof(TypeReferenceArtifact.TypeName))
            TypeNameField.Value = _artifact?.TypeName;
    }

    private void OnFieldChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_isLoading || _artifact == null) return;

        if (e.PropertyName == nameof(FieldViewModelBase.Value) && sender is FieldViewModelBase field)
        {
            SaveToArtifact();
            ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_artifact, field.Name, field.Value));
        }
    }

    private void LoadFromArtifact()
    {
        if (_artifact == null) return;

        _isLoading = true;
        try
        {
            TypeNameField.Value = _artifact.TypeName;
            NamespaceField.Value = _artifact.Namespace ?? string.Empty;
            IsNullableField.Value = _artifact.IsNullable;
            IsArrayField.Value = _artifact.IsArray;
            ArrayRankField.Value = _artifact.ArrayRank;
        }
        finally { _isLoading = false; }
    }

    private void SaveToArtifact()
    {
        if (_artifact == null) return;

        _artifact.TypeName = TypeNameField.Value as string ?? string.Empty;
        _artifact.Namespace = string.IsNullOrEmpty(NamespaceField.Value as string) ? null : NamespaceField.Value as string;
        _artifact.IsNullable = IsNullableField.Value is bool b && b;
        _artifact.IsArray = IsArrayField.Value is bool a && a;
        _artifact.ArrayRank = ArrayRankField.Value is int r ? r : 1;
    }
}
