namespace CodeGenerator.WinForms.Controls;

partial class PropertyDefinitionEditView
{
    private System.ComponentModel.IContainer components = null;

    private TableLayoutPanel _layout;
    private Label _typeLabel;
    private ComboBox _typeComboBox;
    private Label _formatLabel;
    private TextBox _formatTextBox;
    private Label _descriptionLabel;
    private TextBox _descriptionTextBox;
    private CheckBox _isNullableCheckBox;
    private Label _minLengthLabel;
    private NumericUpDown _minLengthNumeric;
    private Label _maxLengthLabel;
    private NumericUpDown _maxLengthNumeric;

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
        _layout = new TableLayoutPanel();
        _refComboBox = new ComboBox();
        _refLabel = new Label();
        _typeLabel = new Label();
        _typeComboBox = new ComboBox();
        _formatLabel = new Label();
        _formatTextBox = new TextBox();
        _descriptionLabel = new Label();
        _descriptionTextBox = new TextBox();
        _isNullableCheckBox = new CheckBox();
        _minLengthLabel = new Label();
        _minLengthNumeric = new NumericUpDown();
        _maxLengthLabel = new Label();
        _maxLengthNumeric = new NumericUpDown();
        _layout.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_minLengthNumeric).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_maxLengthNumeric).BeginInit();
        SuspendLayout();
        // 
        // _layout
        // 
        _layout.ColumnCount = 2;
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _layout.Controls.Add(_refComboBox, 1, 1);
        _layout.Controls.Add(_refLabel, 0, 1);
        _layout.Controls.Add(_typeLabel, 0, 0);
        _layout.Controls.Add(_typeComboBox, 1, 0);
        _layout.Controls.Add(_formatLabel, 0, 2);
        _layout.Controls.Add(_formatTextBox, 1, 2);
        _layout.Controls.Add(_descriptionLabel, 0, 3);
        _layout.Controls.Add(_descriptionTextBox, 1, 3);
        _layout.Controls.Add(_isNullableCheckBox, 1, 4);
        _layout.Controls.Add(_minLengthLabel, 0, 5);
        _layout.Controls.Add(_minLengthNumeric, 1, 5);
        _layout.Controls.Add(_maxLengthLabel, 0, 6);
        _layout.Controls.Add(_maxLengthNumeric, 1, 6);
        _layout.Dock = DockStyle.Fill;
        _layout.Location = new Point(0, 0);
        _layout.Name = "_layout";
        _layout.Padding = new Padding(10);
        _layout.RowCount = 9;
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _layout.RowStyles.Add(new RowStyle());
        _layout.Size = new Size(484, 263);
        _layout.TabIndex = 0;
        // 
        // _refComboBox
        // 
        _refComboBox.Dock = DockStyle.Fill;
        _refComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _refComboBox.Items.AddRange(new object[] { "string", "integer", "number", "boolean", "array", "object" });
        _refComboBox.Location = new Point(133, 43);
        _refComboBox.Name = "_refComboBox";
        _refComboBox.Size = new Size(338, 23);
        _refComboBox.TabIndex = 12;
        // 
        // _refLabel
        // 
        _refLabel.Anchor = AnchorStyles.Left;
        _refLabel.Location = new Point(13, 43);
        _refLabel.Name = "_refLabel";
        _refLabel.Size = new Size(100, 23);
        _refLabel.TabIndex = 11;
        _refLabel.Text = "Ref:";
        // 
        // _typeLabel
        // 
        _typeLabel.Anchor = AnchorStyles.Left;
        _typeLabel.Location = new Point(13, 13);
        _typeLabel.Name = "_typeLabel";
        _typeLabel.Size = new Size(100, 23);
        _typeLabel.TabIndex = 0;
        _typeLabel.Text = "Type:";
        // 
        // _typeComboBox
        // 
        _typeComboBox.Dock = DockStyle.Fill;
        _typeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _typeComboBox.Items.AddRange(new object[] { "string", "integer", "number", "boolean", "array", "object", "ref" });
        _typeComboBox.Location = new Point(133, 13);
        _typeComboBox.Name = "_typeComboBox";
        _typeComboBox.Size = new Size(338, 23);
        _typeComboBox.TabIndex = 1;
        // 
        // _formatLabel
        // 
        _formatLabel.Anchor = AnchorStyles.Left;
        _formatLabel.Location = new Point(13, 73);
        _formatLabel.Name = "_formatLabel";
        _formatLabel.Size = new Size(100, 23);
        _formatLabel.TabIndex = 2;
        _formatLabel.Text = "Format:";
        // 
        // _formatTextBox
        // 
        _formatTextBox.Dock = DockStyle.Fill;
        _formatTextBox.Location = new Point(133, 73);
        _formatTextBox.Name = "_formatTextBox";
        _formatTextBox.Size = new Size(338, 23);
        _formatTextBox.TabIndex = 3;
        // 
        // _descriptionLabel
        // 
        _descriptionLabel.Location = new Point(13, 100);
        _descriptionLabel.Name = "_descriptionLabel";
        _descriptionLabel.Size = new Size(100, 23);
        _descriptionLabel.TabIndex = 4;
        _descriptionLabel.Text = "Description:";
        // 
        // _descriptionTextBox
        // 
        _descriptionTextBox.Dock = DockStyle.Fill;
        _descriptionTextBox.Location = new Point(133, 103);
        _descriptionTextBox.Multiline = true;
        _descriptionTextBox.Name = "_descriptionTextBox";
        _descriptionTextBox.Size = new Size(338, 24);
        _descriptionTextBox.TabIndex = 5;
        // 
        // _isNullableCheckBox
        // 
        _isNullableCheckBox.Anchor = AnchorStyles.Left;
        _isNullableCheckBox.Location = new Point(133, 133);
        _isNullableCheckBox.Name = "_isNullableCheckBox";
        _isNullableCheckBox.Size = new Size(104, 24);
        _isNullableCheckBox.TabIndex = 6;
        _isNullableCheckBox.Text = "Nullable";
        // 
        // _minLengthLabel
        // 
        _minLengthLabel.Anchor = AnchorStyles.Left;
        _minLengthLabel.Location = new Point(13, 163);
        _minLengthLabel.Name = "_minLengthLabel";
        _minLengthLabel.Size = new Size(100, 23);
        _minLengthLabel.TabIndex = 7;
        _minLengthLabel.Text = "Min Length:";
        // 
        // _minLengthNumeric
        // 
        _minLengthNumeric.Dock = DockStyle.Fill;
        _minLengthNumeric.Location = new Point(133, 163);
        _minLengthNumeric.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
        _minLengthNumeric.Name = "_minLengthNumeric";
        _minLengthNumeric.Size = new Size(338, 23);
        _minLengthNumeric.TabIndex = 8;
        // 
        // _maxLengthLabel
        // 
        _maxLengthLabel.Anchor = AnchorStyles.Left;
        _maxLengthLabel.Location = new Point(13, 193);
        _maxLengthLabel.Name = "_maxLengthLabel";
        _maxLengthLabel.Size = new Size(100, 23);
        _maxLengthLabel.TabIndex = 9;
        _maxLengthLabel.Text = "Max Length:";
        // 
        // _maxLengthNumeric
        // 
        _maxLengthNumeric.Dock = DockStyle.Fill;
        _maxLengthNumeric.Location = new Point(133, 193);
        _maxLengthNumeric.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
        _maxLengthNumeric.Name = "_maxLengthNumeric";
        _maxLengthNumeric.Size = new Size(338, 23);
        _maxLengthNumeric.TabIndex = 10;
        // 
        // PropertyDefinitionEditView
        // 
        Controls.Add(_layout);
        Name = "PropertyDefinitionEditView";
        Size = new Size(484, 263);
        _layout.ResumeLayout(false);
        _layout.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)_minLengthNumeric).EndInit();
        ((System.ComponentModel.ISupportInitialize)_maxLengthNumeric).EndInit();
        ResumeLayout(false);
    }

    private ComboBox _refComboBox;
    private Label _refLabel;
}
