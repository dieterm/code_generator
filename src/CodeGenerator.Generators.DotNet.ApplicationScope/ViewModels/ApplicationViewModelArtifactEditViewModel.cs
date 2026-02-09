using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Generators.DotNet.ApplicationScope.Workspace.Artifacts;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Generators.DotNet.ApplicationScope.ViewModels
{
    /// <summary>
    /// ViewModel for editing ApplicationViewModelArtifact properties
    /// </summary>
    public class ApplicationViewModelArtifactEditViewModel : ViewModelBase
    {
        private ApplicationViewModelArtifact? _artifact;
        private bool _isLoading;

        public ApplicationViewModelArtifactEditViewModel()
        {
            ApplicationNameField = new SingleLineTextFieldModel { Label = "Application Name", Name = nameof(ApplicationViewModelArtifact.ApplicationName) };

            ApplicationNameField.PropertyChanged += OnFieldChanged;
        }

        /// <summary>
        /// The artifact being edited
        /// </summary>
        public ApplicationViewModelArtifact? Artifact
        {
            get => _artifact;
            set
            {
                if (_artifact != null)
                {
                    _artifact.PropertyChanged -= Artifact_PropertyChanged;
                }
                if (SetProperty(ref _artifact, value))
                {
                    LoadFromArtifact();
                    if (_artifact != null)
                    {
                        _artifact.PropertyChanged += Artifact_PropertyChanged;
                    }
                }
            }
        }

        private void Artifact_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ApplicationViewModelArtifact.ApplicationName))
            {
                ApplicationNameField.Value = _artifact?.ApplicationName;
            }
        }

        /// <summary>
        /// Application name field
        /// </summary>
        public SingleLineTextFieldModel ApplicationNameField { get; }

        /// <summary>
        /// Event raised when any field value changes
        /// </summary>
        public event EventHandler<ArtifactPropertyChangedEventArgs>? ValueChanged;

        private void LoadFromArtifact()
        {
            if (_artifact == null) return;

            _isLoading = true;
            try
            {
                ApplicationNameField.Value = _artifact.ApplicationName;
            }
            finally
            {
                _isLoading = false;
            }
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

        private void SaveToArtifact()
        {
            if (_artifact == null) return;
            _artifact.ApplicationName = ApplicationNameField.Value as string;
        }
    }
}
