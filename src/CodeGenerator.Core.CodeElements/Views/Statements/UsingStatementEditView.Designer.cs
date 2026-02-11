using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Core.CodeElements.Views.Statements;

partial class UsingStatementEditView
{
    private System.ComponentModel.IContainer components = null;

    #region Component Designer generated code

    private void InitializeComponent()
    {
        txtResource = new SingleLineTextField();
        chkIsDeclaration = new BooleanField();
        tableLayoutPanel = new TableLayoutPanel();
        tableLayoutPanel.SuspendLayout();
        SuspendLayout();
        tableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        tableLayoutPanel.AutoSize = true;
        tableLayoutPanel.ColumnCount = 1;
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanel.Controls.Add(txtResource, 0, 0);
        tableLayoutPanel.Controls.Add(chkIsDeclaration, 0, 1);
        tableLayoutPanel.Location = new Point(0, 0);
        tableLayoutPanel.Name = "tableLayoutPanel";
        tableLayoutPanel.RowCount = 2;
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.Size = new Size(380, 80);
        tableLayoutPanel.TabIndex = 0;
        txtResource.Dock = DockStyle.Top; txtResource.Label = "Resource:"; txtResource.Name = "txtResource"; txtResource.Size = new Size(374, 50); txtResource.TabIndex = 0;
        chkIsDeclaration.Dock = DockStyle.Top; chkIsDeclaration.Label = "Is Declaration:"; chkIsDeclaration.Name = "chkIsDeclaration"; chkIsDeclaration.Size = new Size(374, 30); chkIsDeclaration.TabIndex = 1;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoSize = true;
        Controls.Add(tableLayoutPanel);
        Name = "UsingStatementEditView";
        Size = new Size(380, 80);
        tableLayoutPanel.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TableLayoutPanel tableLayoutPanel;
    private SingleLineTextField txtResource;
    private BooleanField chkIsDeclaration;
}
