namespace CodeGenerator.WinForms.Controls;

partial class DatabaseMetadataEditView
{
    private System.ComponentModel.IContainer components = null;

    private TableLayoutPanel _layout;
    private Label _databaseNameLabel;
    private TextBox _databaseNameTextBox;
    private Label _schemaLabel;
    private TextBox _schemaTextBox;
    private Label _providerLabel;
    private ComboBox _providerComboBox;
    private Label _connectionStringNameLabel;
    private TextBox _connectionStringNameTextBox;
    private CheckBox _useMigrationsCheckBox;
    private CheckBox _seedDataCheckBox;

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
        _databaseNameLabel = new Label();
        _databaseNameTextBox = new TextBox();
        _schemaLabel = new Label();
        _schemaTextBox = new TextBox();
        _providerLabel = new Label();
        _providerComboBox = new ComboBox();
        _connectionStringNameLabel = new Label();
        _connectionStringNameTextBox = new TextBox();
        _useMigrationsCheckBox = new CheckBox();
        _seedDataCheckBox = new CheckBox();
        _layout.SuspendLayout();
        SuspendLayout();
        // 
        // _layout
        // 
        _layout.ColumnCount = 2;
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _layout.Controls.Add(_databaseNameLabel, 0, 0);
        _layout.Controls.Add(_databaseNameTextBox, 1, 0);
        _layout.Controls.Add(_schemaLabel, 0, 1);
        _layout.Controls.Add(_schemaTextBox, 1, 1);
        _layout.Controls.Add(_providerLabel, 0, 2);
        _layout.Controls.Add(_providerComboBox, 1, 2);
        _layout.Controls.Add(_connectionStringNameLabel, 0, 3);
        _layout.Controls.Add(_connectionStringNameTextBox, 1, 3);
        _layout.Controls.Add(_useMigrationsCheckBox, 1, 4);
        _layout.Controls.Add(_seedDataCheckBox, 1, 5);
        _layout.Dock = DockStyle.Fill;
        _layout.Location = new Point(0, 0);
        _layout.Name = "_layout";
        _layout.Padding = new Padding(10);
        _layout.RowCount = 7;
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _layout.RowStyles.Add(new RowStyle());
        _layout.Size = new Size(486, 237);
        _layout.TabIndex = 0;
        // 
        // _databaseNameLabel
        // 
        _databaseNameLabel.Anchor = AnchorStyles.Left;
        _databaseNameLabel.Location = new Point(13, 13);
        _databaseNameLabel.Name = "_databaseNameLabel";
        _databaseNameLabel.Size = new Size(100, 23);
        _databaseNameLabel.TabIndex = 0;
        _databaseNameLabel.Text = "Database Name:";
        // 
        // _databaseNameTextBox
        // 
        _databaseNameTextBox.Dock = DockStyle.Fill;
        _databaseNameTextBox.Location = new Point(193, 13);
        _databaseNameTextBox.Name = "_databaseNameTextBox";
        _databaseNameTextBox.Size = new Size(280, 23);
        _databaseNameTextBox.TabIndex = 1;
        // 
        // _schemaLabel
        // 
        _schemaLabel.Anchor = AnchorStyles.Left;
        _schemaLabel.Location = new Point(13, 43);
        _schemaLabel.Name = "_schemaLabel";
        _schemaLabel.Size = new Size(100, 23);
        _schemaLabel.TabIndex = 2;
        _schemaLabel.Text = "Schema:";
        // 
        // _schemaTextBox
        // 
        _schemaTextBox.Dock = DockStyle.Fill;
        _schemaTextBox.Location = new Point(193, 43);
        _schemaTextBox.Name = "_schemaTextBox";
        _schemaTextBox.Size = new Size(280, 23);
        _schemaTextBox.TabIndex = 3;
        _schemaTextBox.Text = "dbo";
        // 
        // _providerLabel
        // 
        _providerLabel.Anchor = AnchorStyles.Left;
        _providerLabel.Location = new Point(13, 73);
        _providerLabel.Name = "_providerLabel";
        _providerLabel.Size = new Size(100, 23);
        _providerLabel.TabIndex = 4;
        _providerLabel.Text = "Provider:";
        // 
        // _providerComboBox
        // 
        _providerComboBox.Dock = DockStyle.Fill;
        _providerComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _providerComboBox.Items.AddRange(new object[] { "SqlServer", "PostgreSQL", "MySQL", "SQLite", "Oracle" });
        _providerComboBox.Location = new Point(193, 73);
        _providerComboBox.Name = "_providerComboBox";
        _providerComboBox.Size = new Size(280, 23);
        _providerComboBox.TabIndex = 5;
        // 
        // _connectionStringNameLabel
        // 
        _connectionStringNameLabel.Anchor = AnchorStyles.Left;
        _connectionStringNameLabel.Location = new Point(13, 103);
        _connectionStringNameLabel.Name = "_connectionStringNameLabel";
        _connectionStringNameLabel.Size = new Size(100, 23);
        _connectionStringNameLabel.TabIndex = 6;
        _connectionStringNameLabel.Text = "Connection String Name:";
        // 
        // _connectionStringNameTextBox
        // 
        _connectionStringNameTextBox.Dock = DockStyle.Fill;
        _connectionStringNameTextBox.Location = new Point(193, 103);
        _connectionStringNameTextBox.Name = "_connectionStringNameTextBox";
        _connectionStringNameTextBox.Size = new Size(280, 23);
        _connectionStringNameTextBox.TabIndex = 7;
        // 
        // _useMigrationsCheckBox
        // 
        _useMigrationsCheckBox.Anchor = AnchorStyles.Left;
        _useMigrationsCheckBox.Checked = true;
        _useMigrationsCheckBox.CheckState = CheckState.Checked;
        _useMigrationsCheckBox.Location = new Point(193, 133);
        _useMigrationsCheckBox.Name = "_useMigrationsCheckBox";
        _useMigrationsCheckBox.Size = new Size(104, 24);
        _useMigrationsCheckBox.TabIndex = 8;
        _useMigrationsCheckBox.Text = "Use Migrations";
        // 
        // _seedDataCheckBox
        // 
        _seedDataCheckBox.Anchor = AnchorStyles.Left;
        _seedDataCheckBox.Location = new Point(193, 163);
        _seedDataCheckBox.Name = "_seedDataCheckBox";
        _seedDataCheckBox.Size = new Size(104, 24);
        _seedDataCheckBox.TabIndex = 9;
        _seedDataCheckBox.Text = "Seed Data";
        // 
        // DatabaseMetadataEditView
        // 
        Controls.Add(_layout);
        Name = "DatabaseMetadataEditView";
        Size = new Size(486, 237);
        _layout.ResumeLayout(false);
        _layout.PerformLayout();
        ResumeLayout(false);
    }
}
