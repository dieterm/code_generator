using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Core.Workspaces.Models;
using CodeGenerator.Domain.DataTypes;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CodeGenerator.Application.ViewModels.Workspace
{
    /// <summary>
    /// ViewModel for editing column properties
    /// </summary>
    public class ColumnEditViewModel : ViewModelBase
    {
        private ColumnArtifact? _column;
        private bool _isLoading;

        public IEnumerable<FieldViewModelBase> AllFields => new FieldViewModelBase[]
        {
            NameField,
            DataTypeField,
            MaxLengthField,
            PrecisionField,
            ScaleField,
            AllowedValuesField,
            IsNullableField,
            IsPrimaryKeyField,
            IsAutoIncrementField,
            DefaultValueField
        };

        public ColumnEditViewModel()
        {
            NameField = new SingleLineTextFieldModel { Label = "Column Name", Name = nameof(ColumnArtifact.Name) };
            DataTypeField = new ComboboxFieldModel { Label = "Data Type", Name = nameof(ColumnArtifact.DataType) };
            MaxLengthField = new IntegerFieldModel { Label = "Max Length", Name = nameof(ColumnArtifact.MaxLength), Minimum=0, Maximum=int.MaxValue };
            PrecisionField = new IntegerFieldModel { Label = "Precision", Name = nameof(ColumnArtifact.Precision), Minimum = 0, Maximum = int.MaxValue, Tooltip="Total number of digits (including decimals)" };
            ScaleField = new IntegerFieldModel { Label = "Scale", Name = nameof(ColumnArtifact.Scale), Minimum = 0, Maximum = int.MaxValue, Tooltip="Number of digits after comma" };
            AllowedValuesField = new SingleLineTextFieldModel { Label = "Allowed Values", Name = nameof(ColumnArtifact.AllowedValues), Tooltip = "Comma-separated list of allowed values for enum type" };
            IsNullableField = new BooleanFieldModel { Label = "Nullable", Name = nameof(ColumnArtifact.IsNullable) };
            IsPrimaryKeyField = new BooleanFieldModel { Label = "Primary Key", Name = nameof(ColumnArtifact.IsPrimaryKey) };
            IsAutoIncrementField = new BooleanFieldModel { Label = "Auto Increment", Name = nameof(ColumnArtifact.IsAutoIncrement) };
            DefaultValueField = new SingleLineTextFieldModel { Label = "Default Value", Name = nameof(ColumnArtifact.DefaultValue) };

            DataTypeField.PropertyChanged += DataTypeField_PropertyChanged;

            // Subscribe to field changes
            NameField.PropertyChanged += OnFieldChanged;
            DataTypeField.PropertyChanged += OnFieldChanged;
            MaxLengthField.PropertyChanged += OnFieldChanged;
            PrecisionField.PropertyChanged += OnFieldChanged;
            ScaleField.PropertyChanged += OnFieldChanged;
            AllowedValuesField.PropertyChanged += OnAllowedValuesFieldChanged;
            IsNullableField.PropertyChanged += OnFieldChanged;
            IsPrimaryKeyField.PropertyChanged += OnFieldChanged;
            IsAutoIncrementField.PropertyChanged += OnFieldChanged;
            DefaultValueField.PropertyChanged += OnFieldChanged;
        }

        private void DataTypeField_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ComboboxFieldModel.SelectedItem))
            {
                var dataType = DataTypeField.SelectedItem as DataTypeComboboxItem;
                if (dataType != null)
                {
                    DataTypeField.ErrorMessage = dataType.TypeNotes;
                    DataTypeField.Tooltip = dataType.TypeDescription;
                    
                    // Validate AllowedValues when data type changes
                    ValidateAllowedValues();
                }
            }
        }

        private void OnAllowedValuesFieldChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_isLoading || _column == null) return;

            if (e.PropertyName == nameof(FieldViewModelBase.Value) && sender is FieldViewModelBase field)
            {
                ValidateAllowedValues();
                SaveToColumn();
                ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_column, field.Name, field.Value));
            }
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

        /// <summary>
        /// The column being edited
        /// </summary>
        public ColumnArtifact? Column
        {
            get => _column;
            set
            {
                if(_column!=null)
                {
                    // remove old listener
                    _column.PropertyChanged -= ColumnArtifact_PropertyChanged;
                }
                if (SetProperty(ref _column, value))
                {
                    LoadFromColumn();
                    if (_column != null)
                    {
                        // add new listener
                        _column.PropertyChanged += ColumnArtifact_PropertyChanged;
                    }
                }
            }
        }

        private void ColumnArtifact_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            //reflect changes from the ColumnArtifact back to the ViewModel fields
            var fieldModel = AllFields.FirstOrDefault(f => f.Name == e.PropertyName);
            if (fieldModel != null)
            {
                fieldModel.Value = (sender as ColumnArtifact).GetValue<object>(e.PropertyName);
            }
        }

        // Field ViewModels
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
                AllowedValuesField.Value = _column.AllowedValues;
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

            _column.Name = NameField.Value?.ToString();
            _column.DataType = DataTypeField.Value?.ToString();
            _column.MaxLength = MaxLengthField.Value is int maxLen ? maxLen : null;
            _column.Precision = PrecisionField.Value is int prec ? prec : null;
            _column.Scale = ScaleField.Value is int scale ? scale : null;
            _column.AllowedValues = AllowedValuesField.Value?.ToString();
            _column.IsNullable = IsNullableField.Value is bool nullable && nullable;
            _column.IsPrimaryKey = IsPrimaryKeyField.Value is bool pk && pk;
            _column.IsAutoIncrement = IsAutoIncrementField.Value is bool autoInc && autoInc;
            _column.DefaultValue = DefaultValueField.Value?.ToString();
        }

        //private string BuildFullDataType()
        //{
        //    var baseType = DataTypeField.Value?.ToString() ?? "varchar";
            
        //    // Add length/precision based on type
        //    if (RequiresLength(baseType))
        //    {
        //        var length = MaxLengthField.Value is int len ? len : 255;
        //        return $"{baseType}({length})";
        //    }
        //    else if (RequiresPrecisionScale(baseType))
        //    {
        //        var precision = PrecisionField.Value is int prec ? prec : 18;
        //        var scale = ScaleField.Value is int s ? s : 2;
        //        return $"{baseType}({precision},{scale})";
        //    }

        //    return baseType;
        //}

        //private bool RequiresLength(string dataType)
        //{
        //    var lengthTypes = new[] { "char", "varchar", "nchar", "nvarchar", "binary", "varbinary" };
        //    return lengthTypes.Contains(dataType.ToLowerInvariant());
        //}

        //private bool RequiresPrecisionScale(string dataType)
        //{
        //    var precisionTypes = new[] { "decimal", "numeric" };
        //    return precisionTypes.Contains(dataType.ToLowerInvariant());
        //}
    }
}
