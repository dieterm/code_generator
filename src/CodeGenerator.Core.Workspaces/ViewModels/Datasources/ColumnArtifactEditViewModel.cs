using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Core.Workspaces.Models;
using CodeGenerator.Core.Workspaces.ViewModels.Common;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;
using System.Diagnostics;

namespace CodeGenerator.Core.Workspaces.ViewModels.Datasources
{
    public class ColumnArtifactEditViewModel : ArtifactEditViewModel<ColumnArtifact, ColumnGeneralTabViewModel>
    {
        public ColumnArtifactEditViewModel()
            : base("Column", new ColumnGeneralTabViewModel())
        {
        }

        /// <summary>
        /// Sets the available data types for the DataType combobox.
        /// Called by the controller after determining the datasource type.
        /// </summary>
        public void SetAvailableDataTypes(IEnumerable<DataTypeComboboxItem> dataTypes)
        {
            GeneralTab.SetAvailableDataTypes(dataTypes);
        }
    }

    public class ColumnGeneralTabViewModel : ArtifactEditViewTabModel<ColumnArtifact>
    {
        public SingleLineTextFieldModel NameField { get; }
        public ComboboxFieldModel DataTypeField { get; }
        public IntegerFieldModel MaxLengthField { get; }
        public IntegerFieldModel PrecisionField { get; }
        public IntegerFieldModel ScaleField { get; }
        public SingleLineTextFieldModel AllowedValuesField { get; }
        public BooleanFieldModel IsNullableField { get; }
        public BooleanFieldModel IsPrimaryKeyField { get; }
        public BooleanFieldModel IsAutoIncrementField { get; }
        public SingleLineTextFieldModel DefaultValueField { get; }

        public ColumnGeneralTabViewModel() : base("General")
        {
            NameField = new SingleLineTextFieldModel
            {
                Label = "Column Name",
                Name = nameof(ColumnArtifact.Name),
                Tooltip = "Name of the column",
                IsRequired = true,
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(NameField);

            DataTypeField = new ComboboxFieldModel
            {
                Label = "Data Type",
                Name = nameof(ColumnArtifact.DataType),
                Tooltip = "Data type of the column",
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(DataTypeField);

            MaxLengthField = new IntegerFieldModel
            {
                Label = "Max Length",
                Name = nameof(ColumnArtifact.MaxLength),
                Tooltip = "Maximum length for string types",
                Minimum = 0,
                Maximum = int.MaxValue,
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(MaxLengthField);

            PrecisionField = new IntegerFieldModel
            {
                Label = "Precision",
                Name = nameof(ColumnArtifact.Precision),
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
                Name = nameof(ColumnArtifact.Scale),
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
                Name = nameof(ColumnArtifact.AllowedValues),
                Tooltip = "Comma-separated list of allowed values for enum type",
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(AllowedValuesField);

            IsNullableField = new BooleanFieldModel
            {
                Label = "Nullable",
                Name = nameof(ColumnArtifact.IsNullable),
                Tooltip = "Whether the column allows null values",
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(IsNullableField);

            IsPrimaryKeyField = new BooleanFieldModel
            {
                Label = "Primary Key",
                Name = nameof(ColumnArtifact.IsPrimaryKey),
                Tooltip = "Whether the column is part of the primary key",
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(IsPrimaryKeyField);

            IsAutoIncrementField = new BooleanFieldModel
            {
                Label = "Auto Increment",
                Name = nameof(ColumnArtifact.IsAutoIncrement),
                Tooltip = "Whether the column auto-increments",
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(IsAutoIncrementField);

            DefaultValueField = new SingleLineTextFieldModel
            {
                Label = "Default Value",
                Name = nameof(ColumnArtifact.DefaultValue),
                Tooltip = "Default value expression for the column",
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(DefaultValueField);

            DataTypeField.PropertyChanged += DataTypeField_PropertyChanged;
        }

        public override void BindArtifact(WorkspaceArtifactBase? artifactBase)
        {
            base.BindArtifact(artifactBase);

            if (Artifact != null)
            {
                // Extract base data type for combobox matching
                var baseDataType = ExtractBaseDataType(Artifact.DataType);
                DataTypeField.Value = baseDataType;

                UpdateFieldVisibility(DataTypeField.SelectedItem as DataTypeComboboxItem);
            }
        }

        /// <summary>
        /// Sets the available data types for the DataType combobox.
        /// </summary>
        public void SetAvailableDataTypes(IEnumerable<DataTypeComboboxItem> dataTypes)
        {
            DataTypeField.Items = dataTypes.ToList<ComboboxItem>();

            // If we have a current value, try to reselect it
            if (Artifact != null && DataTypeField.Value != null)
            {
                var selectedItem = dataTypes.FirstOrDefault(i =>
                    string.Equals(i.Value?.ToString(), DataTypeField.Value?.ToString(), StringComparison.OrdinalIgnoreCase));
                if (selectedItem != null)
                {
                    DataTypeField.SelectedItem = selectedItem;
                }
            }
        }

        protected override void OnArtifactPropertyChanged(ColumnArtifact artifact, string propertyName)
        {
            if (propertyName == nameof(ColumnArtifact.DataType))
            {
                var baseDataType = ExtractBaseDataType(artifact.DataType);
                DataTypeField.Value = baseDataType;
                Debug.WriteLine($"DataType changed to {artifact.DataType}, extracted base type: {baseDataType}");
                UpdateFieldVisibility(DataTypeField.SelectedItem as DataTypeComboboxItem);
            }
        }

        private void DataTypeField_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ComboboxFieldModel.SelectedItem))
            {
                var dataType = DataTypeField.SelectedItem as DataTypeComboboxItem;
                if (dataType != null)
                {
                    DataTypeField.ErrorMessage = dataType.TypeNotes;
                    DataTypeField.Tooltip = dataType.TypeDescription;

                    UpdateFieldVisibility(dataType);
                    ValidateAllowedValues();
                }
            }
        }

        private void UpdateFieldVisibility(DataTypeComboboxItem? dataType)
        {
            MaxLengthField.Visible = dataType?.UseMaxLength ?? false;
            PrecisionField.Visible = dataType?.UsePrecision ?? false;
            ScaleField.Visible = dataType?.UseScale ?? false;
            AllowedValuesField.Visible = dataType?.UseAllowedValues ?? false;
        }

        private void ValidateAllowedValues()
        {
            var dataType = DataTypeField.SelectedItem as DataTypeComboboxItem;
            if (dataType != null && dataType.UseAllowedValues)
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

        private static string ExtractBaseDataType(string fullDataType)
        {
            if (string.IsNullOrEmpty(fullDataType)) return "varchar";

            var parenIndex = fullDataType.IndexOf('(');
            return parenIndex > 0 ? fullDataType.Substring(0, parenIndex).Trim() : fullDataType.Split(' ')[0].Trim();
        }
    }
}
