using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Domain.DataTypes;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Application.ViewModels.Workspace.Domains
{
    /// <summary>
    /// ViewModel for editing property artifact properties
    /// </summary>
    public class PropertyEditViewModel : ViewModelBase
    {
        private PropertyArtifact? _property;
        private bool _isLoading;

        public PropertyEditViewModel()
        {
            NameField = new SingleLineTextFieldModel { Label = "Property Name", Name = nameof(PropertyArtifact.Name) };
            DataTypeField = new ComboboxFieldModel { Label = "Data Type", Name = nameof(PropertyArtifact.DataType) };
            IsNullableField = new BooleanFieldModel { Label = "Is Nullable", Name = nameof(PropertyArtifact.IsNullable) };
            MaxLengthField = new IntegerFieldModel { Label = "Max Length", Name = nameof(PropertyArtifact.MaxLength), Minimum = 0, Maximum = int.MaxValue };
            PrecisionField = new IntegerFieldModel { Label = "Precision", Name = nameof(PropertyArtifact.Precision), Minimum = 0, Maximum = int.MaxValue };
            ScaleField = new IntegerFieldModel { Label = "Scale", Name = nameof(PropertyArtifact.Scale), Minimum = 0, Maximum = int.MaxValue };
            AllowedValuesField = new SingleLineTextFieldModel { Label = "Allowed Values", Name = nameof(PropertyArtifact.AllowedValues), Tooltip = "Comma-separated list of allowed values for enum type" };
            DescriptionField = new SingleLineTextFieldModel { Label = "Description", Name = nameof(PropertyArtifact.Description) };
            ExampleValueField = new SingleLineTextFieldModel { Label = "Example Value", Name = nameof(PropertyArtifact.ExampleValue) };

            // Initialize data type options
            DataTypeField.Items = GenericDataTypes.All
                .Select(dt => new ComboboxItem { DisplayName = dt.Name, Value = dt.Id })
                .ToList();

            NameField.PropertyChanged += OnFieldChanged;
            DataTypeField.PropertyChanged += OnDataTypeFieldChanged;
            IsNullableField.PropertyChanged += OnFieldChanged;
            MaxLengthField.PropertyChanged += OnFieldChanged;
            PrecisionField.PropertyChanged += OnFieldChanged;
            ScaleField.PropertyChanged += OnFieldChanged;
            AllowedValuesField.PropertyChanged += OnAllowedValuesFieldChanged;
            DescriptionField.PropertyChanged += OnFieldChanged;
            ExampleValueField.PropertyChanged += OnFieldChanged;
        }

        /// <summary>
        /// The property being edited
        /// </summary>
        public PropertyArtifact? Property
        {
            get => _property;
            set
            {
                if (_property != null)
                {
                    _property.PropertyChanged -= Property_PropertyChanged;
                }
                if (SetProperty(ref _property, value))
                {
                    LoadFromProperty();
                    if (_property != null)
                    {
                        _property.PropertyChanged += Property_PropertyChanged;
                    }
                }
            }
        }

        private void Property_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PropertyArtifact.Name))
            {
                NameField.Value = _property?.Name;
            }
        }

        // Field ViewModels
        public SingleLineTextFieldModel NameField { get; }
        public ComboboxFieldModel DataTypeField { get; }
        public BooleanFieldModel IsNullableField { get; }
        public IntegerFieldModel MaxLengthField { get; }
        public IntegerFieldModel PrecisionField { get; }
        public IntegerFieldModel ScaleField { get; }
        public SingleLineTextFieldModel AllowedValuesField { get; }
        public SingleLineTextFieldModel DescriptionField { get; }
        public SingleLineTextFieldModel ExampleValueField { get; }

        // Visibility properties for conditional fields
        private bool _showMaxLength;
        public bool ShowMaxLength
        {
            get => _showMaxLength;
            set => SetProperty(ref _showMaxLength, value);
        }

        private bool _showPrecisionScale;
        public bool ShowPrecisionScale
        {
            get => _showPrecisionScale;
            set => SetProperty(ref _showPrecisionScale, value);
        }

        private bool _showAllowedValues;
        public bool ShowAllowedValues
        {
            get => _showAllowedValues;
            set => SetProperty(ref _showAllowedValues, value);
        }

        /// <summary>
        /// Event raised when any field value changes
        /// </summary>
        public event EventHandler<ArtifactPropertyChangedEventArgs>? ValueChanged;

        private void LoadFromProperty()
        {
            if (_property == null) return;

            _isLoading = true;
            try
            {
                NameField.Value = _property.Name;
                
                var selectedDataType = DataTypeField.Items.FirstOrDefault(i => i.Value?.ToString() == _property.DataType);
                DataTypeField.SelectedItem = selectedDataType;
                
                IsNullableField.Value = _property.IsNullable;
                MaxLengthField.Value = _property.MaxLength;
                PrecisionField.Value = _property.Precision;
                ScaleField.Value = _property.Scale;
                AllowedValuesField.Value = _property.AllowedValues;
                DescriptionField.Value = _property.Description;
                ExampleValueField.Value = _property.ExampleValue;

                UpdateFieldVisibility();
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void OnFieldChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading || _property == null) return;

            if (e.PropertyName == nameof(FieldViewModelBase.Value) && sender is FieldViewModelBase field)
            {
                SaveToProperty();
                ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_property, field.Name, field.Value));
            }
        }

        private void OnAllowedValuesFieldChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading || _property == null) return;

            if (e.PropertyName == nameof(FieldViewModelBase.Value) && sender is FieldViewModelBase field)
            {
                ValidateAllowedValues();
                SaveToProperty();
                ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_property, field.Name, field.Value));
            }
        }

        private void OnDataTypeFieldChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading || _property == null) return;

            if (e.PropertyName == nameof(ComboboxFieldModel.SelectedItem))
            {
                SaveToProperty();
                UpdateFieldVisibility();
                ValidateAllowedValues();
                ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_property, nameof(PropertyArtifact.DataType), _property.DataType));
            }
        }

        private void ValidateAllowedValues()
        {
            if (ShowAllowedValues)
            {
                var allowedValues = AllowedValuesField.Value?.ToString();
                if (string.IsNullOrWhiteSpace(allowedValues))
                {
                    AllowedValuesField.ErrorMessage = "At least one value is required for Enum-type";
                }
                else
                {
                    AllowedValuesField.ErrorMessage = null;
                }
            }
            else
            {
                AllowedValuesField.ErrorMessage = null;
            }
        }

        private void UpdateFieldVisibility()
        {
            var dataTypeId = _property?.DataType ?? string.Empty;
            
            // Show MaxLength for text-based types
            ShowMaxLength = GenericDataTypes.IsTextBasedType(dataTypeId);
            
            // Show Precision/Scale for decimal types
            ShowPrecisionScale = dataTypeId == GenericDataTypes.Decimal.Id || dataTypeId == GenericDataTypes.Money.Id;
            
            // Show AllowedValues for enum types
            ShowAllowedValues = GenericDataTypes.IsEnumType(dataTypeId);
        }

        private void SaveToProperty()
        {
            if (_property == null) return;

            _property.Name = !string.IsNullOrWhiteSpace(NameField.Value as string) ? NameField.Value as string : "Property";
            _property.DataType = DataTypeField.SelectedItem?.Value?.ToString() ?? GenericDataTypes.VarChar.Id;
            _property.IsNullable = IsNullableField.Value is bool isNullable && isNullable;
            _property.MaxLength = MaxLengthField.Value as int?;
            _property.Precision = PrecisionField.Value as int?;
            _property.Scale = ScaleField.Value as int?;
            _property.AllowedValues = AllowedValuesField.Value as string;
            _property.Description = DescriptionField.Value as string;
            _property.ExampleValue = ExampleValueField.Value as string;
        }
    }
}
