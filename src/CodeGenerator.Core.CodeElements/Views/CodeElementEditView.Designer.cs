using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Core.CodeElements.Views;

partial class CodeElementEditView
{
    private System.ComponentModel.IContainer components = null;

    #region Component Designer generated code

    private void InitializeComponent()
    {
        txtName = new SingleLineTextField();
        cbxAccessModifier = new ComboboxField();
        msfModifiers = new MultiSelectField();
        txtDocumentation = new SingleLineTextField();
        txtRawCode = new SingleLineTextField();
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
        tableLayoutPanel.Controls.Add(txtName, 0, 0);
        tableLayoutPanel.Controls.Add(cbxAccessModifier, 0, 1);
        tableLayoutPanel.Controls.Add(msfModifiers, 0, 2);
        tableLayoutPanel.Controls.Add(txtDocumentation, 0, 3);
        tableLayoutPanel.Controls.Add(txtRawCode, 0, 4);
        tableLayoutPanel.Location = new Point(0, 0);
        tableLayoutPanel.Name = "tableLayoutPanel";
        tableLayoutPanel.RowCount = 5;
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.Size = new Size(380, 320);
        tableLayoutPanel.TabIndex = 0;
        // 
        // txtName
        // 
        txtName.Dock = DockStyle.Top;
        txtName.Label = "Name:";
        txtName.Name = "txtName";
        txtName.Size = new Size(374, 50);
        txtName.TabIndex = 0;
        // 
        // cbxAccessModifier
        // 
        cbxAccessModifier.Dock = DockStyle.Top;
        cbxAccessModifier.Label = "Access Modifier:";
        cbxAccessModifier.Name = "cbxAccessModifier";
        cbxAccessModifier.Size = new Size(374, 50);
        cbxAccessModifier.TabIndex = 1;
        // 
        // msfModifiers
        // 
        msfModifiers.Dock = DockStyle.Top;
        msfModifiers.Label = "Modifiers:";
        msfModifiers.Name = "msfModifiers";
        msfModifiers.Size = new Size(374, 115);
        msfModifiers.TabIndex = 2;
        // 
        // txtDocumentation
        // 
        txtDocumentation.Dock = DockStyle.Top;
        txtDocumentation.Label = "Documentation:";
        txtDocumentation.Name = "txtDocumentation";
        txtDocumentation.Size = new Size(374, 50);
        txtDocumentation.TabIndex = 3;
        // 
        // txtRawCode
        // 
        txtRawCode.Dock = DockStyle.Top;
        txtRawCode.Label = "Raw Code:";
        txtRawCode.Name = "txtRawCode";
        txtRawCode.Size = new Size(374, 50);
        txtRawCode.TabIndex = 4;
        // 
        // CodeElementEditView
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoSize = true;
        Controls.Add(tableLayoutPanel);
        Name = "CodeElementEditView";
        Size = new Size(380, 320);
        tableLayoutPanel.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TableLayoutPanel tableLayoutPanel;
    private SingleLineTextField txtName;
    private ComboboxField cbxAccessModifier;
    private MultiSelectField msfModifiers;
    private SingleLineTextField txtDocumentation;
    private SingleLineTextField txtRawCode;
}
