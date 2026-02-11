using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Core.CodeElements.Views.Statements;

partial class ReturnStatementEditView
{
    private System.ComponentModel.IContainer components = null;

    #region Component Designer generated code

    private void InitializeComponent()
    {
        txtExpression = new SingleLineTextField();
        tableLayoutPanel = new TableLayoutPanel();
        tableLayoutPanel.SuspendLayout();
        SuspendLayout();
        tableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        tableLayoutPanel.AutoSize = true;
        tableLayoutPanel.ColumnCount = 1;
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanel.Controls.Add(txtExpression, 0, 0);
        tableLayoutPanel.Location = new Point(0, 0);
        tableLayoutPanel.Name = "tableLayoutPanel";
        tableLayoutPanel.RowCount = 1;
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.Size = new Size(380, 50);
        tableLayoutPanel.TabIndex = 0;
        txtExpression.Dock = DockStyle.Top; txtExpression.Label = "Expression:"; txtExpression.Name = "txtExpression"; txtExpression.Size = new Size(374, 50); txtExpression.TabIndex = 0;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoSize = true;
        Controls.Add(tableLayoutPanel);
        Name = "ReturnStatementEditView";
        Size = new Size(380, 50);
        tableLayoutPanel.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TableLayoutPanel tableLayoutPanel;
    private SingleLineTextField txtExpression;
}
