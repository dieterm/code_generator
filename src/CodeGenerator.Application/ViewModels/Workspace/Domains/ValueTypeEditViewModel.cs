using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.ValueTypes;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Application.ViewModels.Workspace.Domains
{
    /// <summary>
    /// ViewModel for editing value type properties
    /// </summary>
    public class ValueTypeEditViewModel : ViewModelBase
    {
        private ValueTypeArtifact? _valueType;
        private bool _isLoading;

        public ValueTypeEditViewModel()
        {
            NameField = new SingleLineTextFieldModel { Label = "Value Type Name", Name = nameof(ValueTypeArtifact.Name) };
            DescriptionField = new SingleLineTextFieldModel { Label = "Description", Name = nameof(ValueTypeArtifact.Description) };
            
            NameField.PropertyChanged += OnFieldChanged;
            DescriptionField.PropertyChanged += OnFieldChanged;
        }

        /// <summary>
        /// The value type being edited
        /// </summary>
        public ValueTypeArtifact? ValueType
        {
            get => _valueType;
            set
            {
                if (_valueType != null)
                {
                    _valueType.PropertyChanged -= ValueType_PropertyChanged;
                }
                if (SetProperty(ref _valueType, value))
                {
                    LoadFromValueType();
                    if (_valueType != null)
                    {
                        _valueType.PropertyChanged += ValueType_PropertyChanged;
                    }
                }
            }
        }

        private void ValueType_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ValueTypeArtifact.Name))
            {
                NameField.Value = _valueType?.Name;
            }
            else if (e.PropertyName == nameof(ValueTypeArtifact.Description))
            {
                DescriptionField.Value = _valueType?.Description;
            }
        }

        /// <summary>
        /// Value type name field
        /// </summary>
        public SingleLineTextFieldModel NameField { get; }

        /// <summary>
        /// Value type description field
        /// </summary>
        public SingleLineTextFieldModel DescriptionField { get; }

        /// <summary>
        /// Event raised when any field value changes
        /// </summary>
        public event EventHandler<ArtifactPropertyChangedEventArgs>? ValueChanged;

        private void LoadFromValueType()
        {
            if (_valueType == null) return;

            _isLoading = true;
            try
            {
                NameField.Value = _valueType.Name;
                DescriptionField.Value = _valueType.Description;
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void OnFieldChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading || _valueType == null) return;

            if (e.PropertyName == nameof(FieldViewModelBase.Value) && sender is FieldViewModelBase field)
            {
                SaveToValueType();
                ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_valueType, field.Name, field.Value));
            }
        }

        private void SaveToValueType()
        {
            if (_valueType == null) return;
            _valueType.Name = !string.IsNullOrWhiteSpace(NameField.Value as string) ? NameField.Value as string : "ValueType";
            _valueType.Description = DescriptionField.Value as string;
        }
    }
}
