using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts.Statements;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.ViewModels.Statements;

/// <summary>
/// Base ViewModel for editing StatementArtifactBase properties.
/// </summary>
public abstract class StatementEditViewModelBase<TArtifact, TStatement> : ViewModelBase
    where TStatement : StatementElement, new()
    where TArtifact : StatementArtifactBase<TStatement>
{
    private TArtifact? _artifact;
    protected bool _isLoading;

    public TArtifact? Artifact
    {
        get => _artifact;
        set
        {
            if (_artifact != null)
                _artifact.PropertyChanged -= Artifact_PropertyChanged;

            _artifact = value;

            if (_artifact != null)
                _artifact.PropertyChanged += Artifact_PropertyChanged;

            LoadFromArtifact();
        }
    }

    public event EventHandler<ArtifactPropertyChangedEventArgs>? ValueChanged;

    protected void RaiseValueChanged(string propertyName, object? newValue)
    {
        if (_artifact != null)
            ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_artifact, propertyName, newValue));
    }

    private void Artifact_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_isLoading) return;
        OnArtifactPropertyChanged(e.PropertyName);
    }

    protected virtual void OnArtifactPropertyChanged(string? propertyName) { }

    protected abstract void LoadFromArtifact();

    protected abstract void SaveFields();

    protected void OnFieldChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_isLoading || _artifact == null) return;
        if (e.PropertyName == nameof(FieldViewModelBase.Value) && sender is FieldViewModelBase field)
        {
            SaveFields();
            RaiseValueChanged(field.Name, field.Value);
        }
    }

    protected void OnComboboxFieldChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_isLoading || _artifact == null) return;
        if (e.PropertyName == nameof(ComboboxFieldModel.SelectedItem) && sender is ComboboxFieldModel field)
        {
            SaveFields();
            RaiseValueChanged(field.Name, field.SelectedItem?.Value);
        }
    }
}
