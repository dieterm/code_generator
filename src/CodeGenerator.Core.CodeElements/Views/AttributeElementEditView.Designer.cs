using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Core.CodeElements.Views;

partial class AttributeElementEditView
{
    private System.ComponentModel.IContainer components = null;

    #region Component Designer generated code

    private void InitializeComponent()
    {
        codeElementEditView = new CodeElementEditView();
        txtAttributeName = new SingleLineTextField();
        cbxTarget = new ComboboxField();
        lstArguments = new StringListField();
        lstNamedArguments = new StringDictionaryField();
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
        tableLayoutPanel.Controls.Add(txtAttributeName, 0, 1);
        tableLayoutPanel.Controls.Add(cbxTarget, 0, 2);
        tableLayoutPanel.Controls.Add(lstArguments, 0, 3);
        tableLayoutPanel.Controls.Add(lstNamedArguments, 0, 4);
        tableLayoutPanel.Location = new Point(0, 0);
        tableLayoutPanel.Name = "tableLayoutPanel";
        tableLayoutPanel.RowCount = 5;
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.Size = new Size(380, 780);
        tableLayoutPanel.TabIndex = 0;
        // 
        // codeElementEditView
        // 
        codeElementEditView.Dock = DockStyle.Top;
        codeElementEditView.Name = "codeElementEditView";
        codeElementEditView.Size = new Size(374, 320);
        codeElementEditView.TabIndex = 0;
        // 
        // txtAttributeName
        // 
        txtAttributeName.Dock = DockStyle.Top;
        txtAttributeName.ErrorMessageVisible = true;
        txtAttributeName.Label = "Attribute Name:";
        txtAttributeName.Name = "txtAttributeName";
        txtAttributeName.Size = new Size(374, 50);
        txtAttributeName.TabIndex = 1;
        // 
        // cbxTarget
        // 
        cbxTarget.Dock = DockStyle.Top;
        cbxTarget.ErrorMessageVisible = true;
        cbxTarget.Label = "Target:";
        cbxTarget.Name = "cbxTarget";
        cbxTarget.Size = new Size(374, 50);
        cbxTarget.TabIndex = 2;
        // 
        // lstArguments
        // 
        lstArguments.Dock = DockStyle.Top;
        lstArguments.Label = "Arguments";
        lstArguments.Name = "lstArguments";
        lstArguments.Size = new Size(374, 180);
        lstArguments.TabIndex = 3;
        // 
        // lstNamedArguments
        // 
        lstNamedArguments.Dock = DockStyle.Top;
        lstNamedArguments.Label = "Named Arguments";
        lstNamedArguments.Name = "lstNamedArguments";
        lstNamedArguments.Size = new Size(374, 180);
        lstNamedArguments.TabIndex = 4;
        // 
        // AttributeElementEditView
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoSize = true;
        Controls.Add(tableLayoutPanel);
        Name = "AttributeElementEditView";
        Size = new Size(380, 780);
        tableLayoutPanel.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TableLayoutPanel tableLayoutPanel;
    private CodeElementEditView codeElementEditView;
    private SingleLineTextField txtAttributeName;
    private ComboboxField cbxTarget;
    private StringListField lstArguments;
    private StringDictionaryField lstNamedArguments;
}
