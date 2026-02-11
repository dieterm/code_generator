using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Core.CodeElements.Views;

partial class ClassElementEditView
{
    private System.ComponentModel.IContainer components = null;

    #region Component Designer generated code

    private void InitializeComponent()
    {
        codeElementEditView = new CodeElementEditView();
        chkIsRecord = new BooleanField();
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
        tableLayoutPanel.Controls.Add(chkIsRecord, 0, 1);
        tableLayoutPanel.Location = new Point(0, 0);
        tableLayoutPanel.Name = "tableLayoutPanel";
        tableLayoutPanel.RowCount = 2;
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.Size = new Size(380, 360);
        tableLayoutPanel.TabIndex = 0;
        // 
        // codeElementEditView
        // 
        codeElementEditView.Dock = DockStyle.Top;
        codeElementEditView.Name = "codeElementEditView";
        codeElementEditView.Size = new Size(374, 320);
        codeElementEditView.TabIndex = 0;
        // 
        // chkIsRecord
        // 
        chkIsRecord.Dock = DockStyle.Top;
        chkIsRecord.Label = "Is Record:";
        chkIsRecord.Name = "chkIsRecord";
        chkIsRecord.Size = new Size(374, 30);
        chkIsRecord.TabIndex = 1;
        // 
        // ClassElementEditView
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoSize = true;
        Controls.Add(tableLayoutPanel);
        Name = "ClassElementEditView";
        Size = new Size(380, 360);
        tableLayoutPanel.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TableLayoutPanel tableLayoutPanel;
    private CodeElementEditView codeElementEditView;
    private BooleanField chkIsRecord;
}
