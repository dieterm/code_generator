using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Core.CodeElements.Views.Statements;

partial class AssignmentStatementEditView
{
    private System.ComponentModel.IContainer components = null;

    #region Component Designer generated code

    private void InitializeComponent()
    {
        txtLeft = new SingleLineTextField();
        txtRight = new SingleLineTextField();
        tableLayoutPanel = new TableLayoutPanel();
        tableLayoutPanel.SuspendLayout();
        SuspendLayout();
        tableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        tableLayoutPanel.AutoSize = true;
        tableLayoutPanel.ColumnCount = 1;
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanel.Controls.Add(txtLeft, 0, 0);
        tableLayoutPanel.Controls.Add(txtRight, 0, 1);
        tableLayoutPanel.Location = new Point(0, 0);
        tableLayoutPanel.Name = "tableLayoutPanel";
        tableLayoutPanel.RowCount = 2;
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.Size = new Size(380, 100);
        tableLayoutPanel.TabIndex = 0;
        txtLeft.Dock = DockStyle.Top; txtLeft.Label = "Left:"; txtLeft.Name = "txtLeft"; txtLeft.Size = new Size(374, 50); txtLeft.TabIndex = 0;
        txtRight.Dock = DockStyle.Top; txtRight.Label = "Right:"; txtRight.Name = "txtRight"; txtRight.Size = new Size(374, 50); txtRight.TabIndex = 1;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoSize = true;
        Controls.Add(tableLayoutPanel);
        Name = "AssignmentStatementEditView";
        Size = new Size(380, 100);
        tableLayoutPanel.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TableLayoutPanel tableLayoutPanel;
    private SingleLineTextField txtLeft;
    private SingleLineTextField txtRight;
}
