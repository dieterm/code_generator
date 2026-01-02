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
    
    // Basic Tab Controls
    private TableLayoutPanel _basicTabLayout;
    private Label _nameLabel;
    private TextBox _nameTextBox;
    private Label _typeLabel;
    private ComboBox _typeComboBox;
    private Label _formatLabel;
    private TextBox _formatTextBox;
    private Label _descriptionLabel;
    private TextBox _descriptionTextBox;
    private CheckBox _requiredCheckBox;
    private CheckBox _nullableCheckBox;
    private Label _minLengthLabel;
    private NumericUpDown _minLengthNumeric;
    private Label _maxLengthLabel;
    private NumericUpDown _maxLengthNumeric;
    private Label _patternLabel;
    private TextBox _patternTextBox;
    private Label _defaultValueLabel;
    private TextBox _defaultValueTextBox;
    
    // DDD Tab Controls
    private TableLayoutPanel _dddTabLayout;
    private Label _referenceLabel;
    private TextBox _referenceTextBox;
    private CheckBox _optionalCheckBox;
    private CheckBox _selfReferenceCheckBox;
    
    // Database Tab Controls
    private TableLayoutPanel _databaseTabLayout;
    private Label _columnNameLabel;
    private TextBox _columnNameTextBox;
    private Label _columnTypeLabel;
    private TextBox _columnTypeTextBox;
    private CheckBox _isPrimaryKeyCheckBox;
    private CheckBox _isIdentityCheckBox;
    private CheckBox _isForeignKeyCheckBox;
    
    // Display Tab Controls
    private TableLayoutPanel _displayTabLayout;
    private Label _displayLabelLabel;
    private TextBox _labelTextBox;
    private Label _controlTypeLabel;
    private ComboBox _controlTypeComboBox;
    
    // Buttons
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
        _tabControl = new TabControl();
        _basicTab = new TabPage();
        _basicTabLayout = new TableLayoutPanel();
        _nameLabel = new Label();
        _nameTextBox = new TextBox();
        _typeLabel = new Label();
        _typeComboBox = new ComboBox();
        _formatLabel = new Label();
        _formatTextBox = new TextBox();
        _descriptionLabel = new Label();
        _descriptionTextBox = new TextBox();
        _requiredCheckBox = new CheckBox();
        _nullableCheckBox = new CheckBox();
        _minLengthLabel = new Label();
        _minLengthNumeric = new NumericUpDown();
        _maxLengthLabel = new Label();
        _maxLengthNumeric = new NumericUpDown();
        _patternLabel = new Label();
        _patternTextBox = new TextBox();
        _defaultValueLabel = new Label();
        _defaultValueTextBox = new TextBox();
        _dddTab = new TabPage();
        _dddTabLayout = new TableLayoutPanel();
        _referenceLabel = new Label();
        _referenceTextBox = new TextBox();
        _optionalCheckBox = new CheckBox();
        _selfReferenceCheckBox = new CheckBox();
        _databaseTab = new TabPage();
        _databaseTabLayout = new TableLayoutPanel();
        _columnNameLabel = new Label();
        _columnNameTextBox = new TextBox();
        _columnTypeLabel = new Label();
        _columnTypeTextBox = new TextBox();
        _isPrimaryKeyCheckBox = new CheckBox();
        _isIdentityCheckBox = new CheckBox();
        _isForeignKeyCheckBox = new CheckBox();
        _displayTab = new TabPage();
        _displayTabLayout = new TableLayoutPanel();
        _displayLabelLabel = new Label();
        _labelTextBox = new TextBox();
        _controlTypeLabel = new Label();
        _controlTypeComboBox = new ComboBox();
        _buttonPanel = new Panel();
        _okButton = new Button();
        _cancelButton = new Button();
        _tabControl.SuspendLayout();
        _basicTab.SuspendLayout();
        _basicTabLayout.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_minLengthNumeric).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_maxLengthNumeric).BeginInit();
        _dddTab.SuspendLayout();
        _dddTabLayout.SuspendLayout();
        _databaseTab.SuspendLayout();
        _databaseTabLayout.SuspendLayout();
        _displayTab.SuspendLayout();
        _displayTabLayout.SuspendLayout();
        _buttonPanel.SuspendLayout();
        SuspendLayout();
        // 
        // _tabControl
        // 
        _tabControl.Controls.Add(_basicTab);
        _tabControl.Controls.Add(_dddTab);
        _tabControl.Controls.Add(_databaseTab);
        _tabControl.Controls.Add(_displayTab);
        _tabControl.Dock = DockStyle.Fill;
        _tabControl.Location = new Point(0, 0);
        _tabControl.Name = "_tabControl";
        _tabControl.SelectedIndex = 0;
        _tabControl.Size = new Size(534, 561);
        _tabControl.TabIndex = 0;
        // 
        // _basicTab
        // 
        _basicTab.Controls.Add(_basicTabLayout);
        _basicTab.Location = new Point(4, 24);
        _basicTab.Name = "_basicTab";
        _basicTab.Size = new Size(526, 533);
        _basicTab.TabIndex = 0;
        _basicTab.Text = "Basic";
        // 
        // _basicTabLayout
        // 
        _basicTabLayout.ColumnCount = 2;
        _basicTabLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
        _basicTabLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _basicTabLayout.Controls.Add(_nameLabel, 0, 0);
        _basicTabLayout.Controls.Add(_nameTextBox, 1, 0);
        _basicTabLayout.Controls.Add(_typeLabel, 0, 1);
        _basicTabLayout.Controls.Add(_typeComboBox, 1, 1);
        _basicTabLayout.Controls.Add(_formatLabel, 0, 2);
        _basicTabLayout.Controls.Add(_formatTextBox, 1, 2);
        _basicTabLayout.Controls.Add(_descriptionLabel, 0, 3);
        _basicTabLayout.Controls.Add(_descriptionTextBox, 1, 3);
        _basicTabLayout.Controls.Add(_requiredCheckBox, 1, 4);
        _basicTabLayout.Controls.Add(_nullableCheckBox, 1, 5);
        _basicTabLayout.Controls.Add(_minLengthLabel, 0, 6);
        _basicTabLayout.Controls.Add(_minLengthNumeric, 1, 6);
        _basicTabLayout.Controls.Add(_maxLengthLabel, 0, 7);
        _basicTabLayout.Controls.Add(_maxLengthNumeric, 1, 7);
        _basicTabLayout.Controls.Add(_patternLabel, 0, 8);
        _basicTabLayout.Controls.Add(_patternTextBox, 1, 8);
        _basicTabLayout.Controls.Add(_defaultValueLabel, 0, 9);
        _basicTabLayout.Controls.Add(_defaultValueTextBox, 1, 9);
        _basicTabLayout.Dock = DockStyle.Fill;
        _basicTabLayout.Location = new Point(0, 0);
        _basicTabLayout.Name = "_basicTabLayout";
        _basicTabLayout.Padding = new Padding(10);
        _basicTabLayout.RowCount = 11;
        _basicTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _basicTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _basicTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _basicTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _basicTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _basicTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _basicTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _basicTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _basicTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _basicTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _basicTabLayout.RowStyles.Add(new RowStyle());
        _basicTabLayout.Size = new Size(526, 533);
        _basicTabLayout.TabIndex = 0;
        // 
        // _nameLabel
        // 
        _nameLabel.Anchor = AnchorStyles.Left;
        _nameLabel.Location = new Point(13, 13);
        _nameLabel.Name = "_nameLabel";
        _nameLabel.Size = new Size(100, 23);
        _nameLabel.TabIndex = 0;
        _nameLabel.Text = "Name:*";
        // 
        // _nameTextBox
        // 
        _nameTextBox.Dock = DockStyle.Fill;
        _nameTextBox.Location = new Point(133, 13);
        _nameTextBox.Name = "_nameTextBox";
        _nameTextBox.Size = new Size(380, 23);
        _nameTextBox.TabIndex = 1;
        // 
        // _typeLabel
        // 
        _typeLabel.Anchor = AnchorStyles.Left;
        _typeLabel.Location = new Point(13, 43);
        _typeLabel.Name = "_typeLabel";
        _typeLabel.Size = new Size(100, 23);
        _typeLabel.TabIndex = 2;
        _typeLabel.Text = "Type:";
        // 
        // _typeComboBox
        // 
        _typeComboBox.Dock = DockStyle.Fill;
        _typeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _typeComboBox.Items.AddRange(new object[] { "string", "integer", "number", "boolean", "array", "object" });
        _typeComboBox.Location = new Point(133, 43);
        _typeComboBox.Name = "_typeComboBox";
        _typeComboBox.Size = new Size(380, 23);
        _typeComboBox.TabIndex = 3;
        // 
        // _formatLabel
        // 
        _formatLabel.Anchor = AnchorStyles.Left;
        _formatLabel.Location = new Point(13, 73);
        _formatLabel.Name = "_formatLabel";
        _formatLabel.Size = new Size(100, 23);
        _formatLabel.TabIndex = 4;
        _formatLabel.Text = "Format:";
        // 
        // _formatTextBox
        // 
        _formatTextBox.Dock = DockStyle.Fill;
        _formatTextBox.Location = new Point(133, 73);
        _formatTextBox.Name = "_formatTextBox";
        _formatTextBox.Size = new Size(380, 23);
        _formatTextBox.TabIndex = 5;
        // 
        // _descriptionLabel
        // 
        _descriptionLabel.Anchor = AnchorStyles.Left;
        _descriptionLabel.Location = new Point(13, 103);
        _descriptionLabel.Name = "_descriptionLabel";
        _descriptionLabel.Size = new Size(100, 23);
        _descriptionLabel.TabIndex = 6;
        _descriptionLabel.Text = "Description:";
        // 
        // _descriptionTextBox
        // 
        _descriptionTextBox.Dock = DockStyle.Fill;
        _descriptionTextBox.Location = new Point(133, 103);
        _descriptionTextBox.Multiline = true;
        _descriptionTextBox.Name = "_descriptionTextBox";
        _descriptionTextBox.Size = new Size(380, 24);
        _descriptionTextBox.TabIndex = 7;
        // 
        // _requiredCheckBox
        // 
        _requiredCheckBox.Anchor = AnchorStyles.Left;
        _requiredCheckBox.Location = new Point(133, 133);
        _requiredCheckBox.Name = "_requiredCheckBox";
        _requiredCheckBox.Size = new Size(104, 24);
        _requiredCheckBox.TabIndex = 8;
        _requiredCheckBox.Text = "Required";
        // 
        // _nullableCheckBox
        // 
        _nullableCheckBox.Anchor = AnchorStyles.Left;
        _nullableCheckBox.Location = new Point(133, 163);
        _nullableCheckBox.Name = "_nullableCheckBox";
        _nullableCheckBox.Size = new Size(104, 24);
        _nullableCheckBox.TabIndex = 9;
        _nullableCheckBox.Text = "Nullable";
        // 
        // _minLengthLabel
        // 
        _minLengthLabel.Anchor = AnchorStyles.Left;
        _minLengthLabel.Location = new Point(13, 193);
        _minLengthLabel.Name = "_minLengthLabel";
        _minLengthLabel.Size = new Size(100, 23);
        _minLengthLabel.TabIndex = 10;
        _minLengthLabel.Text = "Min Length:";
        // 
        // _minLengthNumeric
        // 
        _minLengthNumeric.Dock = DockStyle.Fill;
        _minLengthNumeric.Location = new Point(133, 193);
        _minLengthNumeric.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
        _minLengthNumeric.Name = "_minLengthNumeric";
        _minLengthNumeric.Size = new Size(380, 23);
        _minLengthNumeric.TabIndex = 11;
        // 
        // _maxLengthLabel
        // 
        _maxLengthLabel.Anchor = AnchorStyles.Left;
        _maxLengthLabel.Location = new Point(13, 223);
        _maxLengthLabel.Name = "_maxLengthLabel";
        _maxLengthLabel.Size = new Size(100, 23);
        _maxLengthLabel.TabIndex = 12;
        _maxLengthLabel.Text = "Max Length:";
        // 
        // _maxLengthNumeric
        // 
        _maxLengthNumeric.Dock = DockStyle.Fill;
        _maxLengthNumeric.Location = new Point(133, 223);
        _maxLengthNumeric.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
        _maxLengthNumeric.Name = "_maxLengthNumeric";
        _maxLengthNumeric.Size = new Size(380, 23);
        _maxLengthNumeric.TabIndex = 13;
        // 
        // _patternLabel
        // 
        _patternLabel.Anchor = AnchorStyles.Left;
        _patternLabel.Location = new Point(13, 253);
        _patternLabel.Name = "_patternLabel";
        _patternLabel.Size = new Size(100, 23);
        _patternLabel.TabIndex = 14;
        _patternLabel.Text = "Pattern (Regex):";
        // 
        // _patternTextBox
        // 
        _patternTextBox.Dock = DockStyle.Fill;
        _patternTextBox.Location = new Point(133, 253);
        _patternTextBox.Name = "_patternTextBox";
        _patternTextBox.Size = new Size(380, 23);
        _patternTextBox.TabIndex = 15;
        // 
        // _defaultValueLabel
        // 
        _defaultValueLabel.Anchor = AnchorStyles.Left;
        _defaultValueLabel.Location = new Point(13, 283);
        _defaultValueLabel.Name = "_defaultValueLabel";
        _defaultValueLabel.Size = new Size(100, 23);
        _defaultValueLabel.TabIndex = 16;
        _defaultValueLabel.Text = "Default Value:";
        // 
        // _defaultValueTextBox
        // 
        _defaultValueTextBox.Dock = DockStyle.Fill;
        _defaultValueTextBox.Location = new Point(133, 283);
        _defaultValueTextBox.Name = "_defaultValueTextBox";
        _defaultValueTextBox.Size = new Size(380, 23);
        _defaultValueTextBox.TabIndex = 17;
        // 
        // _dddTab
        // 
        _dddTab.Controls.Add(_dddTabLayout);
        _dddTab.Location = new Point(4, 24);
        _dddTab.Name = "_dddTab";
        _dddTab.Size = new Size(526, 533);
        _dddTab.TabIndex = 1;
        _dddTab.Text = "DDD";
        // 
        // _dddTabLayout
        // 
        _dddTabLayout.ColumnCount = 2;
        _dddTabLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
        _dddTabLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _dddTabLayout.Controls.Add(_referenceLabel, 0, 0);
        _dddTabLayout.Controls.Add(_referenceTextBox, 1, 0);
        _dddTabLayout.Controls.Add(_optionalCheckBox, 1, 1);
        _dddTabLayout.Controls.Add(_selfReferenceCheckBox, 1, 2);
        _dddTabLayout.Dock = DockStyle.Fill;
        _dddTabLayout.Location = new Point(0, 0);
        _dddTabLayout.Name = "_dddTabLayout";
        _dddTabLayout.Padding = new Padding(10);
        _dddTabLayout.RowCount = 4;
        _dddTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _dddTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _dddTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _dddTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _dddTabLayout.Size = new Size(526, 533);
        _dddTabLayout.TabIndex = 0;
        // 
        // _referenceLabel
        // 
        _referenceLabel.Anchor = AnchorStyles.Left;
        _referenceLabel.Location = new Point(13, 13);
        _referenceLabel.Name = "_referenceLabel";
        _referenceLabel.Size = new Size(100, 23);
        _referenceLabel.TabIndex = 0;
        _referenceLabel.Text = "Reference:";
        // 
        // _referenceTextBox
        // 
        _referenceTextBox.Dock = DockStyle.Fill;
        _referenceTextBox.Location = new Point(133, 13);
        _referenceTextBox.Name = "_referenceTextBox";
        _referenceTextBox.PlaceholderText = "e.g. Country";
        _referenceTextBox.Size = new Size(380, 23);
        _referenceTextBox.TabIndex = 1;
        // 
        // _optionalCheckBox
        // 
        _optionalCheckBox.Anchor = AnchorStyles.Left;
        _optionalCheckBox.Location = new Point(133, 43);
        _optionalCheckBox.Name = "_optionalCheckBox";
        _optionalCheckBox.Size = new Size(104, 24);
        _optionalCheckBox.TabIndex = 2;
        _optionalCheckBox.Text = "Optional (nullable relationship)";
        // 
        // _selfReferenceCheckBox
        // 
        _selfReferenceCheckBox.Anchor = AnchorStyles.Left;
        _selfReferenceCheckBox.Location = new Point(133, 73);
        _selfReferenceCheckBox.Name = "_selfReferenceCheckBox";
        _selfReferenceCheckBox.Size = new Size(104, 24);
        _selfReferenceCheckBox.TabIndex = 3;
        _selfReferenceCheckBox.Text = "Self Reference (hierarchical)";
        // 
        // _databaseTab
        // 
        _databaseTab.Controls.Add(_databaseTabLayout);
        _databaseTab.Location = new Point(4, 24);
        _databaseTab.Name = "_databaseTab";
        _databaseTab.Size = new Size(526, 533);
        _databaseTab.TabIndex = 2;
        _databaseTab.Text = "Database";
        // 
        // _databaseTabLayout
        // 
        _databaseTabLayout.ColumnCount = 2;
        _databaseTabLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
        _databaseTabLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _databaseTabLayout.Controls.Add(_columnNameLabel, 0, 0);
        _databaseTabLayout.Controls.Add(_columnNameTextBox, 1, 0);
        _databaseTabLayout.Controls.Add(_columnTypeLabel, 0, 1);
        _databaseTabLayout.Controls.Add(_columnTypeTextBox, 1, 1);
        _databaseTabLayout.Controls.Add(_isPrimaryKeyCheckBox, 1, 2);
        _databaseTabLayout.Controls.Add(_isIdentityCheckBox, 1, 3);
        _databaseTabLayout.Controls.Add(_isForeignKeyCheckBox, 1, 4);
        _databaseTabLayout.Dock = DockStyle.Fill;
        _databaseTabLayout.Location = new Point(0, 0);
        _databaseTabLayout.Name = "_databaseTabLayout";
        _databaseTabLayout.Padding = new Padding(10);
        _databaseTabLayout.RowCount = 6;
        _databaseTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _databaseTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _databaseTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _databaseTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _databaseTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _databaseTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _databaseTabLayout.Size = new Size(526, 533);
        _databaseTabLayout.TabIndex = 0;
        // 
        // _columnNameLabel
        // 
        _columnNameLabel.Anchor = AnchorStyles.Left;
        _columnNameLabel.Location = new Point(13, 13);
        _columnNameLabel.Name = "_columnNameLabel";
        _columnNameLabel.Size = new Size(100, 23);
        _columnNameLabel.TabIndex = 0;
        _columnNameLabel.Text = "Column Name:";
        // 
        // _columnNameTextBox
        // 
        _columnNameTextBox.Dock = DockStyle.Fill;
        _columnNameTextBox.Location = new Point(133, 13);
        _columnNameTextBox.Name = "_columnNameTextBox";
        _columnNameTextBox.Size = new Size(380, 23);
        _columnNameTextBox.TabIndex = 1;
        // 
        // _columnTypeLabel
        // 
        _columnTypeLabel.Anchor = AnchorStyles.Left;
        _columnTypeLabel.Location = new Point(13, 43);
        _columnTypeLabel.Name = "_columnTypeLabel";
        _columnTypeLabel.Size = new Size(100, 23);
        _columnTypeLabel.TabIndex = 2;
        _columnTypeLabel.Text = "Column Type:";
        // 
        // _columnTypeTextBox
        // 
        _columnTypeTextBox.Dock = DockStyle.Fill;
        _columnTypeTextBox.Location = new Point(133, 43);
        _columnTypeTextBox.Name = "_columnTypeTextBox";
        _columnTypeTextBox.Size = new Size(380, 23);
        _columnTypeTextBox.TabIndex = 3;
        // 
        // _isPrimaryKeyCheckBox
        // 
        _isPrimaryKeyCheckBox.Anchor = AnchorStyles.Left;
        _isPrimaryKeyCheckBox.Location = new Point(133, 73);
        _isPrimaryKeyCheckBox.Name = "_isPrimaryKeyCheckBox";
        _isPrimaryKeyCheckBox.Size = new Size(104, 24);
        _isPrimaryKeyCheckBox.TabIndex = 4;
        _isPrimaryKeyCheckBox.Text = "Is Primary Key";
        // 
        // _isIdentityCheckBox
        // 
        _isIdentityCheckBox.Anchor = AnchorStyles.Left;
        _isIdentityCheckBox.Location = new Point(133, 103);
        _isIdentityCheckBox.Name = "_isIdentityCheckBox";
        _isIdentityCheckBox.Size = new Size(104, 24);
        _isIdentityCheckBox.TabIndex = 5;
        _isIdentityCheckBox.Text = "Is Identity (Auto-increment)";
        // 
        // _isForeignKeyCheckBox
        // 
        _isForeignKeyCheckBox.Anchor = AnchorStyles.Left;
        _isForeignKeyCheckBox.Location = new Point(133, 133);
        _isForeignKeyCheckBox.Name = "_isForeignKeyCheckBox";
        _isForeignKeyCheckBox.Size = new Size(104, 24);
        _isForeignKeyCheckBox.TabIndex = 6;
        _isForeignKeyCheckBox.Text = "Is Foreign Key";
        // 
        // _displayTab
        // 
        _displayTab.Controls.Add(_displayTabLayout);
        _displayTab.Location = new Point(4, 24);
        _displayTab.Name = "_displayTab";
        _displayTab.Size = new Size(526, 533);
        _displayTab.TabIndex = 3;
        _displayTab.Text = "Display";
        // 
        // _displayTabLayout
        // 
        _displayTabLayout.ColumnCount = 2;
        _displayTabLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
        _displayTabLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _displayTabLayout.Controls.Add(_displayLabelLabel, 0, 0);
        _displayTabLayout.Controls.Add(_labelTextBox, 1, 0);
        _displayTabLayout.Controls.Add(_controlTypeLabel, 0, 1);
        _displayTabLayout.Controls.Add(_controlTypeComboBox, 1, 1);
        _displayTabLayout.Dock = DockStyle.Fill;
        _displayTabLayout.Location = new Point(0, 0);
        _displayTabLayout.Name = "_displayTabLayout";
        _displayTabLayout.Padding = new Padding(10);
        _displayTabLayout.RowCount = 4;
        _displayTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _displayTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _displayTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _displayTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _displayTabLayout.Size = new Size(526, 533);
        _displayTabLayout.TabIndex = 0;
        // 
        // _displayLabelLabel
        // 
        _displayLabelLabel.Anchor = AnchorStyles.Left;
        _displayLabelLabel.Location = new Point(13, 13);
        _displayLabelLabel.Name = "_displayLabelLabel";
        _displayLabelLabel.Size = new Size(100, 23);
        _displayLabelLabel.TabIndex = 0;
        _displayLabelLabel.Text = "Display Label:";
        // 
        // _labelTextBox
        // 
        _labelTextBox.Dock = DockStyle.Fill;
        _labelTextBox.Location = new Point(133, 13);
        _labelTextBox.Name = "_labelTextBox";
        _labelTextBox.Size = new Size(380, 23);
        _labelTextBox.TabIndex = 1;
        // 
        // _controlTypeLabel
        // 
        _controlTypeLabel.Anchor = AnchorStyles.Left;
        _controlTypeLabel.Location = new Point(13, 43);
        _controlTypeLabel.Name = "_controlTypeLabel";
        _controlTypeLabel.Size = new Size(100, 23);
        _controlTypeLabel.TabIndex = 2;
        _controlTypeLabel.Text = "Control Type:";
        // 
        // _controlTypeComboBox
        // 
        _controlTypeComboBox.Dock = DockStyle.Fill;
        _controlTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _controlTypeComboBox.Items.AddRange(new object[] { "(Auto)", "TextBox", "TextArea", "NumericUpDown", "CheckBox", "ComboBox", "DatePicker", "DateTimePicker", "TimePicker" });
        _controlTypeComboBox.Location = new Point(133, 43);
        _controlTypeComboBox.Name = "_controlTypeComboBox";
        _controlTypeComboBox.Size = new Size(380, 23);
        _controlTypeComboBox.TabIndex = 3;
        // 
        // _buttonPanel
        // 
        _buttonPanel.Controls.Add(_okButton);
        _buttonPanel.Controls.Add(_cancelButton);
        _buttonPanel.Dock = DockStyle.Bottom;
        _buttonPanel.Location = new Point(0, 561);
        _buttonPanel.Name = "_buttonPanel";
        _buttonPanel.Size = new Size(534, 50);
        _buttonPanel.TabIndex = 1;
        // 
        // _okButton
        // 
        _okButton.DialogResult = DialogResult.OK;
        _okButton.Location = new Point(300, 12);
        _okButton.Name = "_okButton";
        _okButton.Size = new Size(80, 28);
        _okButton.TabIndex = 0;
        _okButton.Text = "OK";
        _okButton.Click += OkButton_Click;
        // 
        // _cancelButton
        // 
        _cancelButton.DialogResult = DialogResult.Cancel;
        _cancelButton.Location = new Point(300, 12);
        _cancelButton.Name = "_cancelButton";
        _cancelButton.Size = new Size(80, 28);
        _cancelButton.TabIndex = 1;
        _cancelButton.Text = "Cancel";
        // 
        // PropertyEditorForm
        // 
        AcceptButton = _okButton;
        CancelButton = _cancelButton;
        ClientSize = new Size(534, 611);
        Controls.Add(_tabControl);
        Controls.Add(_buttonPanel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "PropertyEditorForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Add or Edit property";
        _tabControl.ResumeLayout(false);
        _basicTab.ResumeLayout(false);
        _basicTabLayout.ResumeLayout(false);
        _basicTabLayout.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)_minLengthNumeric).EndInit();
        ((System.ComponentModel.ISupportInitialize)_maxLengthNumeric).EndInit();
        _dddTab.ResumeLayout(false);
        _dddTabLayout.ResumeLayout(false);
        _dddTabLayout.PerformLayout();
        _databaseTab.ResumeLayout(false);
        _databaseTabLayout.ResumeLayout(false);
        _databaseTabLayout.PerformLayout();
        _displayTab.ResumeLayout(false);
        _displayTabLayout.ResumeLayout(false);
        _displayTabLayout.PerformLayout();
        _buttonPanel.ResumeLayout(false);
        ResumeLayout(false);
    }

}
