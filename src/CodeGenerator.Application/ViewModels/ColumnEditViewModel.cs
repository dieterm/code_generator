using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Core.Workspaces.Models;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CodeGenerator.Application.ViewModels
{
    /// <summary>
    /// ViewModel for editing column properties
    /// </summary>
    public class ColumnEditViewModel : ViewModelBase
    {
        private ColumnArtifact? _column;
        private bool _isLoading;

        public ColumnEditViewModel()
        {
            NameField = new SingleLineTextFieldModel { Label = "Column Name", Name = "Name" };
            DataTypeField = new ComboboxFieldModel { Label = "Data Type", Name = "DataType" };
            MaxLengthField = new IntegerFieldModel { Label = "Max Length", Name = "MaxLength", Minimum=0, Maximum=int.MaxValue };
            PrecisionField = new IntegerFieldModel { Label = "Precision", Name = "Precision", Minimum = 0, Maximum = int.MaxValue };
            ScaleField = new IntegerFieldModel { Label = "Scale", Name = "Scale", Minimum = 0, Maximum = int.MaxValue };
            IsNullableField = new BooleanFieldModel { Label = "Nullable", Name = "IsNullable" };
            IsPrimaryKeyField = new BooleanFieldModel { Label = "Primary Key", Name = "IsPrimaryKey" };
            IsAutoIncrementField = new BooleanFieldModel { Label = "Auto Increment", Name = "IsAutoIncrement" };
            DefaultValueField = new SingleLineTextFieldModel { Label = "Default Value", Name = "DefaultValue" };

            DataTypeField.PropertyChanged += DataTypeField_PropertyChanged;

            // Subscribe to field changes
            NameField.PropertyChanged += OnFieldChanged;
            DataTypeField.PropertyChanged += OnFieldChanged;
            MaxLengthField.PropertyChanged += OnFieldChanged;
            PrecisionField.PropertyChanged += OnFieldChanged;
            ScaleField.PropertyChanged += OnFieldChanged;
            IsNullableField.PropertyChanged += OnFieldChanged;
            IsPrimaryKeyField.PropertyChanged += OnFieldChanged;
            IsAutoIncrementField.PropertyChanged += OnFieldChanged;
            DefaultValueField.PropertyChanged += OnFieldChanged;
        }

        private void DataTypeField_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(ComboboxFieldModel.SelectedItem))
            {
                var dataType = DataTypeField.SelectedItem as DataTypeComboboxItem;
                if (dataType != null)
                {
                    DataTypeField.ErrorMessage = dataType.TypeNotes;
                    DataTypeField.Tooltip = dataType.TypeDescription;
                }
            }
        }

        /// <summary>
        /// The column being edited
        /// </summary>
        public ColumnArtifact? Column
        {
            get => _column;
            set
            {
                if (SetProperty(ref _column, value))
                {
                    LoadFromColumn();
                }
            }
        }

        // Field ViewModels
        public SingleLineTextFieldModel NameField { get; }
        public ComboboxFieldModel DataTypeField { get; }
        public IntegerFieldModel MaxLengthField { get; }
        public IntegerFieldModel PrecisionField { get; }
        public IntegerFieldModel ScaleField { get; }
        public BooleanFieldModel IsNullableField { get; }
        public BooleanFieldModel IsPrimaryKeyField { get; }
        public BooleanFieldModel IsAutoIncrementField { get; }
        public SingleLineTextFieldModel DefaultValueField { get; }

        public void SetAvailableDataTypes(IEnumerable<DataTypeComboboxItem> dataTypes)
        {
            DataTypeField.Items = dataTypes.ToList<ComboboxItem>();
            
            // If we have a current value, try to reselect it to ensure properties like UseMaxLength are respected
            if (Column != null && DataTypeField.Value != null)
            {
                var selectedItem = dataTypes.FirstOrDefault(i => string.Equals(i.Value?.ToString() ,DataTypeField.Value?.ToString(), StringComparison.OrdinalIgnoreCase));
                if (selectedItem != null)
                {
                    DataTypeField.SelectedItem = selectedItem;
                }
                 // This logic might be handled by the view binding, but if we need to update visibility of fields based on the new item properties:
                 // OnFieldChanged handles saving, but what about UI state?
                 // The view usually binds to the Item properties if the Combobox supports it, or we rely on the `BuildFullDataType` logic.
            }
        }

        /// <summary>
        /// Event raised when a property value changes
        /// </summary>
        public event EventHandler<ArtifactPropertyChangedEventArgs>? ValueChanged;

        private void LoadFromColumn()
        {
            if (_column == null) return;

            _isLoading = true;
            try
            {
                NameField.Value = _column.Name;
                DataTypeField.Value = ExtractBaseDataType(_column.DataType);
                MaxLengthField.Value = _column.MaxLength;
                PrecisionField.Value = _column.Precision;
                ScaleField.Value = _column.Scale;
                IsNullableField.Value = _column.IsNullable;
                IsPrimaryKeyField.Value = _column.IsPrimaryKey;
                IsAutoIncrementField.Value = _column.IsAutoIncrement;
                DefaultValueField.Value = _column.DefaultValue;
            }
            finally
            {
                _isLoading = false;
            }
        }

        private string ExtractBaseDataType(string fullDataType)
        {
            if (string.IsNullOrEmpty(fullDataType)) return "varchar";

            // Extract base type from full type like "varchar(255)"
            var parenIndex = fullDataType.IndexOf('(');
            return parenIndex > 0 ? fullDataType.Substring(0, parenIndex).Trim() : fullDataType.Split(' ')[0].Trim();
        }

        private void OnFieldChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_isLoading || _column == null) return;

            if (e.PropertyName == nameof(FieldViewModelBase.Value) && sender is FieldViewModelBase field)
            {
                SaveToColumn();
                ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_column, field.Name, field.Value));
            }
        }

        private void SaveToColumn()
        {
            if (_column == null) return;

            _column.Name = NameField.Value?.ToString() ?? "Column";
            _column.DataType = BuildFullDataType();
            _column.MaxLength = MaxLengthField.Value is int maxLen ? maxLen : null;
            _column.Precision = PrecisionField.Value is int prec ? prec : null;
            _column.Scale = ScaleField.Value is int scale ? scale : null;
            _column.IsNullable = IsNullableField.Value is bool nullable && nullable;
            _column.IsPrimaryKey = IsPrimaryKeyField.Value is bool pk && pk;
            _column.IsAutoIncrement = IsAutoIncrementField.Value is bool autoInc && autoInc;
            _column.DefaultValue = DefaultValueField.Value?.ToString();
        }

        private string BuildFullDataType()
        {
            var baseType = DataTypeField.Value?.ToString() ?? "varchar";
            
            // Add length/precision based on type
            if (RequiresLength(baseType))
            {
                var length = MaxLengthField.Value is int len ? len : 255;
                return $"{baseType}({length})";
            }
            else if (RequiresPrecisionScale(baseType))
            {
                var precision = PrecisionField.Value is int prec ? prec : 18;
                var scale = ScaleField.Value is int s ? s : 2;
                return $"{baseType}({precision},{scale})";
            }

            return baseType;
        }

        private bool RequiresLength(string dataType)
        {
            var lengthTypes = new[] { "char", "varchar", "nchar", "nvarchar", "binary", "varbinary" };
            return lengthTypes.Contains(dataType.ToLowerInvariant());
        }

        private bool RequiresPrecisionScale(string dataType)
        {
            var precisionTypes = new[] { "decimal", "numeric" };
            return precisionTypes.Contains(dataType.ToLowerInvariant());
        }
    }
}
