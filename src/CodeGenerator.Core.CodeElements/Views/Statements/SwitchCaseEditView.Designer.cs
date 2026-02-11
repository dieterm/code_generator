using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Core.CodeElements.Views.Statements;

partial class SwitchCaseEditView
{
    private System.ComponentModel.IContainer components = null;

    #region Component Designer generated code

    private void InitializeComponent()
    {
        txtPattern = new SingleLineTextField();
        txtWhenClause = new SingleLineTextField();
        tableLayoutPanel = new TableLayoutPanel();
        tableLayoutPanel.SuspendLayout();
        SuspendLayout();
        tableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        tableLayoutPanel.AutoSize = true;
        tableLayoutPanel.ColumnCount = 1;
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanel.Controls.Add(txtPattern, 0, 0);
        tableLayoutPanel.Controls.Add(txtWhenClause, 0, 1);
        tableLayoutPanel.Location = new Point(0, 0);
        tableLayoutPanel.Name = "tableLayoutPanel";
        tableLayoutPanel.RowCount = 2;
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.Size = new Size(380, 100);
        tableLayoutPanel.TabIndex = 0;
        txtPattern.Dock = DockStyle.Top; txtPattern.Label = "Pattern:"; txtPattern.Name = "txtPattern"; txtPattern.Size = new Size(374, 50); txtPattern.TabIndex = 0;
        txtWhenClause.Dock = DockStyle.Top; txtWhenClause.Label = "When Clause:"; txtWhenClause.Name = "txtWhenClause"; txtWhenClause.Size = new Size(374, 50); txtWhenClause.TabIndex = 1;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoSize = true;
        Controls.Add(tableLayoutPanel);
        Name = "SwitchCaseEditView";
        Size = new Size(380, 100);
        tableLayoutPanel.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TableLayoutPanel tableLayoutPanel;
    private SingleLineTextField txtPattern;
    private SingleLineTextField txtWhenClause;
}
