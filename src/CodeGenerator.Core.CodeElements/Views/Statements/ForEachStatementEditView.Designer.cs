using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Core.CodeElements.Views.Statements;

partial class ForEachStatementEditView
{
    private System.ComponentModel.IContainer components = null;

    #region Component Designer generated code

    private void InitializeComponent()
    {
        txtVariableName = new SingleLineTextField();
        txtCollection = new SingleLineTextField();
        tableLayoutPanel = new TableLayoutPanel();
        tableLayoutPanel.SuspendLayout();
        SuspendLayout();
        tableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        tableLayoutPanel.AutoSize = true;
        tableLayoutPanel.ColumnCount = 1;
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanel.Controls.Add(txtVariableName, 0, 0);
        tableLayoutPanel.Controls.Add(txtCollection, 0, 1);
        tableLayoutPanel.Location = new Point(0, 0);
        tableLayoutPanel.Name = "tableLayoutPanel";
        tableLayoutPanel.RowCount = 2;
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.Size = new Size(380, 100);
        tableLayoutPanel.TabIndex = 0;
        txtVariableName.Dock = DockStyle.Top; txtVariableName.Label = "Variable Name:"; txtVariableName.Name = "txtVariableName"; txtVariableName.Size = new Size(374, 50); txtVariableName.TabIndex = 0;
        txtCollection.Dock = DockStyle.Top; txtCollection.Label = "Collection:"; txtCollection.Name = "txtCollection"; txtCollection.Size = new Size(374, 50); txtCollection.TabIndex = 1;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoSize = true;
        Controls.Add(tableLayoutPanel);
        Name = "ForEachStatementEditView";
        Size = new Size(380, 100);
        tableLayoutPanel.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TableLayoutPanel tableLayoutPanel;
    private SingleLineTextField txtVariableName;
    private SingleLineTextField txtCollection;
}
