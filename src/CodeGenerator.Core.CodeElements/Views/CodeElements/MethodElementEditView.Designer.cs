using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Core.CodeElements.Views;

partial class MethodElementEditView
{
    private System.ComponentModel.IContainer components = null;

    #region Component Designer generated code

    private void InitializeComponent()
    {
        codeElementEditView = new CodeElementEditView();
        txtReturnType = new SingleLineTextField();
        chkIsExpressionBodied = new BooleanField();
        txtExpressionBody = new SingleLineTextField();
        chkIsExtensionMethod = new BooleanField();
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
        tableLayoutPanel.Controls.Add(txtReturnType, 0, 1);
        tableLayoutPanel.Controls.Add(chkIsExpressionBodied, 0, 2);
        tableLayoutPanel.Controls.Add(txtExpressionBody, 0, 3);
        tableLayoutPanel.Controls.Add(chkIsExtensionMethod, 0, 4);
        tableLayoutPanel.Location = new Point(0, 0);
        tableLayoutPanel.Name = "tableLayoutPanel";
        tableLayoutPanel.RowCount = 5;
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.Size = new Size(380, 480);
        tableLayoutPanel.TabIndex = 0;
        // 
        // codeElementEditView
        // 
        codeElementEditView.Dock = DockStyle.Top;
        codeElementEditView.Name = "codeElementEditView";
        codeElementEditView.Size = new Size(374, 320);
        codeElementEditView.TabIndex = 0;
        // 
        // txtReturnType
        // 
        txtReturnType.Dock = DockStyle.Top;
        txtReturnType.Label = "Return Type:";
        txtReturnType.Name = "txtReturnType";
        txtReturnType.Size = new Size(374, 50);
        txtReturnType.TabIndex = 1;
        // 
        // chkIsExpressionBodied
        // 
        chkIsExpressionBodied.Dock = DockStyle.Top;
        chkIsExpressionBodied.Label = "Is Expression Bodied:";
        chkIsExpressionBodied.Name = "chkIsExpressionBodied";
        chkIsExpressionBodied.Size = new Size(374, 30);
        chkIsExpressionBodied.TabIndex = 2;
        // 
        // txtExpressionBody
        // 
        txtExpressionBody.Dock = DockStyle.Top;
        txtExpressionBody.Label = "Expression Body:";
        txtExpressionBody.Name = "txtExpressionBody";
        txtExpressionBody.Size = new Size(374, 50);
        txtExpressionBody.TabIndex = 3;
        // 
        // chkIsExtensionMethod
        // 
        chkIsExtensionMethod.Dock = DockStyle.Top;
        chkIsExtensionMethod.Label = "Is Extension Method:";
        chkIsExtensionMethod.Name = "chkIsExtensionMethod";
        chkIsExtensionMethod.Size = new Size(374, 30);
        chkIsExtensionMethod.TabIndex = 4;
        // 
        // MethodElementEditView
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoSize = true;
        Controls.Add(tableLayoutPanel);
        Name = "MethodElementEditView";
        Size = new Size(380, 480);
        tableLayoutPanel.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TableLayoutPanel tableLayoutPanel;
    private CodeElementEditView codeElementEditView;
    private SingleLineTextField txtReturnType;
    private BooleanField chkIsExpressionBodied;
    private SingleLineTextField txtExpressionBody;
    private BooleanField chkIsExtensionMethod;
}
