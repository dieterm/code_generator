namespace CodeGenerator.WinForms;

partial class EntityEditorForm
{
    private System.ComponentModel.IContainer components = null;

    private TableLayoutPanel _layout;
    private Panel _buttonPanel;
    
    // Labels
    private Label _entityNameLabel;
    private Label _titleLabel;
    private Label _descriptionLabel;
    private Label _dddHeaderLabel;
    private Label _databaseHeaderLabel;
    private Label _tableNameLabel;
    private Label _schemaLabel;
    private Label _codeGenHeaderLabel;
    private Label _baseClassLabel;
    private Label _generateHeaderLabel;
    
    // TextBoxes
    private TextBox _nameTextBox;
    private TextBox _titleTextBox;
    private TextBox _descriptionTextBox;
    private TextBox _tableNameTextBox;
    private TextBox _schemaTextBox;
    private TextBox _baseClassTextBox;
    
    // CheckBoxes - DDD
    private CheckBox _isValueObjectCheckBox;
    private CheckBox _isAggregateRootCheckBox;
    private CheckBox _isHierarchicalCheckBox;
    
    // CheckBoxes - Code Generation
    private CheckBox _isAbstractCheckBox;
    private CheckBox _isSealedCheckBox;
    private CheckBox _isOwnedTypeCheckBox;
    
