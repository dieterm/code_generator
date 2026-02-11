using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Core.CodeElements.Views.Statements;

partial class CatchBlockEditView
{
    private System.ComponentModel.IContainer components = null;

    #region Component Designer generated code

    private void InitializeComponent()
    {
        txtExceptionVariable = new SingleLineTextField();
        txtWhenFilter = new SingleLineTextField();
        tableLayoutPanel = new TableLayoutPanel();
        tableLayoutPanel.SuspendLayout();
        SuspendLayout();
        tableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        tableLayoutPanel.AutoSize = true;
        tableLayoutPanel.ColumnCount = 1;
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanel.Controls.Add(txtExceptionVariable, 0, 0);
        tableLayoutPanel.Controls.Add(txtWhenFilter, 0, 1);
        tableLayoutPanel.Location = new Point(0, 0);
        tableLayoutPanel.Name = "tableLayoutPanel";
        tableLayoutPanel.RowCount = 2;
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.Size = new Size(380, 100);
        tableLayoutPanel.TabIndex = 0;
        txtExceptionVariable.Dock = DockStyle.Top; txtExceptionVariable.Label = "Exception Variable:"; txtExceptionVariable.Name = "txtExceptionVariable"; txtExceptionVariable.Size = new Size(374, 50); txtExceptionVariable.TabIndex = 0;
        txtWhenFilter.Dock = DockStyle.Top; txtWhenFilter.Label = "When Filter:"; txtWhenFilter.Name = "txtWhenFilter"; txtWhenFilter.Size = new Size(374, 50); txtWhenFilter.TabIndex = 1;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoSize = true;
        Controls.Add(tableLayoutPanel);
        Name = "CatchBlockEditView";
        Size = new Size(380, 100);
        tableLayoutPanel.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TableLayoutPanel tableLayoutPanel;
    private SingleLineTextField txtExceptionVariable;
    private SingleLineTextField txtWhenFilter;
}
