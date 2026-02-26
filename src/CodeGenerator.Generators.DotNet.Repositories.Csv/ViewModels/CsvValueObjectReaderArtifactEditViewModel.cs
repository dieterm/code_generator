using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.CodeElements.ViewModels.EditFields;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Generators.DotNet.Repositories.Csv.Workspace.Artifacts;
using CodeGenerator.Shared.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Generators.DotNet.Repositories.Csv.ViewModels
{
    public class CsvValueObjectReaderArtifactEditViewModel : ViewModelBase
    {
        private CsvValueObjectReaderArtifact? _csvValueObjectReaderArtifact;
        private bool _isLoading;

        public CsvValueObjectReaderArtifactEditViewModel()
        {
            CodeFileField = new CodeFileElementFieldModel { Label = "Code File", Name = "CodeFile" };

            CodeFileField.PropertyChanged += OnFieldChanged;
        }

        private void OnFieldChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading || _csvValueObjectReaderArtifact == null) return;

            if (e.PropertyName == nameof(FieldViewModelBase.Value) && sender is FieldViewModelBase field)
            {
                SaveToCsvValueObjectReaderArtifact();
                ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_csvValueObjectReaderArtifact, field.Name, field.Value));
            }
        }

        private void SaveToCsvValueObjectReaderArtifact()
        {
            _csvValueObjectReaderArtifact!.CodeFileElement = CodeFileField.Value as CodeFileElement;
        }

        public CsvValueObjectReaderArtifact? CsvValueObjectReaderArtifact
        {
            get => _csvValueObjectReaderArtifact;
            set {
                if (_csvValueObjectReaderArtifact == value) return;

                if (_csvValueObjectReaderArtifact != null)
                {
                    _csvValueObjectReaderArtifact.PropertyChanged -= CsvRepositoryBaseArtifact_PropertyChanged;
                }
                if (SetProperty(ref _csvValueObjectReaderArtifact, value))
                {
                    LoadFromCsvValueObjectReaderArtifact();
                    // listen for rename event, when table name changes outside this viewmodel
                    // (e.g., from the tree view editlabel action)
                    if (_csvValueObjectReaderArtifact != null)
                        _csvValueObjectReaderArtifact.PropertyChanged += CsvRepositoryBaseArtifact_PropertyChanged;
                }

            }
        }

        public CodeFileElementFieldModel CodeFileField { get;  }

        private void CsvRepositoryBaseArtifact_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // not used for now
        }
        /// <summary>
        /// Event raised when a property value changes
        /// </summary>
        public event EventHandler<ArtifactPropertyChangedEventArgs>? ValueChanged;
        private void LoadFromCsvValueObjectReaderArtifact()
        {
            if (_csvValueObjectReaderArtifact == null) return;

            _isLoading = true;
            try
            {
                CodeFileField.Value = _csvValueObjectReaderArtifact.CodeFileElement;

            }
            finally
            {
                _isLoading = false;
            }
        }
    }
}
