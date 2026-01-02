using CodeGenerator.Core.Models.Schema;

namespace CodeGenerator.WinForms;

public partial class PropertyEditorForm : Form
{
    public string PropertyName { get; private set; } = string.Empty;
    public PropertyDefinition Property { get; private set; } = new();

    public PropertyEditorForm(string? existingName = null, PropertyDefinition? existingProperty = null)
    {
        if (existingName != null) PropertyName = existingName;
        if (existingProperty != null) Property = existingProperty;

        InitializeComponent();

        if (existingProperty != null)
        {
            LoadProperty();
        }
    }


    private void LoadProperty()
    {
        Text = string.IsNullOrEmpty(PropertyName) ? "Add Property" : $"Edit Property - {PropertyName}";
        _nameTextBox.Text = PropertyName;
        _typeComboBox.Text = Property.Type ?? "string";
        _formatTextBox.Text = Property.Format;
        _descriptionTextBox.Text = Property.Description;
        _requiredCheckBox.Checked = !(Property.CodeGenMetadata?.IsNullable ?? true);
        _nullableCheckBox.Checked = Property.CodeGenMetadata?.IsNullable ?? false;
        _minLengthNumeric.Value = Property.MinLength ?? 0;
        _maxLengthNumeric.Value = Property.MaxLength ?? 0;
        _patternTextBox.Text = Property.Pattern;
        _defaultValueTextBox.Text = Property.Default?.ToString();

        _columnNameTextBox.Text = Property.DatabaseMetadata?.ColumnName;
        _columnTypeTextBox.Text = Property.DatabaseMetadata?.ColumnType;
        _isPrimaryKeyCheckBox.Checked = Property.DatabaseMetadata?.IsPrimaryKey ?? false;
        _isIdentityCheckBox.Checked = Property.DatabaseMetadata?.IsIdentity ?? false;
        _isForeignKeyCheckBox.Checked = Property.DatabaseMetadata?.IsForeignKey ?? false;

        _labelTextBox.Text = Property.CodeGenMetadata?.DisplaySettings?.Label;
        _controlTypeComboBox.Text = Property.CodeGenMetadata?.DisplaySettings?.ControlType ?? "(Auto)";
    }

    private void OkButton_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_nameTextBox.Text))
        {
            MessageBox.Show("Property name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            DialogResult = DialogResult.None;
            return;
        }

        PropertyName = _nameTextBox.Text.Trim();

        Property = new PropertyDefinition
        {
            Type = _typeComboBox.Text,
            Format = string.IsNullOrEmpty(_formatTextBox.Text) ? null : _formatTextBox.Text,
            Description = _descriptionTextBox.Text,
            MinLength = (int)_minLengthNumeric.Value > 0 ? (int)_minLengthNumeric.Value : null,
            MaxLength = (int)_maxLengthNumeric.Value > 0 ? (int)_maxLengthNumeric.Value : null,
            Pattern = string.IsNullOrEmpty(_patternTextBox.Text) ? null : _patternTextBox.Text,
            Default = string.IsNullOrEmpty(_defaultValueTextBox.Text) ? null : _defaultValueTextBox.Text,
            DatabaseMetadata = new PropertyDatabaseMetadata
            {
                ColumnName = string.IsNullOrEmpty(_columnNameTextBox.Text) ? PropertyName : _columnNameTextBox.Text,
                ColumnType = string.IsNullOrEmpty(_columnTypeTextBox.Text) ? null : _columnTypeTextBox.Text,
                IsPrimaryKey = _isPrimaryKeyCheckBox.Checked,
                IsIdentity = _isIdentityCheckBox.Checked,
                IsForeignKey = _isForeignKeyCheckBox.Checked
            },
            CodeGenMetadata = new PropertyCodeGenMetadata
            {
                PropertyName = PropertyName,
                IsNullable = _nullableCheckBox.Checked,
                DisplaySettings = new DisplaySettings
                {
                    Label = string.IsNullOrEmpty(_labelTextBox.Text) ? PropertyName : _labelTextBox.Text,
                    ControlType = _controlTypeComboBox.Text == "(Auto)" ? null : _controlTypeComboBox.Text
                }
            },
            DomainDrivenDesignMetadata = new PropertyDomainDrivenDesignMetadata
            {
                Optional = _optionalCheckBox.Checked,
                Reference = _referenceTextBox.Text,
                SelfReference = _selfReferenceCheckBox.Checked
            }
        };
    }
}
