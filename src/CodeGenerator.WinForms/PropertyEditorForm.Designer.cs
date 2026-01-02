namespace CodeGenerator.WinForms;

partial class PropertyEditorForm
{
    private System.ComponentModel.IContainer components = null;

    private TabControl _tabControl;
    private TabPage _basicTab;
    private TabPage _dddTab;
    private TabPage _databaseTab;
    private TabPage _displayTab;
    private Panel _buttonPanel;
    
    private TextBox _nameTextBox;
    private ComboBox _typeComboBox;
    private TextBox _formatTextBox;
    private TextBox _descriptionTextBox;
    private CheckBox _requiredCheckBox;
    private CheckBox _nullableCheckBox;
    private NumericUpDown _minLengthNumeric;
    private NumericUpDown _maxLengthNumeric;
    private TextBox _patternTextBox;
    private TextBox _defaultValueTextBox;
    private TextBox _referenceTextBox;
    private CheckBox _optionalCheckBox;
    private CheckBox _selfReferenceCheckBox;
    private CheckBox _isPrimaryKeyCheckBox;
    private CheckBox _isIdentityCheckBox;
    private CheckBox _isForeignKeyCheckBox;
    private TextBox _columnNameTextBox;
    private TextBox _columnTypeTextBox;
    private TextBox _labelTextBox;
    private ComboBox _controlTypeComboBox;
    private Button _okButton;
    private Button _cancelButton;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();

        Text = string.IsNullOrEmpty(PropertyName) ? "Add Property" : $"Edit Property - {PropertyName}";
        Size = new Size(550, 650);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        _tabControl = new TabControl { Dock = DockStyle.Fill };

        _basicTab = new TabPage("Basic");
        _dddTab = new TabPage("DDD");
        _databaseTab = new TabPage("Database");
        _displayTab = new TabPage("Display");

        CreateBasicTab();
        CreateDDDTab();
        CreateDatabaseTab();
        CreateDisplayTab();

        _tabControl.TabPages.Add(_basicTab);
        _tabControl.TabPages.Add(_dddTab);
        _tabControl.TabPages.Add(_databaseTab);
        _tabControl.TabPages.Add(_displayTab);

        _buttonPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 50
        };

        _okButton = new Button
        {
            Text = "OK",
            DialogResult = DialogResult.OK,
            Location = new Point(Width - 200, 12),
            Size = new Size(80, 28)
        };
        _okButton.Click += OkButton_Click;

        _cancelButton = new Button
        {
            Text = "Cancel",
            DialogResult = DialogResult.Cancel,
            Location = new Point(Width - 110, 12),
            Size = new Size(80, 28)
        };

        _buttonPanel.Controls.AddRange(new Control[] { _okButton, _cancelButton });

        Controls.Add(_tabControl);
        Controls.Add(_buttonPanel);

        AcceptButton = _okButton;
        CancelButton = _cancelButton;
    }

    private void CreateBasicTab()
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

        _basicTab.Controls.Add(layout);
    }

    private void CreateDDDTab()
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

        layout.Controls.Add(new Label { Text = "Reference:", Anchor = AnchorStyles.Left }, 0, row);
        _referenceTextBox = new TextBox { Dock = DockStyle.Fill, PlaceholderText = "e.g. Country" };
        layout.Controls.Add(_referenceTextBox, 1, row++);

        _optionalCheckBox = new CheckBox { Text = "Optional (nullable relationship)", Anchor = AnchorStyles.Left };
        layout.Controls.Add(_optionalCheckBox, 1, row++);

        _selfReferenceCheckBox = new CheckBox { Text = "Self Reference (hierarchical)", Anchor = AnchorStyles.Left };
        layout.Controls.Add(_selfReferenceCheckBox, 1, row++);

        _dddTab.Controls.Add(layout);
    }

    private void CreateDatabaseTab()
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

        _databaseTab.Controls.Add(layout);
    }

    private void CreateDisplayTab()
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

        _displayTab.Controls.Add(layout);
    }
}
