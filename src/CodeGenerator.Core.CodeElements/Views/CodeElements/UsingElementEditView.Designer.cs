using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Core.CodeElements.Views;

partial class UsingElementEditView
{
    private System.ComponentModel.IContainer components = null;

    #region Component Designer generated code

    private void InitializeComponent()
    {
        codeElementEditView = new CodeElementEditView();
        txtNamespace = new SingleLineTextField();
        txtAlias = new SingleLineTextField();
        chkIsGlobal = new BooleanField();
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
        tableLayoutPanel.Controls.Add(txtNamespace, 0, 1);
        tableLayoutPanel.Controls.Add(txtAlias, 0, 2);
        tableLayoutPanel.Controls.Add(chkIsGlobal, 0, 3);
        tableLayoutPanel.Location = new Point(0, 0);
        tableLayoutPanel.Name = "tableLayoutPanel";
        tableLayoutPanel.RowCount = 4;
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.Size = new Size(380, 450);
        tableLayoutPanel.TabIndex = 0;
        // 
        // codeElementEditView
        // 
        codeElementEditView.Dock = DockStyle.Top;
        codeElementEditView.Name = "codeElementEditView";
        codeElementEditView.Size = new Size(374, 320);
        codeElementEditView.TabIndex = 0;
        // 
        // txtNamespace
        // 
        txtNamespace.Dock = DockStyle.Top;
        txtNamespace.Label = "Namespace:";
        txtNamespace.Name = "txtNamespace";
        txtNamespace.Size = new Size(374, 50);
        txtNamespace.TabIndex = 1;
        // 
        // txtAlias
        // 
        txtAlias.Dock = DockStyle.Top;
        txtAlias.Label = "Alias:";
        txtAlias.Name = "txtAlias";
        txtAlias.Size = new Size(374, 50);
        txtAlias.TabIndex = 2;
        // 
        // chkIsGlobal
        // 
        chkIsGlobal.Dock = DockStyle.Top;
        chkIsGlobal.Label = "Is Global:";
        chkIsGlobal.Name = "chkIsGlobal";
        chkIsGlobal.Size = new Size(374, 30);
        chkIsGlobal.TabIndex = 3;
        // 
        // UsingElementEditView
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoSize = true;
        Controls.Add(tableLayoutPanel);
        Name = "UsingElementEditView";
        Size = new Size(380, 450);
        tableLayoutPanel.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TableLayoutPanel tableLayoutPanel;
    private CodeElementEditView codeElementEditView;
    private SingleLineTextField txtNamespace;
    private SingleLineTextField txtAlias;
    private BooleanField chkIsGlobal;
}
