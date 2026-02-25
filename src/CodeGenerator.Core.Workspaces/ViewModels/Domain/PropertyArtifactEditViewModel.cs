using CodeGenerator.Core.Artifacts.Events;
using CodeGenerator.Core.Artifacts.Views;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.ValueTypes;
using CodeGenerator.Core.Workspaces.ViewModels.Common;
using CodeGenerator.Domain.DataTypes;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Core.Workspaces.ViewModels.Domain
{
    public class PropertyArtifactEditViewModel : ArtifactEditViewModel<PropertyArtifact, PropertyGeneralTabViewModel>
    {
        public PropertyArtifactEditViewModel()
            : base("Property", new PropertyGeneralTabViewModel())
        {
        }
    }

    public class PropertyGeneralTabViewModel : ArtifactEditViewTabModel<PropertyArtifact>
    {
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

        private ValueTypesContainerArtifact? _valueTypesContainerToMonitor;

        public PropertyGeneralTabViewModel() : base("General")
        {
            NameField = new SingleLineTextFieldModel
            {
                Label = "Property Name",
                Name = nameof(PropertyArtifact.Name),
                Tooltip = "Name of the property",
                IsRequired = true,
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(NameField);

            DataTypeField = new ComboboxFieldModel
            {
                Label = "Data Type",
                Name = nameof(PropertyArtifact.DataType),
                Tooltip = "Data type of the property",
                AutoBind = true,
                AutoUpdate = true
            };
            DataTypeField.Items = GenericDataTypes.All
                .Select(dt => new ComboboxItem { DisplayName = dt.Name, Value = dt.Id })
                .ToList();
            FieldCollection.FieldModels.Add(DataTypeField);

            IsNullableField = new BooleanFieldModel
            {
                Label = "Is Nullable",
                Name = nameof(PropertyArtifact.IsNullable),
                Tooltip = "Whether the property allows null values",
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(IsNullableField);

            MaxLengthField = new IntegerFieldModel
            {
                Label = "Max Length",
                Name = nameof(PropertyArtifact.MaxLength),
                Tooltip = "Maximum length for text-based types",
                Minimum = 0,
                Maximum = int.MaxValue,
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(MaxLengthField);

            PrecisionField = new IntegerFieldModel
            {
                Label = "Precision",
                Name = nameof(PropertyArtifact.Precision),
                Tooltip = "Total number of digits (including decimals)",
                Minimum = 0,
                Maximum = int.MaxValue,
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(PrecisionField);

            ScaleField = new IntegerFieldModel
            {
                Label = "Scale",
                Name = nameof(PropertyArtifact.Scale),
                Tooltip = "Number of digits after comma",
                Minimum = 0,
                Maximum = int.MaxValue,
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(ScaleField);

            AllowedValuesField = new SingleLineTextFieldModel
            {
                Label = "Allowed Values",
                Name = nameof(PropertyArtifact.AllowedValues),
                Tooltip = "Comma-separated list of allowed values for enum type",
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(AllowedValuesField);

            ValueTypeReferenceField = new ComboboxFieldModel
            {
                Label = "Value Type",
                Name = nameof(PropertyArtifact.ValueTypeReferenceId),
                Tooltip = "Reference to a value type defined in the domain",
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(ValueTypeReferenceField);

            DescriptionField = new SingleLineTextFieldModel
            {
                Label = "Description",
                Name = nameof(PropertyArtifact.Description),
                Tooltip = "Description of the property",
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(DescriptionField);

            ExampleValueField = new SingleLineTextFieldModel
            {
                Label = "Example Value",
                Name = nameof(PropertyArtifact.ExampleValue),
                Tooltip = "Example value for documentation purposes",
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(ExampleValueField);

            DataTypeField.PropertyChanged += DataTypeField_PropertyChanged;
        }

        public override void BindArtifact(WorkspaceArtifactBase? artifactBase)
        {
            // Unsubscribe from old artifact events
            UnsubscribeFromValueTypesContainer();

            base.BindArtifact(artifactBase);

            if (Artifact != null)
            {
                LoadAvailableValueTypes();
                MonitorValueTypesContainer();
                UpdateFieldVisibility();
            }
        }

        protected override void OnArtifactPropertyChanged(PropertyArtifact artifact, string propertyName)
        {
            if (propertyName == nameof(PropertyArtifact.DataType))
            {
                UpdateFieldVisibility();
                ValidateAllowedValues();
                ValidateValueTypeReference();
            }
        }

        private void DataTypeField_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ComboboxFieldModel.SelectedItem))
            {
                UpdateFieldVisibility();
                ValidateAllowedValues();
                ValidateValueTypeReference();
            }
        }

        #region Field Visibility

        private void UpdateFieldVisibility()
        {
            var dataTypeId = Artifact?.DataType ?? string.Empty;

            MaxLengthField.Visible = GenericDataTypes.IsTextBasedType(dataTypeId);
            PrecisionField.Visible = dataTypeId == GenericDataTypes.Decimal.Id || dataTypeId == GenericDataTypes.Money.Id;
            ScaleField.Visible = dataTypeId == GenericDataTypes.Decimal.Id || dataTypeId == GenericDataTypes.Money.Id;
            AllowedValuesField.Visible = GenericDataTypes.IsEnumType(dataTypeId);
            ValueTypeReferenceField.Visible = GenericDataTypes.IsValueTypeReferenceType(dataTypeId);
        }

        #endregion

        #region Validation

        private void ValidateAllowedValues()
        {
            if (AllowedValuesField.Visible)
            {
                var allowedValues = AllowedValuesField.Value?.ToString();
                AllowedValuesField.ErrorMessage = string.IsNullOrWhiteSpace(allowedValues)
                    ? "At least one value is required for Enum-type"
                    : null;
            }
            else
            {
                AllowedValuesField.ErrorMessage = null;
            }
        }

        private void ValidateValueTypeReference()
        {
            if (ValueTypeReferenceField.Visible)
            {
                ValueTypeReferenceField.ErrorMessage = ValueTypeReferenceField.SelectedItem == null
                    ? "A value type must be selected"
                    : null;
            }
            else
            {
                ValueTypeReferenceField.ErrorMessage = null;
            }
        }

        #endregion

        #region Value Types Management

        private void LoadAvailableValueTypes()
        {
            foreach (var item in ValueTypeReferenceField.Items)
            {
                item.Dispose();
            }
            ValueTypeReferenceField.Items.Clear();

            if (Artifact == null) return;

            var domain = Artifact.FindAncesterOfType<DomainArtifact>();
            if (domain == null) return;

            var valueTypes = domain.ValueTypes.GetValueTypes()
                .OrderBy(vt => vt.Name);

            foreach (var valueType in valueTypes)
            {
                ValueTypeReferenceField.Items.Add(new ArtifactComboboxItem(valueType));
            }
        }

        private void MonitorValueTypesContainer()
        {
            UnsubscribeFromValueTypesContainer();

            var domainArtifact = Artifact?.FindAncesterOfType<DomainArtifact>();
            if (domainArtifact?.ValueTypes != null)
            {
                _valueTypesContainerToMonitor = domainArtifact.ValueTypes;
                _valueTypesContainerToMonitor.ChildAdded += ValueTypes_ChildChanged;
                _valueTypesContainerToMonitor.ChildRemoved += ValueTypes_ChildChanged;
            }
        }

        private void UnsubscribeFromValueTypesContainer()
        {
            if (_valueTypesContainerToMonitor != null)
            {
                _valueTypesContainerToMonitor.ChildAdded -= ValueTypes_ChildChanged;
                _valueTypesContainerToMonitor.ChildRemoved -= ValueTypes_ChildChanged;
                _valueTypesContainerToMonitor = null;
            }
        }

        private void ValueTypes_ChildChanged(object? sender, EventArgs e)
        {
            LoadAvailableValueTypes();

            // Re-select current value type reference if still available
            if (Artifact != null)
            {
                var selectedValueType = ValueTypeReferenceField.Items
                    .FirstOrDefault(i => i.Value?.ToString() == Artifact.ValueTypeReferenceId);
                ValueTypeReferenceField.SelectedItem = selectedValueType;
            }
        }

        #endregion
    }
}
