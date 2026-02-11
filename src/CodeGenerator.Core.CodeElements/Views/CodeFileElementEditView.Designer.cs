using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Core.CodeElements.Views;

partial class CodeFileElementEditView
{
    private System.ComponentModel.IContainer components = null;

    #region Component Designer generated code

    private void InitializeComponent()
    {
        codeElementEditView = new CodeElementEditView();
        txtFileHeader = new SingleLineTextField();
        cbxLanguage = new ComboboxField();
        chkNullableContext = new BooleanField();
        chkUseImplicitUsings = new BooleanField();
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
        tableLayoutPanel.Controls.Add(txtFileHeader, 0, 1);
        tableLayoutPanel.Controls.Add(cbxLanguage, 0, 2);
        tableLayoutPanel.Controls.Add(chkNullableContext, 0, 3);
        tableLayoutPanel.Controls.Add(chkUseImplicitUsings, 0, 4);
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
        // txtFileHeader
        // 
        txtFileHeader.Dock = DockStyle.Top;
        txtFileHeader.Label = "File Header:";
        txtFileHeader.Name = "txtFileHeader";
        txtFileHeader.Size = new Size(374, 50);
        txtFileHeader.TabIndex = 1;
        // 
        // cbxLanguage
        // 
        cbxLanguage.Dock = DockStyle.Top;
        cbxLanguage.Label = "Language:";
        cbxLanguage.Name = "cbxLanguage";
        cbxLanguage.Size = new Size(374, 50);
        cbxLanguage.TabIndex = 2;
        // 
        // chkNullableContext
        // 
        chkNullableContext.Dock = DockStyle.Top;
        chkNullableContext.Label = "Nullable Context:";
        chkNullableContext.Name = "chkNullableContext";
        chkNullableContext.Size = new Size(374, 30);
        chkNullableContext.TabIndex = 3;
        // 
        // chkUseImplicitUsings
        // 
        chkUseImplicitUsings.Dock = DockStyle.Top;
        chkUseImplicitUsings.Label = "Use Implicit Usings:";
        chkUseImplicitUsings.Name = "chkUseImplicitUsings";
        chkUseImplicitUsings.Size = new Size(374, 30);
        chkUseImplicitUsings.TabIndex = 4;
        // 
        // CodeFileElementEditView
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoSize = true;
        Controls.Add(tableLayoutPanel);
        Name = "CodeFileElementEditView";
        Size = new Size(380, 480);
        tableLayoutPanel.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TableLayoutPanel tableLayoutPanel;
    private CodeElementEditView codeElementEditView;
    private SingleLineTextField txtFileHeader;
    private BooleanField chkNullableContext;
    private BooleanField chkUseImplicitUsings;
    private ComboboxField cbxLanguage;
}
