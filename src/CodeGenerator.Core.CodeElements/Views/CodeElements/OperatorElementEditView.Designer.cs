using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Core.CodeElements.Views;

partial class OperatorElementEditView
{
    private System.ComponentModel.IContainer components = null;

    #region Component Designer generated code

    private void InitializeComponent()
    {
        codeElementEditView = new CodeElementEditView();
        cbxOperatorType = new ComboboxField();
        txtReturnType = new SingleLineTextField();
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
        tableLayoutPanel.Controls.Add(cbxOperatorType, 0, 1);
        tableLayoutPanel.Controls.Add(txtReturnType, 0, 2);
        tableLayoutPanel.Location = new Point(0, 0);
        tableLayoutPanel.Name = "tableLayoutPanel";
        tableLayoutPanel.RowCount = 3;
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.Size = new Size(380, 420);
        tableLayoutPanel.TabIndex = 0;
        // 
        // codeElementEditView
        // 
        codeElementEditView.Dock = DockStyle.Top;
        codeElementEditView.Name = "codeElementEditView";
        codeElementEditView.Size = new Size(374, 320);
        codeElementEditView.TabIndex = 0;
        // 
        // cbxOperatorType
        // 
        cbxOperatorType.Dock = DockStyle.Top;
        cbxOperatorType.ErrorMessageVisible = true;
        cbxOperatorType.Label = "Operator Type:";
        cbxOperatorType.Name = "cbxOperatorType";
        cbxOperatorType.Size = new Size(374, 50);
        cbxOperatorType.TabIndex = 1;
        // 
        // txtReturnType
        // 
        txtReturnType.Dock = DockStyle.Top;
        txtReturnType.Label = "Return Type:";
        txtReturnType.Name = "txtReturnType";
        txtReturnType.Size = new Size(374, 50);
        txtReturnType.TabIndex = 2;
        // 
        // OperatorElementEditView
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoSize = true;
        Controls.Add(tableLayoutPanel);
        Name = "OperatorElementEditView";
        Size = new Size(380, 420);
        tableLayoutPanel.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TableLayoutPanel tableLayoutPanel;
    private CodeElementEditView codeElementEditView;
    private ComboboxField cbxOperatorType;
    private SingleLineTextField txtReturnType;
}
