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

        // Form properties
        Text = "Add or Edit property";
        Size = new Size(550, 650);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        // TabControl
        _tabControl = new TabControl();
        _tabControl.Dock = DockStyle.Fill;

        // TabPages
        _basicTab = new TabPage();
        _basicTab.Text = "Basic";

        _dddTab = new TabPage();
        _dddTab.Text = "DDD";

        _databaseTab = new TabPage();
        _databaseTab.Text = "Database";

        _displayTab = new TabPage();
        _displayTab.Text = "Display";

        // Create tab contents
        CreateBasicTab();
        CreateDDDTab();
        CreateDatabaseTab();
        CreateDisplayTab();

        // Add tabs to TabControl
        _tabControl.TabPages.Add(_basicTab);
        _tabControl.TabPages.Add(_dddTab);
        _tabControl.TabPages.Add(_databaseTab);
        _tabControl.TabPages.Add(_displayTab);

        // Button Panel
        _buttonPanel = new Panel();
        _buttonPanel.Dock = DockStyle.Bottom;
        _buttonPanel.Height = 50;

        // OK Button
        _okButton = new Button();
        _okButton.Text = "OK";
        _okButton.DialogResult = DialogResult.OK;
        _okButton.Location = new Point(Width - 200, 12);
        _okButton.Size = new Size(80, 28);
        _okButton.Click += OkButton_Click;

        // Cancel Button
        _cancelButton = new Button();
        _cancelButton.Text = "Cancel";
        _cancelButton.DialogResult = DialogResult.Cancel;
        _cancelButton.Location = new Point(Width - 110, 12);
        _cancelButton.Size = new Size(80, 28);

        // Add buttons to panel
        _buttonPanel.Controls.Add(_okButton);
        _buttonPanel.Controls.Add(_cancelButton);

        // Add controls to form
        Controls.Add(_tabControl);
        Controls.Add(_buttonPanel);

        AcceptButton = _okButton;
        CancelButton = _cancelButton;
    }

    private void CreateBasicTab()
    {
        // Layout
        var layout = new TableLayoutPanel();
        layout.Dock = DockStyle.Fill;
        layout.Padding = new Padding(10);
        layout.ColumnCount = 2;
        layout.RowCount = 10;

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        int row = 0;

        // Name
        var nameLabel = new Label();
        nameLabel.Text = "Name:*";
        nameLabel.Anchor = AnchorStyles.Left;
        layout.Controls.Add(nameLabel, 0, row);

        _nameTextBox = new TextBox();
        _nameTextBox.Dock = DockStyle.Fill;
        layout.Controls.Add(_nameTextBox, 1, row++);

        // Type
        var typeLabel = new Label();
        typeLabel.Text = "Type:";
        typeLabel.Anchor = AnchorStyles.Left;
        layout.Controls.Add(typeLabel, 0, row);

        _typeComboBox = new ComboBox();
        _typeComboBox.Dock = DockStyle.Fill;
        _typeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _typeComboBox.Items.Add("string");
        _typeComboBox.Items.Add("integer");
        _typeComboBox.Items.Add("number");
        _typeComboBox.Items.Add("boolean");
        _typeComboBox.Items.Add("array");
        _typeComboBox.Items.Add("object");
        _typeComboBox.SelectedIndex = 0;
        layout.Controls.Add(_typeComboBox, 1, row++);

        // Format
        var formatLabel = new Label();
        formatLabel.Text = "Format:";
        formatLabel.Anchor = AnchorStyles.Left;
        layout.Controls.Add(formatLabel, 0, row);

        _formatTextBox = new TextBox();
        _formatTextBox.Dock = DockStyle.Fill;
        layout.Controls.Add(_formatTextBox, 1, row++);

        // Description
        var descriptionLabel = new Label();
        descriptionLabel.Text = "Description:";
        descriptionLabel.Anchor = AnchorStyles.Left;
        layout.Controls.Add(descriptionLabel, 0, row);

        _descriptionTextBox = new TextBox();
        _descriptionTextBox.Dock = DockStyle.Fill;
        _descriptionTextBox.Multiline = true;
        _descriptionTextBox.Height = 50;
        layout.Controls.Add(_descriptionTextBox, 1, row++);

        // Required
        _requiredCheckBox = new CheckBox();
        _requiredCheckBox.Text = "Required";
        _requiredCheckBox.Anchor = AnchorStyles.Left;
        layout.Controls.Add(_requiredCheckBox, 1, row++);

        // Nullable
        _nullableCheckBox = new CheckBox();
        _nullableCheckBox.Text = "Nullable";
        _nullableCheckBox.Anchor = AnchorStyles.Left;
        layout.Controls.Add(_nullableCheckBox, 1, row++);

        // Min Length
        var minLengthLabel = new Label();
        minLengthLabel.Text = "Min Length:";
        minLengthLabel.Anchor = AnchorStyles.Left;
        layout.Controls.Add(minLengthLabel, 0, row);

        _minLengthNumeric = new NumericUpDown();
        _minLengthNumeric.Dock = DockStyle.Fill;
        _minLengthNumeric.Maximum = 10000;
        layout.Controls.Add(_minLengthNumeric, 1, row++);

        // Max Length
        var maxLengthLabel = new Label();
        maxLengthLabel.Text = "Max Length:";
        maxLengthLabel.Anchor = AnchorStyles.Left;
        layout.Controls.Add(maxLengthLabel, 0, row);

        _maxLengthNumeric = new NumericUpDown();
        _maxLengthNumeric.Dock = DockStyle.Fill;
        _maxLengthNumeric.Maximum = 10000;
        layout.Controls.Add(_maxLengthNumeric, 1, row++);

        // Pattern
        var patternLabel = new Label();
        patternLabel.Text = "Pattern (Regex):";
        patternLabel.Anchor = AnchorStyles.Left;
        layout.Controls.Add(patternLabel, 0, row);

        _patternTextBox = new TextBox();
        _patternTextBox.Dock = DockStyle.Fill;
        layout.Controls.Add(_patternTextBox, 1, row++);

        // Default Value
        var defaultValueLabel = new Label();
        defaultValueLabel.Text = "Default Value:";
        defaultValueLabel.Anchor = AnchorStyles.Left;
        layout.Controls.Add(defaultValueLabel, 0, row);

        _defaultValueTextBox = new TextBox();
        _defaultValueTextBox.Dock = DockStyle.Fill;
        layout.Controls.Add(_defaultValueTextBox, 1, row++);

        _basicTab.Controls.Add(layout);
    }

    private void CreateDDDTab()
    {
        // Layout
        var layout = new TableLayoutPanel();
        layout.Dock = DockStyle.Fill;
        layout.Padding = new Padding(10);
        layout.ColumnCount = 2;
        layout.RowCount = 4;

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        int row = 0;

        // Reference
        var referenceLabel = new Label();
        referenceLabel.Text = "Reference:";
        referenceLabel.Anchor = AnchorStyles.Left;
        layout.Controls.Add(referenceLabel, 0, row);

        _referenceTextBox = new TextBox();
        _referenceTextBox.Dock = DockStyle.Fill;
        _referenceTextBox.PlaceholderText = "e.g. Country";
        layout.Controls.Add(_referenceTextBox, 1, row++);

        // Optional
        _optionalCheckBox = new CheckBox();
        _optionalCheckBox.Text = "Optional (nullable relationship)";
        _optionalCheckBox.Anchor = AnchorStyles.Left;
        layout.Controls.Add(_optionalCheckBox, 1, row++);

        // Self Reference
        _selfReferenceCheckBox = new CheckBox();
        _selfReferenceCheckBox.Text = "Self Reference (hierarchical)";
        _selfReferenceCheckBox.Anchor = AnchorStyles.Left;
        layout.Controls.Add(_selfReferenceCheckBox, 1, row++);

        _dddTab.Controls.Add(layout);
    }

    private void CreateDatabaseTab()
    {
        // Layout
        var layout = new TableLayoutPanel();
        layout.Dock = DockStyle.Fill;
        layout.Padding = new Padding(10);
        layout.ColumnCount = 2;
        layout.RowCount = 6;

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        int row = 0;

        // Column Name
        var columnNameLabel = new Label();
        columnNameLabel.Text = "Column Name:";
        columnNameLabel.Anchor = AnchorStyles.Left;
        layout.Controls.Add(columnNameLabel, 0, row);

        _columnNameTextBox = new TextBox();
        _columnNameTextBox.Dock = DockStyle.Fill;
        layout.Controls.Add(_columnNameTextBox, 1, row++);

        // Column Type
        var columnTypeLabel = new Label();
        columnTypeLabel.Text = "Column Type:";
        columnTypeLabel.Anchor = AnchorStyles.Left;
        layout.Controls.Add(columnTypeLabel, 0, row);

        _columnTypeTextBox = new TextBox();
        _columnTypeTextBox.Dock = DockStyle.Fill;
        layout.Controls.Add(_columnTypeTextBox, 1, row++);

        // Is Primary Key
        _isPrimaryKeyCheckBox = new CheckBox();
        _isPrimaryKeyCheckBox.Text = "Is Primary Key";
        _isPrimaryKeyCheckBox.Anchor = AnchorStyles.Left;
        layout.Controls.Add(_isPrimaryKeyCheckBox, 1, row++);

        // Is Identity
        _isIdentityCheckBox = new CheckBox();
        _isIdentityCheckBox.Text = "Is Identity (Auto-increment)";
        _isIdentityCheckBox.Anchor = AnchorStyles.Left;
        layout.Controls.Add(_isIdentityCheckBox, 1, row++);

        // Is Foreign Key
        _isForeignKeyCheckBox = new CheckBox();
        _isForeignKeyCheckBox.Text = "Is Foreign Key";
        _isForeignKeyCheckBox.Anchor = AnchorStyles.Left;
        layout.Controls.Add(_isForeignKeyCheckBox, 1, row++);

        _databaseTab.Controls.Add(layout);
    }

    private void CreateDisplayTab()
    {
        // Layout
        var layout = new TableLayoutPanel();
        layout.Dock = DockStyle.Fill;
        layout.Padding = new Padding(10);
        layout.ColumnCount = 2;
        layout.RowCount = 4;

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        int row = 0;

        // Display Label
        var labelLabel = new Label();
        labelLabel.Text = "Display Label:";
        labelLabel.Anchor = AnchorStyles.Left;
        layout.Controls.Add(labelLabel, 0, row);

        _labelTextBox = new TextBox();
        _labelTextBox.Dock = DockStyle.Fill;
        layout.Controls.Add(_labelTextBox, 1, row++);

        // Control Type
        var controlTypeLabel = new Label();
        controlTypeLabel.Text = "Control Type:";
        controlTypeLabel.Anchor = AnchorStyles.Left;
        layout.Controls.Add(controlTypeLabel, 0, row);

        _controlTypeComboBox = new ComboBox();
        _controlTypeComboBox.Dock = DockStyle.Fill;
        _controlTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _controlTypeComboBox.Items.Add("(Auto)");
        _controlTypeComboBox.Items.Add("TextBox");
        _controlTypeComboBox.Items.Add("TextArea");
        _controlTypeComboBox.Items.Add("NumericUpDown");
        _controlTypeComboBox.Items.Add("CheckBox");
        _controlTypeComboBox.Items.Add("ComboBox");
        _controlTypeComboBox.Items.Add("DatePicker");
        _controlTypeComboBox.Items.Add("DateTimePicker");
        _controlTypeComboBox.Items.Add("TimePicker");
        _controlTypeComboBox.SelectedIndex = 0;
        layout.Controls.Add(_controlTypeComboBox, 1, row++);

        _displayTab.Controls.Add(layout);
    }
}
