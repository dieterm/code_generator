namespace CodeGenerator.WinForms.Controls;

partial class EntityDefinitionEditView
{
    private System.ComponentModel.IContainer components = null;

    private TableLayoutPanel _layout;
    private Label _titleLabel;
    private TextBox _titleTextBox;
    private Label _descriptionLabel;
    private TextBox _descriptionTextBox;
    private Label _dddHeaderLabel;
    private CheckBox _isValueObjectCheckBox;
    private CheckBox _isAggregateRootCheckBox;
    private CheckBox _isHierarchicalCheckBox;

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
        txtKey = new TextBox();
        _titleLabel = new Label();
        _titleTextBox = new TextBox();
        _descriptionLabel = new Label();
        _descriptionTextBox = new TextBox();
        _dddHeaderLabel = new Label();
        _isValueObjectCheckBox = new CheckBox();
        _isAggregateRootCheckBox = new CheckBox();
        _isHierarchicalCheckBox = new CheckBox();
        lblKey = new Label();
        _layout.SuspendLayout();
        SuspendLayout();
        // 
        // _layout
        // 
        _layout.ColumnCount = 2;
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _layout.Controls.Add(txtKey, 1, 0);
        _layout.Controls.Add(_titleLabel, 0, 1);
        _layout.Controls.Add(_titleTextBox, 1, 1);
        _layout.Controls.Add(_descriptionLabel, 0, 2);
        _layout.Controls.Add(_descriptionTextBox, 1, 2);
        _layout.Controls.Add(_dddHeaderLabel, 0, 3);
        _layout.Controls.Add(_isValueObjectCheckBox, 1, 4);
        _layout.Controls.Add(_isAggregateRootCheckBox, 1, 5);
        _layout.Controls.Add(_isHierarchicalCheckBox, 1, 6);
        _layout.Controls.Add(lblKey, 0, 0);
        _layout.Dock = DockStyle.Fill;
        _layout.Location = new Point(0, 0);
        _layout.Name = "_layout";
        _layout.Padding = new Padding(10);
        _layout.RowCount = 8;
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _layout.RowStyles.Add(new RowStyle());
        _layout.Size = new Size(423, 265);
        _layout.TabIndex = 0;
        // 
        // txtKey
        // 
        txtKey.Dock = DockStyle.Fill;
        txtKey.Location = new Point(133, 13);
        txtKey.Name = "txtKey";
        txtKey.Size = new Size(277, 23);
        txtKey.TabIndex = 9;
        // 
        // _titleLabel
        // 
        _titleLabel.Anchor = AnchorStyles.Left;
        _titleLabel.Location = new Point(13, 43);
        _titleLabel.Name = "_titleLabel";
        _titleLabel.Size = new Size(100, 23);
        _titleLabel.TabIndex = 0;
        _titleLabel.Text = "Title:";
        // 
        // _titleTextBox
        // 
        _titleTextBox.Dock = DockStyle.Fill;
        _titleTextBox.Location = new Point(133, 43);
        _titleTextBox.Name = "_titleTextBox";
        _titleTextBox.Size = new Size(277, 23);
        _titleTextBox.TabIndex = 1;
        // 
        // _descriptionLabel
        // 
        _descriptionLabel.Location = new Point(13, 70);
        _descriptionLabel.Name = "_descriptionLabel";
        _descriptionLabel.Size = new Size(100, 23);
        _descriptionLabel.TabIndex = 2;
        _descriptionLabel.Text = "Description:";
        // 
        // _descriptionTextBox
        // 
        _descriptionTextBox.Dock = DockStyle.Fill;
        _descriptionTextBox.Location = new Point(133, 73);
        _descriptionTextBox.Multiline = true;
        _descriptionTextBox.Name = "_descriptionTextBox";
        _descriptionTextBox.Size = new Size(277, 24);
        _descriptionTextBox.TabIndex = 3;
        // 
        // _dddHeaderLabel
        // 
        _dddHeaderLabel.Anchor = AnchorStyles.Left;
        _layout.SetColumnSpan(_dddHeaderLabel, 2);
        _dddHeaderLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        _dddHeaderLabel.Location = new Point(13, 103);
        _dddHeaderLabel.Name = "_dddHeaderLabel";
        _dddHeaderLabel.Size = new Size(100, 23);
        _dddHeaderLabel.TabIndex = 4;
        _dddHeaderLabel.Text = "Domain-Driven Design";
        // 
        // _isValueObjectCheckBox
        // 
        _isValueObjectCheckBox.Anchor = AnchorStyles.Left;
        _isValueObjectCheckBox.Location = new Point(133, 133);
        _isValueObjectCheckBox.Name = "_isValueObjectCheckBox";
        _isValueObjectCheckBox.Size = new Size(104, 24);
        _isValueObjectCheckBox.TabIndex = 5;
        _isValueObjectCheckBox.Text = "Value Object (immutable, no identity)";
        // 
        // _isAggregateRootCheckBox
        // 
        _isAggregateRootCheckBox.Anchor = AnchorStyles.Left;
        _isAggregateRootCheckBox.Location = new Point(133, 163);
        _isAggregateRootCheckBox.Name = "_isAggregateRootCheckBox";
        _isAggregateRootCheckBox.Size = new Size(104, 24);
        _isAggregateRootCheckBox.TabIndex = 6;
        _isAggregateRootCheckBox.Text = "Aggregate Root (entry point)";
        // 
        // _isHierarchicalCheckBox
        // 
        _isHierarchicalCheckBox.Anchor = AnchorStyles.Left;
        _isHierarchicalCheckBox.Location = new Point(133, 193);
        _isHierarchicalCheckBox.Name = "_isHierarchicalCheckBox";
        _isHierarchicalCheckBox.Size = new Size(104, 24);
        _isHierarchicalCheckBox.TabIndex = 7;
        _isHierarchicalCheckBox.Text = "Hierarchical (parent-child)";
        // 
        // lblKey
        // 
        lblKey.Anchor = AnchorStyles.Left;
        lblKey.Location = new Point(13, 13);
        lblKey.Name = "lblKey";
        lblKey.Size = new Size(100, 23);
        lblKey.TabIndex = 8;
        lblKey.Text = "Key:";
        // 
        // EntityDefinitionEditView
        // 
        Controls.Add(_layout);
        Name = "EntityDefinitionEditView";
        Size = new Size(423, 265);
        _layout.ResumeLayout(false);
        _layout.PerformLayout();
        ResumeLayout(false);
    }

    private TextBox txtKey;
    private Label lblKey;
}
