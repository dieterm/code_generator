namespace CodeGenerator.WinForms;

partial class EntityEditorForm
{
    private System.ComponentModel.IContainer components = null;

    private TableLayoutPanel layout;
    private Panel buttonPanel;
    private TextBox _nameTextBox;
    private TextBox _titleTextBox;
    private TextBox _descriptionTextBox;
    private TextBox _tableNameTextBox;
    private TextBox _schemaTextBox;
    private TextBox _baseClassTextBox;
    private CheckBox _isAbstractCheckBox;
    private CheckBox _isSealedCheckBox;
    private CheckBox _isOwnedTypeCheckBox;
    private CheckBox _generateRepoCheckBox;
    private CheckBox _generateControllerCheckBox;
    private CheckBox _generateViewModelCheckBox;
    private CheckBox _generateViewCheckBox;
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

        Text = string.IsNullOrEmpty(EntityName) ? "Add Entity" : $"Edit Entity - {EntityName}";
        Size = new Size(500, 550);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(10),
            ColumnCount = 2,
            RowCount = 16
        };

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        int row = 0;

        // Entity Name
        layout.Controls.Add(new Label { Text = "Entity Name:*", Anchor = AnchorStyles.Left }, 0, row);
        _nameTextBox = new TextBox { Dock = DockStyle.Fill };
        layout.Controls.Add(_nameTextBox, 1, row++);

        // Title
        layout.Controls.Add(new Label { Text = "Title:", Anchor = AnchorStyles.Left }, 0, row);
        _titleTextBox = new TextBox { Dock = DockStyle.Fill };
        layout.Controls.Add(_titleTextBox, 1, row++);

        // Description
        layout.Controls.Add(new Label { Text = "Description:", Anchor = AnchorStyles.Left }, 0, row);
        _descriptionTextBox = new TextBox { Dock = DockStyle.Fill, Multiline = true, Height = 60 };
        layout.Controls.Add(_descriptionTextBox, 1, row++);

        // Database Settings Header
        var dbLabel = new Label { Text = "Database Settings", Font = new Font(Font, FontStyle.Bold), Anchor = AnchorStyles.Left };
        layout.Controls.Add(dbLabel, 0, row++);
        layout.SetColumnSpan(dbLabel, 2);

        // Table Name
        layout.Controls.Add(new Label { Text = "Table Name:", Anchor = AnchorStyles.Left }, 0, row);
        _tableNameTextBox = new TextBox { Dock = DockStyle.Fill };
        layout.Controls.Add(_tableNameTextBox, 1, row++);

        // Schema
        layout.Controls.Add(new Label { Text = "Schema:", Anchor = AnchorStyles.Left }, 0, row);
        _schemaTextBox = new TextBox { Dock = DockStyle.Fill, Text = "dbo" };
        layout.Controls.Add(_schemaTextBox, 1, row++);

        // Code Generation Settings Header
        var codeGenLabel = new Label { Text = "Code Generation Settings", Font = new Font(Font, FontStyle.Bold), Anchor = AnchorStyles.Left };
        layout.Controls.Add(codeGenLabel, 0, row++);
        layout.SetColumnSpan(codeGenLabel, 2);

        // Base Class
        layout.Controls.Add(new Label { Text = "Base Class:", Anchor = AnchorStyles.Left }, 0, row);
        _baseClassTextBox = new TextBox { Dock = DockStyle.Fill };
        layout.Controls.Add(_baseClassTextBox, 1, row++);

        // Checkboxes
        _isAbstractCheckBox = new CheckBox { Text = "Is Abstract", Anchor = AnchorStyles.Left };
        layout.Controls.Add(_isAbstractCheckBox, 1, row++);

        _isSealedCheckBox = new CheckBox { Text = "Is Sealed", Anchor = AnchorStyles.Left };
        layout.Controls.Add(_isSealedCheckBox, 1, row++);

        _isOwnedTypeCheckBox = new CheckBox { Text = "Is Owned Type (Value Object)", Anchor = AnchorStyles.Left };
        layout.Controls.Add(_isOwnedTypeCheckBox, 1, row++);

        // Generate Options Header
        var generateLabel = new Label { Text = "Generate:", Font = new Font(Font, FontStyle.Bold), Anchor = AnchorStyles.Left };
        layout.Controls.Add(generateLabel, 0, row++);

        _generateRepoCheckBox = new CheckBox { Text = "Repository", Anchor = AnchorStyles.Left, Checked = true };
        layout.Controls.Add(_generateRepoCheckBox, 1, row++);

        _generateControllerCheckBox = new CheckBox { Text = "Controller", Anchor = AnchorStyles.Left, Checked = true };
        layout.Controls.Add(_generateControllerCheckBox, 1, row++);

        _generateViewModelCheckBox = new CheckBox { Text = "ViewModel", Anchor = AnchorStyles.Left, Checked = true };
        layout.Controls.Add(_generateViewModelCheckBox, 1, row++);

        _generateViewCheckBox = new CheckBox { Text = "View", Anchor = AnchorStyles.Left, Checked = true };
        layout.Controls.Add(_generateViewCheckBox, 1, row++);

        // Button Panel
        buttonPanel = new Panel
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

        buttonPanel.Controls.AddRange(new Control[] { _okButton, _cancelButton });

        Controls.Add(layout);
        Controls.Add(buttonPanel);

        AcceptButton = _okButton;
        CancelButton = _cancelButton;
    }
}
