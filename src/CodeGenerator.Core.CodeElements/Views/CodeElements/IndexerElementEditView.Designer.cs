using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Core.CodeElements.Views;

partial class IndexerElementEditView
{
    private System.ComponentModel.IContainer components = null;

    #region Component Designer generated code

    private void InitializeComponent()
    {
        codeElementEditView = new CodeElementEditView();
        txtTypeName = new SingleLineTextField();
        chkHasGetter = new BooleanField();
        chkHasSetter = new BooleanField();
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
        tableLayoutPanel.Controls.Add(txtTypeName, 0, 1);
        tableLayoutPanel.Controls.Add(chkHasGetter, 0, 2);
        tableLayoutPanel.Controls.Add(chkHasSetter, 0, 3);
        tableLayoutPanel.Location = new Point(0, 0);
        tableLayoutPanel.Name = "tableLayoutPanel";
        tableLayoutPanel.RowCount = 4;
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.Size = new Size(380, 430);
        tableLayoutPanel.TabIndex = 0;
        // 
        // codeElementEditView
        // 
        codeElementEditView.Dock = DockStyle.Top;
        codeElementEditView.Name = "codeElementEditView";
        codeElementEditView.Size = new Size(374, 320);
        codeElementEditView.TabIndex = 0;
        // 
        // txtTypeName
        // 
        txtTypeName.Dock = DockStyle.Top;
        txtTypeName.Label = "Type:";
        txtTypeName.Name = "txtTypeName";
        txtTypeName.Size = new Size(374, 50);
        txtTypeName.TabIndex = 1;
        // 
        // chkHasGetter
        // 
        chkHasGetter.Dock = DockStyle.Top;
        chkHasGetter.Label = "Has Getter:";
        chkHasGetter.Name = "chkHasGetter";
        chkHasGetter.Size = new Size(374, 30);
        chkHasGetter.TabIndex = 2;
        // 
        // chkHasSetter
        // 
        chkHasSetter.Dock = DockStyle.Top;
        chkHasSetter.Label = "Has Setter:";
        chkHasSetter.Name = "chkHasSetter";
        chkHasSetter.Size = new Size(374, 30);
        chkHasSetter.TabIndex = 3;
        // 
        // IndexerElementEditView
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoSize = true;
        Controls.Add(tableLayoutPanel);
        Name = "IndexerElementEditView";
        Size = new Size(380, 430);
        tableLayoutPanel.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TableLayoutPanel tableLayoutPanel;
    private CodeElementEditView codeElementEditView;
    private SingleLineTextField txtTypeName;
    private BooleanField chkHasGetter;
    private BooleanField chkHasSetter;
}
