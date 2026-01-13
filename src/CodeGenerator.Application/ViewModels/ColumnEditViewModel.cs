using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Core.Workspaces.Models;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.Collections.Generic;

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

            // Common data types
            DataTypeField.Items = new List<DataTypeComboboxItem>
            {
                new DataTypeComboboxItem { DisplayName = "int", Value = "int" },
                new DataTypeComboboxItem { DisplayName = "bigint", Value = "bigint" },
                new DataTypeComboboxItem { DisplayName = "smallint", Value = "smallint" },
                new DataTypeComboboxItem { DisplayName = "tinyint", Value = "tinyint" },
                new DataTypeComboboxItem { DisplayName = "bit", Value = "bit" },
                new DataTypeComboboxItem { DisplayName = "decimal", Value = "decimal" },
                new DataTypeComboboxItem { DisplayName = "numeric", Value = "numeric" },
                new DataTypeComboboxItem { DisplayName = "float", Value = "float" },
                new DataTypeComboboxItem { DisplayName = "real", Value = "real" },
                new DataTypeComboboxItem { DisplayName = "money", Value = "money" },
                new DataTypeComboboxItem { DisplayName = "char", Value = "char" },
                new DataTypeComboboxItem { DisplayName = "varchar", Value = "varchar", UseMaxLength= true },
                new DataTypeComboboxItem { DisplayName = "nchar", Value = "nchar" },
                new DataTypeComboboxItem { DisplayName = "nvarchar", Value = "nvarchar", UseMaxLength= true },
                new DataTypeComboboxItem { DisplayName = "text", Value = "text" },
                new DataTypeComboboxItem { DisplayName = "ntext", Value = "ntext" },
                new DataTypeComboboxItem { DisplayName = "date", Value = "date" },
                new DataTypeComboboxItem { DisplayName = "time", Value = "time" },
                new DataTypeComboboxItem { DisplayName = "datetime", Value = "datetime" },
                new DataTypeComboboxItem { DisplayName = "datetime2", Value = "datetime2" },
                new DataTypeComboboxItem { DisplayName = "timestamp", Value = "timestamp" },
                new DataTypeComboboxItem { DisplayName = "binary", Value = "binary" },
                new DataTypeComboboxItem { DisplayName = "varbinary", Value = "varbinary" },
                new DataTypeComboboxItem { DisplayName = "image", Value = "image" },
                new DataTypeComboboxItem { DisplayName = "uniqueidentifier", Value = "uniqueidentifier" },
                new DataTypeComboboxItem { DisplayName = "xml", Value = "xml" },
                new DataTypeComboboxItem { DisplayName = "json", Value = "json" },
            };

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
