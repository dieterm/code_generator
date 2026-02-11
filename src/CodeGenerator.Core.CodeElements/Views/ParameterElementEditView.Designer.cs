using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Core.CodeElements.Views;

partial class ParameterElementEditView
{
    private System.ComponentModel.IContainer components = null;

    #region Component Designer generated code

    private void InitializeComponent()
    {
        codeElementEditView = new CodeElementEditView();
        txtTypeName = new SingleLineTextField();
        cbxModifier = new ComboboxField();
        txtDefaultValue = new SingleLineTextField();
        chkIsExtensionMethodThis = new BooleanField();
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
        tableLayoutPanel.Controls.Add(cbxModifier, 0, 2);
        tableLayoutPanel.Controls.Add(txtDefaultValue, 0, 3);
        tableLayoutPanel.Controls.Add(chkIsExtensionMethodThis, 0, 4);
        tableLayoutPanel.Location = new Point(0, 0);
        tableLayoutPanel.Name = "tableLayoutPanel";
        tableLayoutPanel.RowCount = 5;
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.Size = new Size(380, 500);
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
        // cbxModifier
        // 
        cbxModifier.Dock = DockStyle.Top;
        cbxModifier.ErrorMessageVisible = true;
        cbxModifier.Label = "Modifier:";
        cbxModifier.Name = "cbxModifier";
        cbxModifier.Size = new Size(374, 50);
        cbxModifier.TabIndex = 2;
        // 
        // txtDefaultValue
        // 
        txtDefaultValue.Dock = DockStyle.Top;
        txtDefaultValue.Label = "Default Value:";
        txtDefaultValue.Name = "txtDefaultValue";
        txtDefaultValue.Size = new Size(374, 50);
        txtDefaultValue.TabIndex = 3;
        // 
        // chkIsExtensionMethodThis
        // 
        chkIsExtensionMethodThis.Dock = DockStyle.Top;
        chkIsExtensionMethodThis.Label = "Is Extension Method This:";
        chkIsExtensionMethodThis.Name = "chkIsExtensionMethodThis";
        chkIsExtensionMethodThis.Size = new Size(374, 30);
        chkIsExtensionMethodThis.TabIndex = 4;
        // 
        // ParameterElementEditView
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoSize = true;
        Controls.Add(tableLayoutPanel);
        Name = "ParameterElementEditView";
        Size = new Size(380, 500);
        tableLayoutPanel.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TableLayoutPanel tableLayoutPanel;
    private CodeElementEditView codeElementEditView;
    private SingleLineTextField txtTypeName;
    private ComboboxField cbxModifier;
    private SingleLineTextField txtDefaultValue;
    private BooleanField chkIsExtensionMethodThis;
}
