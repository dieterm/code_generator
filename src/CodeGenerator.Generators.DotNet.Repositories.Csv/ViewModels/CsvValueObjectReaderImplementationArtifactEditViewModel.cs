using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.CodeElements.ViewModels.EditFields;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Generators.DotNet.Repositories.Csv.Workspace.Artifacts;
using CodeGenerator.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.Repositories.Csv.ViewModels
{
    public class CsvValueObjectReaderImplementationArtifactEditViewModel : ViewModelBase
    {
        private CsvValueObjectReaderImplementationArtifact? _csvValueObjectReaderImplementationArtifact;
        private bool _isLoading;

        public CsvValueObjectReaderImplementationArtifactEditViewModel()
        {
            CodeFileField = new CodeFileElementFieldModel { Label = "Code File", Name = "CodeFile" };

            CodeFileField.PropertyChanged += OnFieldChanged;
        }

        private void OnFieldChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading || _csvValueObjectReaderImplementationArtifact == null) return;

            if (e.PropertyName == nameof(FieldViewModelBase.Value) && sender is FieldViewModelBase field)
            {
                SaveToCsvValueObjectReaderImplementationArtifact();
                ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_csvValueObjectReaderImplementationArtifact, field.Name, field.Value));
            }
        }

        private void SaveToCsvValueObjectReaderImplementationArtifact()
        {
            _csvValueObjectReaderImplementationArtifact!.CodeFileElement = CodeFileField.Value as CodeFileElement;
        }

        public CsvValueObjectReaderImplementationArtifact? CsvValueObjectReaderImplementationArtifact
        {
            get => _csvValueObjectReaderImplementationArtifact;
            set
            {
                if (_csvValueObjectReaderImplementationArtifact == value) return;

                if (_csvValueObjectReaderImplementationArtifact != null)
                {
                    _csvValueObjectReaderImplementationArtifact.PropertyChanged -= CsvValueObjectReaderImplementationArtifact_PropertyChanged;
                }
                if (SetProperty(ref _csvValueObjectReaderImplementationArtifact, value))
                {
                    LoadFromCsvValueObjectReaderImplementationArtifact();
                    // listen for rename event, when table name changes outside this viewmodel
                    // (e.g., from the tree view editlabel action)
                    if (_csvValueObjectReaderImplementationArtifact != null)
                        _csvValueObjectReaderImplementationArtifact.PropertyChanged += CsvValueObjectReaderImplementationArtifact_PropertyChanged;
                }

            }
        }

        public CodeFileElementFieldModel CodeFileField { get; }

        private void CsvValueObjectReaderImplementationArtifact_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // not used for now
        }
        /// <summary>
        /// Event raised when a property value changes
        /// </summary>
        public event EventHandler<ArtifactPropertyChangedEventArgs>? ValueChanged;
        private void LoadFromCsvValueObjectReaderImplementationArtifact()
        {
            if (_csvValueObjectReaderImplementationArtifact == null) return;

            _isLoading = true;
            try
            {
                CodeFileField.Value = _csvValueObjectReaderImplementationArtifact.CodeFileElement;

            }
            finally
            {
                _isLoading = false;
            }
        }
    }
}
