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

    private void CreateBasicTab(TabPage tab)
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(10),
            ColumnCount = 2,
            RowCount = 10
        };

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        int row = 0;

        layout.Controls.Add(new Label { Text = "Name:*", Anchor = AnchorStyles.Left }, 0, row);
        _nameTextBox = new TextBox { Dock = DockStyle.Fill };
        layout.Controls.Add(_nameTextBox, 1, row++);

        layout.Controls.Add(new Label { Text = "Type:", Anchor = AnchorStyles.Left }, 0, row);
        _typeComboBox = new ComboBox
        {
            Dock = DockStyle.Fill,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        _typeComboBox.Items.AddRange(new object[] { "string", "integer", "number", "boolean", "array", "object" });
        _typeComboBox.SelectedIndex = 0;
        layout.Controls.Add(_typeComboBox, 1, row++);

        layout.Controls.Add(new Label { Text = "Format:", Anchor = AnchorStyles.Left }, 0, row);
        _formatTextBox = new TextBox { Dock = DockStyle.Fill };
        layout.Controls.Add(_formatTextBox, 1, row++);

        layout.Controls.Add(new Label { Text = "Description:", Anchor = AnchorStyles.Left }, 0, row);
        _descriptionTextBox = new TextBox { Dock = DockStyle.Fill, Multiline = true, Height = 50 };
        layout.Controls.Add(_descriptionTextBox, 1, row++);

        _requiredCheckBox = new CheckBox { Text = "Required", Anchor = AnchorStyles.Left };
        layout.Controls.Add(_requiredCheckBox, 1, row++);

        _nullableCheckBox = new CheckBox { Text = "Nullable", Anchor = AnchorStyles.Left };
        layout.Controls.Add(_nullableCheckBox, 1, row++);

        layout.Controls.Add(new Label { Text = "Min Length:", Anchor = AnchorStyles.Left }, 0, row);
        _minLengthNumeric = new NumericUpDown { Dock = DockStyle.Fill, Maximum = 10000 };
        layout.Controls.Add(_minLengthNumeric, 1, row++);

        layout.Controls.Add(new Label { Text = "Max Length:", Anchor = AnchorStyles.Left }, 0, row);
        _maxLengthNumeric = new NumericUpDown { Dock = DockStyle.Fill, Maximum = 10000 };
        layout.Controls.Add(_maxLengthNumeric, 1, row++);

        layout.Controls.Add(new Label { Text = "Pattern (Regex):", Anchor = AnchorStyles.Left }, 0, row);
        _patternTextBox = new TextBox { Dock = DockStyle.Fill };
        layout.Controls.Add(_patternTextBox, 1, row++);

        layout.Controls.Add(new Label { Text = "Default Value:", Anchor = AnchorStyles.Left }, 0, row);
        _defaultValueTextBox = new TextBox { Dock = DockStyle.Fill };
        layout.Controls.Add(_defaultValueTextBox, 1, row++);

        tab.Controls.Add(layout);
    }

    private void CreateDatabaseTab(TabPage tab)
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(10),
            ColumnCount = 2,
            RowCount = 6
        };

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        int row = 0;

        layout.Controls.Add(new Label { Text = "Column Name:", Anchor = AnchorStyles.Left }, 0, row);
        _columnNameTextBox = new TextBox { Dock = DockStyle.Fill };
        layout.Controls.Add(_columnNameTextBox, 1, row++);

        layout.Controls.Add(new Label { Text = "Column Type:", Anchor = AnchorStyles.Left }, 0, row);
        _columnTypeTextBox = new TextBox { Dock = DockStyle.Fill };
        layout.Controls.Add(_columnTypeTextBox, 1, row++);

        _isPrimaryKeyCheckBox = new CheckBox { Text = "Is Primary Key", Anchor = AnchorStyles.Left };
        layout.Controls.Add(_isPrimaryKeyCheckBox, 1, row++);

        _isIdentityCheckBox = new CheckBox { Text = "Is Identity (Auto-increment)", Anchor = AnchorStyles.Left };
        layout.Controls.Add(_isIdentityCheckBox, 1, row++);

        _isForeignKeyCheckBox = new CheckBox { Text = "Is Foreign Key", Anchor = AnchorStyles.Left };
        layout.Controls.Add(_isForeignKeyCheckBox, 1, row++);

        tab.Controls.Add(layout);
    }

    private void CreateDisplayTab(TabPage tab)
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(10),
            ColumnCount = 2,
            RowCount = 4
        };

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        int row = 0;

        layout.Controls.Add(new Label { Text = "Display Label:", Anchor = AnchorStyles.Left }, 0, row);
        _labelTextBox = new TextBox { Dock = DockStyle.Fill };
        layout.Controls.Add(_labelTextBox, 1, row++);

        layout.Controls.Add(new Label { Text = "Control Type:", Anchor = AnchorStyles.Left }, 0, row);
        _controlTypeComboBox = new ComboBox
        {
            Dock = DockStyle.Fill,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        _controlTypeComboBox.Items.AddRange(new object[]
        {
            "(Auto)", "TextBox", "TextArea", "NumericUpDown", "CheckBox",
            "ComboBox", "DatePicker", "DateTimePicker", "TimePicker"
        });
        _controlTypeComboBox.SelectedIndex = 0;
        layout.Controls.Add(_controlTypeComboBox, 1, row++);

        tab.Controls.Add(layout);
    }

    private void LoadProperty()
    {
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
            }
        };
    }
}
