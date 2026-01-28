using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.Events;
using CodeGenerator.Core.Artifacts.Views;
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
            ValueTypeReferenceField = new ComboboxFieldModel { Label = "Value Type", Name = nameof(PropertyArtifact.ValueTypeReferenceId) };
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
            ValueTypeReferenceField.PropertyChanged += OnValueTypeReferenceFieldChanged;
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
                    if (_valueTypesContainerToMonitor != null)
                    {
                        _valueTypesContainerToMonitor.ChildAdded -= ValueTypes_ChildAdded;
                        _valueTypesContainerToMonitor.ChildRemoved -= ValueTypes_ChildRemoved;
                        _valueTypesContainerToMonitor = null;
                    }
                }
                if (SetProperty(ref _property, value))
                {
                    LoadAvailableValueTypes();
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
        public ComboboxFieldModel ValueTypeReferenceField { get; }
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

        private bool _showValueTypeReference;
        public bool ShowValueTypeReference
        {
            get => _showValueTypeReference;
            set => SetProperty(ref _showValueTypeReference, value);
        }

        /// <summary>
        /// Event raised when any field value changes
        /// </summary>
        public event EventHandler<ArtifactPropertyChangedEventArgs>? ValueChanged;

        private void LoadAvailableValueTypes()
        {
            foreach(var item in ValueTypeReferenceField.Items)
            {
                if(item is ComboboxItem comboboxItem)
                {
                    comboboxItem.Dispose();
                }
            }
            ValueTypeReferenceField.Items.Clear();

            if (_property == null) return;

            // Get the domain containing this property
            var domain = _property.FindAncesterOfType<DomainArtifact>();

            if (domain == null) return;

            // Get all value types from the domain
            var valueTypes = domain.ValueTypes.GetValueTypes()
                .OrderBy(vt => vt.Name);

            foreach (var valueType in valueTypes)
            {
                ValueTypeReferenceField.Items.Add(new ArtifactComboboxItem(valueType));
            }
        }

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

                RefreshSelectedValueTypeReference();

                MonitoryValueTypesContainer();

                DescriptionField.Value = _property.Description;
                ExampleValueField.Value = _property.ExampleValue;

                UpdateFieldVisibility();
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void RefreshSelectedValueTypeReference()
        {
            // Set selected value type reference
            var selectedValueType = ValueTypeReferenceField.Items
                .FirstOrDefault(i => i.Value?.ToString() == _property.ValueTypeReferenceId);
            ValueTypeReferenceField.SelectedItem = selectedValueType;
        }

        private void MonitoryValueTypesContainer()
        {
            if (_valueTypesContainerToMonitor != null) {
                _valueTypesContainerToMonitor.ChildAdded -= ValueTypes_ChildAdded;
                _valueTypesContainerToMonitor.ChildRemoved -= ValueTypes_ChildRemoved;
                _valueTypesContainerToMonitor = null;
            }

            var domainArtifact = _property?.FindAncesterOfType<DomainArtifact>();

            if (domainArtifact != null && domainArtifact.ValueTypes != null)
            {
                _valueTypesContainerToMonitor = domainArtifact.ValueTypes;
                _valueTypesContainerToMonitor.ChildAdded += ValueTypes_ChildAdded;
                _valueTypesContainerToMonitor.ChildRemoved += ValueTypes_ChildRemoved;
            }
        }

        private ValueTypesContainerArtifact? _valueTypesContainerToMonitor;
        private void ValueTypes_ChildRemoved(object? sender, ChildRemovedEventArgs e)
        {
            _isLoading = true;
            try
            {
                // if a value type is removed, reload available value types
                LoadAvailableValueTypes();
                RefreshSelectedValueTypeReference();
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void ValueTypes_ChildAdded(object? sender, ChildAddedEventArgs e)
        {
            _isLoading = true;
            try
            {
                // if a value type is added, reload available value types
                LoadAvailableValueTypes();
                RefreshSelectedValueTypeReference();
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

        private void OnValueTypeReferenceFieldChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading || _property == null) return;

            if (e.PropertyName == nameof(ComboboxFieldModel.SelectedItem))
            {
                _property.ValueTypeReferenceId = ValueTypeReferenceField.SelectedItem?.Value?.ToString();
                ValidateValueTypeReference();
                ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_property, nameof(PropertyArtifact.ValueTypeReferenceId), _property.ValueTypeReferenceId));
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
                ValidateValueTypeReference();
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

        private void ValidateValueTypeReference()
        {
            if (ShowValueTypeReference)
            {
                if (ValueTypeReferenceField.SelectedItem == null)
                {
                    ValueTypeReferenceField.ErrorMessage = "A value type must be selected";
                }
                else
                {
                    ValueTypeReferenceField.ErrorMessage = null;
                }
            }
            else
            {
                ValueTypeReferenceField.ErrorMessage = null;
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
            
            // Show ValueTypeReference for value type reference types
            ShowValueTypeReference = GenericDataTypes.IsValueTypeReferenceType(dataTypeId);
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
            _property.ValueTypeReferenceId = ValueTypeReferenceField.SelectedItem?.Value?.ToString();
            _property.Description = DescriptionField.Value as string;
            _property.ExampleValue = ExampleValueField.Value as string;
        }


    }
}