    // CheckBoxes - Generate Options
    private CheckBox _generateRepoCheckBox;
    private CheckBox _generateControllerCheckBox;
    private CheckBox _generateViewModelCheckBox;
    private CheckBox _generateViewCheckBox;
    
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
        _layout = new TableLayoutPanel();
        _entityNameLabel = new Label();
        _nameTextBox = new TextBox();
        _titleLabel = new Label();
        _titleTextBox = new TextBox();
        _descriptionLabel = new Label();
        _descriptionTextBox = new TextBox();
        _dddHeaderLabel = new Label();
        _isValueObjectCheckBox = new CheckBox();
        _isAggregateRootCheckBox = new CheckBox();
        _isHierarchicalCheckBox = new CheckBox();
        _databaseHeaderLabel = new Label();
        _tableNameLabel = new Label();
        _tableNameTextBox = new TextBox();
        _schemaLabel = new Label();
        _schemaTextBox = new TextBox();
        _codeGenHeaderLabel = new Label();
        _baseClassLabel = new Label();
        _baseClassTextBox = new TextBox();
        _isAbstractCheckBox = new CheckBox();
        _isSealedCheckBox = new CheckBox();
        _isOwnedTypeCheckBox = new CheckBox();
        _generateHeaderLabel = new Label();
        _generateRepoCheckBox = new CheckBox();
        _generateControllerCheckBox = new CheckBox();
        _generateViewModelCheckBox = new CheckBox();
        _generateViewCheckBox = new CheckBox();
        _buttonPanel = new Panel();
        _okButton = new Button();
        _cancelButton = new Button();
        _layout.SuspendLayout();
        _buttonPanel.SuspendLayout();
        SuspendLayout();
        // 
        // _layout
        // 
        _layout.ColumnCount = 2;
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _layout.Controls.Add(_entityNameLabel, 0, 0);
        _layout.Controls.Add(_nameTextBox, 1, 0);
        _layout.Controls.Add(_titleLabel, 0, 1);
        _layout.Controls.Add(_titleTextBox, 1, 1);
        _layout.Controls.Add(_descriptionLabel, 0, 2);
        _layout.Controls.Add(_descriptionTextBox, 1, 2);
        _layout.Controls.Add(_dddHeaderLabel, 0, 3);
        _layout.Controls.Add(_isValueObjectCheckBox, 1, 4);
        _layout.Controls.Add(_isAggregateRootCheckBox, 1, 5);
        _layout.Controls.Add(_isHierarchicalCheckBox, 1, 6);
        _layout.Controls.Add(_databaseHeaderLabel, 0, 7);
        _layout.Controls.Add(_tableNameLabel, 0, 8);
        _layout.Controls.Add(_tableNameTextBox, 1, 8);
        _layout.Controls.Add(_schemaLabel, 0, 9);
        _layout.Controls.Add(_schemaTextBox, 1, 9);
        _layout.Controls.Add(_codeGenHeaderLabel, 0, 10);
        _layout.Controls.Add(_baseClassLabel, 0, 11);
        _layout.Controls.Add(_baseClassTextBox, 1, 11);
        _layout.Controls.Add(_isAbstractCheckBox, 1, 12);
        _layout.Controls.Add(_isSealedCheckBox, 1, 13);
        _layout.Controls.Add(_isOwnedTypeCheckBox, 1, 14);
        _layout.Controls.Add(_generateHeaderLabel, 0, 15);
        _layout.Controls.Add(_generateRepoCheckBox, 1, 16);
        _layout.Controls.Add(_generateControllerCheckBox, 1, 17);
        _layout.Controls.Add(_generateViewModelCheckBox, 1, 18);
        _layout.Controls.Add(_generateViewCheckBox, 1, 19);
        _layout.Dock = DockStyle.Fill;
        _layout.Location = new Point(0, 0);
        _layout.Name = "_layout";
        _layout.Padding = new Padding(10);
        _layout.RowCount = 20;
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
        _layout.Size = new Size(484, 531);
        _layout.TabIndex = 0;
        // 
        // _entityNameLabel
        // 
        _entityNameLabel.Anchor = AnchorStyles.Left;
        _entityNameLabel.Location = new Point(13, 11);
        _entityNameLabel.Name = "_entityNameLabel";
        _entityNameLabel.Size = new Size(100, 23);
        _entityNameLabel.TabIndex = 0;
        _entityNameLabel.Text = "Entity Name:*";
        // 
        // _nameTextBox
        // 
        _nameTextBox.Dock = DockStyle.Fill;
        _nameTextBox.Location = new Point(163, 13);
        _nameTextBox.Name = "_nameTextBox";
        _nameTextBox.Size = new Size(308, 23);
        _nameTextBox.TabIndex = 1;
        // 
        // _titleLabel
        // 
        _titleLabel.Anchor = AnchorStyles.Left;
        _titleLabel.Location = new Point(13, 36);
        _titleLabel.Name = "_titleLabel";
        _titleLabel.Size = new Size(100, 23);
        _titleLabel.TabIndex = 2;
        _titleLabel.Text = "Title:";
        // 
        // _titleTextBox
        // 
        _titleTextBox.Dock = DockStyle.Fill;
        _titleTextBox.Location = new Point(163, 38);
        _titleTextBox.Name = "_titleTextBox";
        _titleTextBox.Size = new Size(308, 23);
        _titleTextBox.TabIndex = 3;
        // 
        // _descriptionLabel
        // 
        _descriptionLabel.Anchor = AnchorStyles.Left;
        _descriptionLabel.Location = new Point(13, 61);
        _descriptionLabel.Name = "_descriptionLabel";
        _descriptionLabel.Size = new Size(100, 23);
        _descriptionLabel.TabIndex = 4;
        _descriptionLabel.Text = "Description:";
        // 
        // _descriptionTextBox
        // 
        _descriptionTextBox.Dock = DockStyle.Fill;
        _descriptionTextBox.Location = new Point(163, 63);
        _descriptionTextBox.Multiline = true;
        _descriptionTextBox.Name = "_descriptionTextBox";
        _descriptionTextBox.Size = new Size(308, 19);
        _descriptionTextBox.TabIndex = 5;
        // 
        // _dddHeaderLabel
        // 
        _dddHeaderLabel.Anchor = AnchorStyles.Left;
        _layout.SetColumnSpan(_dddHeaderLabel, 2);
        _dddHeaderLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        _dddHeaderLabel.Location = new Point(13, 86);
        _dddHeaderLabel.Name = "_dddHeaderLabel";
        _dddHeaderLabel.Size = new Size(100, 23);
        _dddHeaderLabel.TabIndex = 6;
        _dddHeaderLabel.Text = "Domain-Driven Design";
        // 
        // _isValueObjectCheckBox
        // 
        _isValueObjectCheckBox.Anchor = AnchorStyles.Left;
        _isValueObjectCheckBox.Location = new Point(163, 113);
        _isValueObjectCheckBox.Name = "_isValueObjectCheckBox";
        _isValueObjectCheckBox.Size = new Size(104, 19);
        _isValueObjectCheckBox.TabIndex = 7;
        _isValueObjectCheckBox.Text = "Value Object (immutable, no identity)";
        // 
        // _isAggregateRootCheckBox
        // 
        _isAggregateRootCheckBox.Anchor = AnchorStyles.Left;
        _isAggregateRootCheckBox.Location = new Point(163, 138);
        _isAggregateRootCheckBox.Name = "_isAggregateRootCheckBox";
        _isAggregateRootCheckBox.Size = new Size(104, 19);
        _isAggregateRootCheckBox.TabIndex = 8;
        _isAggregateRootCheckBox.Text = "Aggregate Root (entry point)";
        // 
        // _isHierarchicalCheckBox
        // 
        _isHierarchicalCheckBox.Anchor = AnchorStyles.Left;
        _isHierarchicalCheckBox.Location = new Point(163, 163);
        _isHierarchicalCheckBox.Name = "_isHierarchicalCheckBox";
        _isHierarchicalCheckBox.Size = new Size(104, 19);
        _isHierarchicalCheckBox.TabIndex = 9;
        _isHierarchicalCheckBox.Text = "Hierarchical (parent-child)";
        // 
        // _databaseHeaderLabel
        // 
        _databaseHeaderLabel.Anchor = AnchorStyles.Left;
        _layout.SetColumnSpan(_databaseHeaderLabel, 2);
        _databaseHeaderLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        _databaseHeaderLabel.Location = new Point(13, 186);
        _databaseHeaderLabel.Name = "_databaseHeaderLabel";
        _databaseHeaderLabel.Size = new Size(100, 23);
        _databaseHeaderLabel.TabIndex = 10;
        _databaseHeaderLabel.Text = "Database Settings";
        // 
        // _tableNameLabel
        // 
        _tableNameLabel.Anchor = AnchorStyles.Left;
        _tableNameLabel.Location = new Point(13, 211);
        _tableNameLabel.Name = "_tableNameLabel";
        _tableNameLabel.Size = new Size(100, 23);
        _tableNameLabel.TabIndex = 11;
        _tableNameLabel.Text = "Table Name:";
        // 
        // _tableNameTextBox
        // 
        _tableNameTextBox.Dock = DockStyle.Fill;
        _tableNameTextBox.Location = new Point(163, 213);
        _tableNameTextBox.Name = "_tableNameTextBox";
        _tableNameTextBox.Size = new Size(308, 23);
        _tableNameTextBox.TabIndex = 12;
        // 
        // _schemaLabel
        // 
        _schemaLabel.Anchor = AnchorStyles.Left;
        _schemaLabel.Location = new Point(13, 236);
        _schemaLabel.Name = "_schemaLabel";
        _schemaLabel.Size = new Size(100, 23);
        _schemaLabel.TabIndex = 13;
        _schemaLabel.Text = "Schema:";
        // 
        // _schemaTextBox
        // 
        _schemaTextBox.Dock = DockStyle.Fill;
        _schemaTextBox.Location = new Point(163, 238);
        _schemaTextBox.Name = "_schemaTextBox";
        _schemaTextBox.Size = new Size(308, 23);
        _schemaTextBox.TabIndex = 14;
        _schemaTextBox.Text = "dbo";
        // 
        // _codeGenHeaderLabel
        // 
        _codeGenHeaderLabel.Anchor = AnchorStyles.Left;
        _layout.SetColumnSpan(_codeGenHeaderLabel, 2);
        _codeGenHeaderLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        _codeGenHeaderLabel.Location = new Point(13, 261);
        _codeGenHeaderLabel.Name = "_codeGenHeaderLabel";
        _codeGenHeaderLabel.Size = new Size(100, 23);
        _codeGenHeaderLabel.TabIndex = 15;
        _codeGenHeaderLabel.Text = "Code Generation Settings";
        // 
        // _baseClassLabel
        // 
        _baseClassLabel.Anchor = AnchorStyles.Left;
        _baseClassLabel.Location = new Point(13, 286);
        _baseClassLabel.Name = "_baseClassLabel";
        _baseClassLabel.Size = new Size(100, 23);
        _baseClassLabel.TabIndex = 16;
        _baseClassLabel.Text = "Base Class:";
        // 
        // _baseClassTextBox
        // 
        _baseClassTextBox.Dock = DockStyle.Fill;
        _baseClassTextBox.Location = new Point(163, 288);
        _baseClassTextBox.Name = "_baseClassTextBox";
        _baseClassTextBox.Size = new Size(308, 23);
        _baseClassTextBox.TabIndex = 17;
        // 
        // _isAbstractCheckBox
        // 
        _isAbstractCheckBox.Anchor = AnchorStyles.Left;
        _isAbstractCheckBox.Location = new Point(163, 313);
        _isAbstractCheckBox.Name = "_isAbstractCheckBox";
        _isAbstractCheckBox.Size = new Size(104, 19);
        _isAbstractCheckBox.TabIndex = 18;
        _isAbstractCheckBox.Text = "Is Abstract";
        // 
        // _isSealedCheckBox
        // 
        _isSealedCheckBox.Anchor = AnchorStyles.Left;
        _isSealedCheckBox.Location = new Point(163, 338);
        _isSealedCheckBox.Name = "_isSealedCheckBox";
        _isSealedCheckBox.Size = new Size(104, 19);
        _isSealedCheckBox.TabIndex = 19;
        _isSealedCheckBox.Text = "Is Sealed";
        // 
        // _isOwnedTypeCheckBox
        // 
        _isOwnedTypeCheckBox.Anchor = AnchorStyles.Left;
        _isOwnedTypeCheckBox.Location = new Point(163, 363);
        _isOwnedTypeCheckBox.Name = "_isOwnedTypeCheckBox";
        _isOwnedTypeCheckBox.Size = new Size(104, 19);
        _isOwnedTypeCheckBox.TabIndex = 20;
        _isOwnedTypeCheckBox.Text = "Is Owned Type (Value Object)";
        // 
        // _generateHeaderLabel
        // 
        _generateHeaderLabel.Anchor = AnchorStyles.Left;
        _generateHeaderLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        _generateHeaderLabel.Location = new Point(13, 386);
        _generateHeaderLabel.Name = "_generateHeaderLabel";
        _generateHeaderLabel.Size = new Size(100, 23);
        _generateHeaderLabel.TabIndex = 21;
        _generateHeaderLabel.Text = "Generate:";
        // 
        // _generateRepoCheckBox
        // 
        _generateRepoCheckBox.Anchor = AnchorStyles.Left;
        _generateRepoCheckBox.Checked = true;
        _generateRepoCheckBox.CheckState = CheckState.Checked;
        _generateRepoCheckBox.Location = new Point(163, 413);
        _generateRepoCheckBox.Name = "_generateRepoCheckBox";
        _generateRepoCheckBox.Size = new Size(104, 19);
        _generateRepoCheckBox.TabIndex = 22;
        _generateRepoCheckBox.Text = "Repository";
        // 
        // _generateControllerCheckBox
        // 
        _generateControllerCheckBox.Anchor = AnchorStyles.Left;
        _generateControllerCheckBox.Checked = true;
        _generateControllerCheckBox.CheckState = CheckState.Checked;
        _generateControllerCheckBox.Location = new Point(163, 438);
        _generateControllerCheckBox.Name = "_generateControllerCheckBox";
        _generateControllerCheckBox.Size = new Size(104, 19);
        _generateControllerCheckBox.TabIndex = 23;
        _generateControllerCheckBox.Text = "Controller";
        // 
        // _generateViewModelCheckBox
        // 
        _generateViewModelCheckBox.Anchor = AnchorStyles.Left;
        _generateViewModelCheckBox.Checked = true;
        _generateViewModelCheckBox.CheckState = CheckState.Checked;
        _generateViewModelCheckBox.Location = new Point(163, 463);
        _generateViewModelCheckBox.Name = "_generateViewModelCheckBox";
        _generateViewModelCheckBox.Size = new Size(104, 19);
        _generateViewModelCheckBox.TabIndex = 24;
        _generateViewModelCheckBox.Text = "ViewModel";
        // 
        // _generateViewCheckBox
        // 
        _generateViewCheckBox.Anchor = AnchorStyles.Left;
        _generateViewCheckBox.Checked = true;
        _generateViewCheckBox.CheckState = CheckState.Checked;
        _generateViewCheckBox.Location = new Point(163, 491);
        _generateViewCheckBox.Name = "_generateViewCheckBox";
        _generateViewCheckBox.Size = new Size(104, 24);
        _generateViewCheckBox.TabIndex = 25;
        _generateViewCheckBox.Text = "View";
        // 
        // _buttonPanel
        // 
        _buttonPanel.Controls.Add(_okButton);
        _buttonPanel.Controls.Add(_cancelButton);
        _buttonPanel.Dock = DockStyle.Bottom;
        _buttonPanel.Location = new Point(0, 531);
        _buttonPanel.Name = "_buttonPanel";
        _buttonPanel.Size = new Size(484, 50);
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
        // EntityEditorForm
        // 
        AcceptButton = _okButton;
        CancelButton = _cancelButton;
        ClientSize = new Size(484, 581);
        Controls.Add(_layout);
        Controls.Add(_buttonPanel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "EntityEditorForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "New or Edit Entity";
        _layout.ResumeLayout(false);
        _layout.PerformLayout();
        _buttonPanel.ResumeLayout(false);
        ResumeLayout(false);
    }
}
