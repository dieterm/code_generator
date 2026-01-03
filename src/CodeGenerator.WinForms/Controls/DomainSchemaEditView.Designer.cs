namespace CodeGenerator.WinForms.Controls;

partial class DomainSchemaEditView
{
    private System.ComponentModel.IContainer components = null;

    private TableLayoutPanel _layout;
    private Label _schemaUrlLabel;
    private TextBox _schemaUrlTextBox;
    private Label _titleLabel;
    private TextBox _titleTextBox;
    private Label _descriptionLabel;
    private TextBox _descriptionTextBox;

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
        _schemaUrlLabel = new Label();
        _schemaUrlTextBox = new TextBox();
        _titleLabel = new Label();
        _titleTextBox = new TextBox();
        _descriptionLabel = new Label();
        _descriptionTextBox = new TextBox();
        _layout.SuspendLayout();
        SuspendLayout();
        // 
        // _layout
        // 
        _layout.ColumnCount = 2;
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _layout.Controls.Add(_schemaUrlLabel, 0, 0);
        _layout.Controls.Add(_schemaUrlTextBox, 1, 0);
        _layout.Controls.Add(_titleLabel, 0, 1);
        _layout.Controls.Add(_titleTextBox, 1, 1);
        _layout.Controls.Add(_descriptionLabel, 0, 2);
        _layout.Controls.Add(_descriptionTextBox, 1, 2);
        _layout.Dock = DockStyle.Fill;
        _layout.Location = new Point(0, 0);
        _layout.Name = "_layout";
        _layout.Padding = new Padding(10);
        _layout.RowCount = 4;
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _layout.RowStyles.Add(new RowStyle());
        _layout.Size = new Size(417, 229);
        _layout.TabIndex = 0;
        // 
        // _schemaUrlLabel
        // 
        _schemaUrlLabel.Anchor = AnchorStyles.Left;
        _schemaUrlLabel.Location = new Point(13, 13);
        _schemaUrlLabel.Name = "_schemaUrlLabel";
        _schemaUrlLabel.Size = new Size(100, 23);
        _schemaUrlLabel.TabIndex = 0;
        _schemaUrlLabel.Text = "Schema URL:";
        // 
        // _schemaUrlTextBox
        // 
        _schemaUrlTextBox.Dock = DockStyle.Fill;
        _schemaUrlTextBox.Location = new Point(133, 13);
        _schemaUrlTextBox.Name = "_schemaUrlTextBox";
        _schemaUrlTextBox.Size = new Size(271, 23);
        _schemaUrlTextBox.TabIndex = 1;
        // 
        // _titleLabel
        // 
        _titleLabel.Anchor = AnchorStyles.Left;
        _titleLabel.Location = new Point(13, 43);
        _titleLabel.Name = "_titleLabel";
        _titleLabel.Size = new Size(100, 23);
        _titleLabel.TabIndex = 2;
        _titleLabel.Text = "Title:*";
        // 
        // _titleTextBox
        // 
        _titleTextBox.Dock = DockStyle.Fill;
        _titleTextBox.Location = new Point(133, 43);
        _titleTextBox.Name = "_titleTextBox";
        _titleTextBox.Size = new Size(271, 23);
        _titleTextBox.TabIndex = 3;
        // 
        // _descriptionLabel
        // 
        _descriptionLabel.Location = new Point(13, 70);
        _descriptionLabel.Name = "_descriptionLabel";
        _descriptionLabel.Size = new Size(100, 23);
        _descriptionLabel.TabIndex = 4;
        _descriptionLabel.Text = "Description:";
        // 
        // _descriptionTextBox
        // 
        _descriptionTextBox.Dock = DockStyle.Fill;
        _descriptionTextBox.Location = new Point(133, 73);
        _descriptionTextBox.Multiline = true;
        _descriptionTextBox.Name = "_descriptionTextBox";
        _descriptionTextBox.Size = new Size(271, 24);
        _descriptionTextBox.TabIndex = 5;
        // 
        // DomainSchemaEditView
        // 
        Controls.Add(_layout);
        Name = "DomainSchemaEditView";
        Size = new Size(417, 229);
        _layout.ResumeLayout(false);
        _layout.PerformLayout();
        ResumeLayout(false);
    }
}
