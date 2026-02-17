using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Core.CodeElements.Views;

partial class GenericConstraintElementEditView
{
    private System.ComponentModel.IContainer? components = null;

    #region Component Designer generated code

    private void InitializeComponent()
    {
        codeElementEditView = new CodeElementEditView();
        txtTypeParameterName = new SingleLineTextField();
        msfConstraintKind = new MultiSelectField();
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
        tableLayoutPanel.Controls.Add(txtTypeParameterName, 0, 1);
        tableLayoutPanel.Controls.Add(msfConstraintKind, 0, 2);
        tableLayoutPanel.Location = new Point(0, 0);
        tableLayoutPanel.Name = "tableLayoutPanel";
        tableLayoutPanel.RowCount = 3;
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.Size = new Size(380, 470);
        tableLayoutPanel.TabIndex = 0;
        // 
        // codeElementEditView
        // 
        codeElementEditView.Dock = DockStyle.Top;
        codeElementEditView.Name = "codeElementEditView";
        codeElementEditView.Size = new Size(374, 320);
        codeElementEditView.TabIndex = 0;
        // 
        // txtTypeParameterName
        // 
        txtTypeParameterName.Dock = DockStyle.Top;
        txtTypeParameterName.Label = "Type Parameter Name:";
        txtTypeParameterName.Name = "txtTypeParameterName";
        txtTypeParameterName.Size = new Size(374, 50);
        txtTypeParameterName.TabIndex = 1;
        // 
        // msfConstraintKind
        // 
        msfConstraintKind.Dock = DockStyle.Top;
        msfConstraintKind.Label = "Constraint Kind:";
        msfConstraintKind.Name = "msfConstraintKind";
        msfConstraintKind.Size = new Size(374, 100);
        msfConstraintKind.TabIndex = 2;
        // 
        // GenericConstraintElementEditView
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoSize = true;
        Controls.Add(tableLayoutPanel);
        Name = "GenericConstraintElementEditView";
        Size = new Size(380, 470);
        tableLayoutPanel.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TableLayoutPanel tableLayoutPanel;
    private CodeElementEditView codeElementEditView;
    private SingleLineTextField txtTypeParameterName;
    private MultiSelectField msfConstraintKind;
}
