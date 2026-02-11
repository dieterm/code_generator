using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Core.CodeElements.Views;

partial class ConstructorElementEditView
{
    private System.ComponentModel.IContainer components = null;

    #region Component Designer generated code

    private void InitializeComponent()
    {
        codeElementEditView = new CodeElementEditView();
        chkIsPrimary = new BooleanField();
        chkIsStatic = new BooleanField();
        tableLayoutPanel = new TableLayoutPanel();
        tableLayoutPanel.SuspendLayout();
        SuspendLayout();
        // 
        // tableLayoutPanel
        // 
        tableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        tableLayoutPanel.AutoSize = true;
        tableLayoutPanel.ColumnCount = 1;
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanel.Controls.Add(codeElementEditView, 0, 0);
        tableLayoutPanel.Controls.Add(chkIsPrimary, 0, 1);
        tableLayoutPanel.Controls.Add(chkIsStatic, 0, 2);
        tableLayoutPanel.Location = new Point(0, 0);
        tableLayoutPanel.Name = "tableLayoutPanel";
        tableLayoutPanel.RowCount = 3;
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.Size = new Size(380, 380);
        tableLayoutPanel.TabIndex = 0;
        // 
        // codeElementEditView
        // 
        codeElementEditView.Dock = DockStyle.Top;
        codeElementEditView.Name = "codeElementEditView";
        codeElementEditView.Size = new Size(374, 320);
        codeElementEditView.TabIndex = 0;
        // 
        // chkIsPrimary
        // 
        chkIsPrimary.Dock = DockStyle.Top;
        chkIsPrimary.Label = "Is Primary:";
        chkIsPrimary.Name = "chkIsPrimary";
        chkIsPrimary.Size = new Size(374, 30);
        chkIsPrimary.TabIndex = 1;
        // 
        // chkIsStatic
        // 
        chkIsStatic.Dock = DockStyle.Top;
        chkIsStatic.Label = "Is Static:";
        chkIsStatic.Name = "chkIsStatic";
        chkIsStatic.Size = new Size(374, 30);
        chkIsStatic.TabIndex = 2;
        // 
        // ConstructorElementEditView
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoSize = true;
        Controls.Add(tableLayoutPanel);
        Name = "ConstructorElementEditView";
        Size = new Size(380, 380);
        tableLayoutPanel.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TableLayoutPanel tableLayoutPanel;
    private CodeElementEditView codeElementEditView;
    private BooleanField chkIsPrimary;
    private BooleanField chkIsStatic;
}
