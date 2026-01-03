namespace CodeGenerator.WinForms.Controls;

partial class CodeGenMetadataEditView
{
    private System.ComponentModel.IContainer components = null;

    private TableLayoutPanel _layout;
    private Label _namespaceLabel;
    private TextBox _namespaceTextBox;
    private Label _outputPathLabel;
    private TextBox _outputPathTextBox;
    private Label _targetLanguageLabel;
    private ComboBox _targetLanguageComboBox;
    private Label _presentationTechnologyLabel;
    private ComboBox _presentationTechnologyComboBox;
    private Label _dataLayerTechnologyLabel;
    private ComboBox _dataLayerTechnologyComboBox;
    private CheckBox _useDependencyInjectionCheckBox;
    private CheckBox _useLoggingCheckBox;
    private CheckBox _useConfigurationCheckBox;

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
        _namespaceLabel = new Label();
        _namespaceTextBox = new TextBox();
        _outputPathLabel = new Label();
        _outputPathTextBox = new TextBox();
        _targetLanguageLabel = new Label();
        _targetLanguageComboBox = new ComboBox();
        _presentationTechnologyLabel = new Label();
        _presentationTechnologyComboBox = new ComboBox();
        _dataLayerTechnologyLabel = new Label();
        _dataLayerTechnologyComboBox = new ComboBox();
        _useDependencyInjectionCheckBox = new CheckBox();
        _useLoggingCheckBox = new CheckBox();
        _useConfigurationCheckBox = new CheckBox();
        _layout.SuspendLayout();
        SuspendLayout();
        // 
        // _layout
        // 
        _layout.ColumnCount = 2;
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _layout.Controls.Add(_namespaceLabel, 0, 0);
        _layout.Controls.Add(_namespaceTextBox, 1, 0);
        _layout.Controls.Add(_outputPathLabel, 0, 1);
        _layout.Controls.Add(_outputPathTextBox, 1, 1);
        _layout.Controls.Add(_targetLanguageLabel, 0, 2);
        _layout.Controls.Add(_targetLanguageComboBox, 1, 2);
        _layout.Controls.Add(_presentationTechnologyLabel, 0, 3);
        _layout.Controls.Add(_presentationTechnologyComboBox, 1, 3);
        _layout.Controls.Add(_dataLayerTechnologyLabel, 0, 4);
        _layout.Controls.Add(_dataLayerTechnologyComboBox, 1, 4);
        _layout.Controls.Add(_useDependencyInjectionCheckBox, 1, 5);
        _layout.Controls.Add(_useLoggingCheckBox, 1, 6);
        _layout.Controls.Add(_useConfigurationCheckBox, 1, 7);
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
        _layout.Size = new Size(574, 282);
        _layout.TabIndex = 0;
        // 
        // _namespaceLabel
        // 
        _namespaceLabel.Anchor = AnchorStyles.Left;
        _namespaceLabel.Location = new Point(13, 13);
        _namespaceLabel.Name = "_namespaceLabel";
        _namespaceLabel.Size = new Size(100, 23);
        _namespaceLabel.TabIndex = 0;
        _namespaceLabel.Text = "Namespace:*";
        // 
        // _namespaceTextBox
        // 
        _namespaceTextBox.Dock = DockStyle.Fill;
        _namespaceTextBox.Location = new Point(193, 13);
        _namespaceTextBox.Name = "_namespaceTextBox";
        _namespaceTextBox.Size = new Size(368, 23);
        _namespaceTextBox.TabIndex = 1;
        // 
        // _outputPathLabel
        // 
        _outputPathLabel.Anchor = AnchorStyles.Left;
        _outputPathLabel.Location = new Point(13, 43);
        _outputPathLabel.Name = "_outputPathLabel";
        _outputPathLabel.Size = new Size(100, 23);
        _outputPathLabel.TabIndex = 2;
        _outputPathLabel.Text = "Output Path:";
        // 
        // _outputPathTextBox
        // 
        _outputPathTextBox.Dock = DockStyle.Fill;
        _outputPathTextBox.Location = new Point(193, 43);
        _outputPathTextBox.Name = "_outputPathTextBox";
        _outputPathTextBox.Size = new Size(368, 23);
        _outputPathTextBox.TabIndex = 3;
        // 
        // _targetLanguageLabel
        // 
        _targetLanguageLabel.Anchor = AnchorStyles.Left;
        _targetLanguageLabel.Location = new Point(13, 73);
        _targetLanguageLabel.Name = "_targetLanguageLabel";
        _targetLanguageLabel.Size = new Size(100, 23);
        _targetLanguageLabel.TabIndex = 4;
        _targetLanguageLabel.Text = "Target Language:";
        // 
        // _targetLanguageComboBox
        // 
        _targetLanguageComboBox.Dock = DockStyle.Fill;
        _targetLanguageComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _targetLanguageComboBox.Items.AddRange(new object[] { "CSharp", "TypeScript", "JavaScript" });
        _targetLanguageComboBox.Location = new Point(193, 73);
        _targetLanguageComboBox.Name = "_targetLanguageComboBox";
        _targetLanguageComboBox.Size = new Size(368, 23);
        _targetLanguageComboBox.TabIndex = 5;
        // 
        // _presentationTechnologyLabel
        // 
        _presentationTechnologyLabel.Anchor = AnchorStyles.Left;
        _presentationTechnologyLabel.Location = new Point(13, 103);
        _presentationTechnologyLabel.Name = "_presentationTechnologyLabel";
        _presentationTechnologyLabel.Size = new Size(100, 23);
        _presentationTechnologyLabel.TabIndex = 6;
        _presentationTechnologyLabel.Text = "Presentation Technology:";
        // 
        // _presentationTechnologyComboBox
        // 
        _presentationTechnologyComboBox.Dock = DockStyle.Fill;
        _presentationTechnologyComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _presentationTechnologyComboBox.Items.AddRange(new object[] { "(None)", "WinForms", "WPF", "WinUI", "Blazor", "React", "Angular" });
        _presentationTechnologyComboBox.Location = new Point(193, 103);
        _presentationTechnologyComboBox.Name = "_presentationTechnologyComboBox";
        _presentationTechnologyComboBox.Size = new Size(368, 23);
        _presentationTechnologyComboBox.TabIndex = 7;
        // 
        // _dataLayerTechnologyLabel
        // 
        _dataLayerTechnologyLabel.Anchor = AnchorStyles.Left;
        _dataLayerTechnologyLabel.Location = new Point(13, 133);
        _dataLayerTechnologyLabel.Name = "_dataLayerTechnologyLabel";
        _dataLayerTechnologyLabel.Size = new Size(100, 23);
        _dataLayerTechnologyLabel.TabIndex = 8;
        _dataLayerTechnologyLabel.Text = "Data Layer Technology:";
        // 
        // _dataLayerTechnologyComboBox
        // 
        _dataLayerTechnologyComboBox.Dock = DockStyle.Fill;
        _dataLayerTechnologyComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _dataLayerTechnologyComboBox.Items.AddRange(new object[] { "EntityFrameworkCore", "Dapper", "ADO.NET" });
        _dataLayerTechnologyComboBox.Location = new Point(193, 133);
        _dataLayerTechnologyComboBox.Name = "_dataLayerTechnologyComboBox";
        _dataLayerTechnologyComboBox.Size = new Size(368, 23);
        _dataLayerTechnologyComboBox.TabIndex = 9;
        // 
        // _useDependencyInjectionCheckBox
        // 
        _useDependencyInjectionCheckBox.Anchor = AnchorStyles.Left;
        _useDependencyInjectionCheckBox.Checked = true;
        _useDependencyInjectionCheckBox.CheckState = CheckState.Checked;
        _useDependencyInjectionCheckBox.Location = new Point(193, 163);
        _useDependencyInjectionCheckBox.Name = "_useDependencyInjectionCheckBox";
        _useDependencyInjectionCheckBox.Size = new Size(104, 24);
        _useDependencyInjectionCheckBox.TabIndex = 10;
        _useDependencyInjectionCheckBox.Text = "Use Dependency Injection";
        // 
        // _useLoggingCheckBox
        // 
        _useLoggingCheckBox.Anchor = AnchorStyles.Left;
        _useLoggingCheckBox.Checked = true;
        _useLoggingCheckBox.CheckState = CheckState.Checked;
        _useLoggingCheckBox.Location = new Point(193, 193);
        _useLoggingCheckBox.Name = "_useLoggingCheckBox";
        _useLoggingCheckBox.Size = new Size(104, 24);
        _useLoggingCheckBox.TabIndex = 11;
        _useLoggingCheckBox.Text = "Use Logging";
        // 
        // _useConfigurationCheckBox
        // 
        _useConfigurationCheckBox.Anchor = AnchorStyles.Left;
        _useConfigurationCheckBox.Checked = true;
        _useConfigurationCheckBox.CheckState = CheckState.Checked;
        _useConfigurationCheckBox.Location = new Point(193, 223);
        _useConfigurationCheckBox.Name = "_useConfigurationCheckBox";
        _useConfigurationCheckBox.Size = new Size(104, 24);
        _useConfigurationCheckBox.TabIndex = 12;
        _useConfigurationCheckBox.Text = "Use Configuration";
        // 
        // CodeGenMetadataEditView
        // 
        Controls.Add(_layout);
        Name = "CodeGenMetadataEditView";
        Size = new Size(574, 282);
        _layout.ResumeLayout(false);
        _layout.PerformLayout();
        ResumeLayout(false);
    }
}
