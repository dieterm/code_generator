using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.CodeElements.ViewModels.EditFields;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.ValueTypes;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Generators.DotNet.Repositories.Csv.Workspace.Artifacts;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
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
            NameField = new SingleLineTextFieldModel { Label = "Name", Name = "Name" };
            DescriptionField = new MultiLineTextFieldModel { Label = "Description", Name = "Description" };
            ValueTypeField = new ComboboxFieldModel { Label = "Value Type", Name = "ValueTypeId" };
            CodeFileField = new CodeFileElementFieldModel { Label = "Code File", Name = "CodeFile" };

            NameField.PropertyChanged += OnFieldChanged;
            DescriptionField.PropertyChanged += OnFieldChanged;
            ValueTypeField.PropertyChanged += OnComboboxFieldChanged;
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

        private void OnComboboxFieldChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading || _csvValueObjectReaderImplementationArtifact == null) return;

            if (e.PropertyName == nameof(ComboboxFieldModel.SelectedItem) && sender is ComboboxFieldModel field)
            {
                SaveToCsvValueObjectReaderImplementationArtifact();
                ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_csvValueObjectReaderImplementationArtifact, field.Name, field.SelectedItem?.Value));
            }
        }

        private void SaveToCsvValueObjectReaderImplementationArtifact()
        {
            _csvValueObjectReaderImplementationArtifact!.Name = NameField.Value as string;
            _csvValueObjectReaderImplementationArtifact!.Description = string.IsNullOrEmpty(DescriptionField.Value as string) ? null : DescriptionField.Value as string;
            _csvValueObjectReaderImplementationArtifact!.ValueTypeId = ValueTypeField.SelectedItem?.Value as string;
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
                    if (_csvValueObjectReaderImplementationArtifact != null)
                        _csvValueObjectReaderImplementationArtifact.PropertyChanged += CsvValueObjectReaderImplementationArtifact_PropertyChanged;
                }
            }
        }

        public SingleLineTextFieldModel NameField { get; }
        public MultiLineTextFieldModel DescriptionField { get; }
        public ComboboxFieldModel ValueTypeField { get; }
        public CodeFileElementFieldModel CodeFileField { get; }

        private void CsvValueObjectReaderImplementationArtifact_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading) return;
            if (e.PropertyName == nameof(CsvValueObjectReaderImplementationArtifact.Name))
                NameField.Value = _csvValueObjectReaderImplementationArtifact?.Name;
        }

        /// <summary>
        /// Event raised when a property value changes
        /// </summary>
        public event EventHandler<ArtifactPropertyChangedEventArgs>? ValueChanged;

        private void LoadValueTypeItems()
        {
            var items = new List<ComboboxItem>();

            if (_csvValueObjectReaderImplementationArtifact != null)
            {
                var scope = _csvValueObjectReaderImplementationArtifact.FindAncesterOfType<Core.Workspaces.Artifacts.Scopes.ScopeArtifact>();
                if (scope != null)
                {
                    foreach (var valueType in scope.Domains.SelectMany(d => d.ValueTypes))
                    {
                        items.Add(new ComboboxItem
                        {
                            DisplayName = valueType.Name,
                            Value = valueType.Id
                        });
                    }
                }
            }

            ValueTypeField.Items = items;
        }

        private void LoadFromCsvValueObjectReaderImplementationArtifact()
        {
            if (_csvValueObjectReaderImplementationArtifact == null) return;

            _isLoading = true;
            try
            {
                NameField.Value = _csvValueObjectReaderImplementationArtifact.Name;
                DescriptionField.Value = _csvValueObjectReaderImplementationArtifact.Description ?? string.Empty;

                LoadValueTypeItems();
                ValueTypeField.SelectedItem = ValueTypeField.Items
                    .FirstOrDefault(i => i.Value as string == _csvValueObjectReaderImplementationArtifact.ValueTypeId);

                CodeFileField.Value = _csvValueObjectReaderImplementationArtifact.CodeFileElement;
            }
            finally
            {
                _isLoading = false;
            }
        }
    }
}
