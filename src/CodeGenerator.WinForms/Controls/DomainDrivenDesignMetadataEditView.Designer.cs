namespace CodeGenerator.WinForms.Controls;

partial class DomainDrivenDesignMetadataEditView
{
    private System.ComponentModel.IContainer components = null;

    private TableLayoutPanel _layout;
    private Label _boundedContextLabel;
    private TextBox _boundedContextTextBox;

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
        _boundedContextLabel = new Label();
        _boundedContextTextBox = new TextBox();
        _layout.SuspendLayout();
        SuspendLayout();
        // 
        // _layout
        // 
        _layout.ColumnCount = 2;
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _layout.Controls.Add(_boundedContextLabel, 0, 0);
        _layout.Controls.Add(_boundedContextTextBox, 1, 0);
        _layout.Dock = DockStyle.Fill;
        _layout.Location = new Point(0, 0);
        _layout.Name = "_layout";
        _layout.Padding = new Padding(10);
        _layout.RowCount = 2;
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        _layout.Size = new Size(410, 172);
        _layout.TabIndex = 0;
        // 
        // _boundedContextLabel
        // 
        _boundedContextLabel.Anchor = AnchorStyles.Left;
        _boundedContextLabel.Location = new Point(13, 13);
        _boundedContextLabel.Name = "_boundedContextLabel";
        _boundedContextLabel.Size = new Size(100, 23);
        _boundedContextLabel.TabIndex = 0;
        _boundedContextLabel.Text = "Bounded Context:";
        // 
        // _boundedContextTextBox
        // 
        _boundedContextTextBox.Dock = DockStyle.Fill;
        _boundedContextTextBox.Location = new Point(163, 13);
        _boundedContextTextBox.Name = "_boundedContextTextBox";
        _boundedContextTextBox.Size = new Size(234, 23);
        _boundedContextTextBox.TabIndex = 1;
        // 
        // DomainDrivenDesignMetadataEditView
        // 
        Controls.Add(_layout);
        Name = "DomainDrivenDesignMetadataEditView";
        Size = new Size(410, 172);
        _layout.ResumeLayout(false);
        _layout.PerformLayout();
        ResumeLayout(false);
    }
}
