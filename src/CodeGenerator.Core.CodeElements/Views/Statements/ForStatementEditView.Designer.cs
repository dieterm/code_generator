using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Core.CodeElements.Views.Statements;

partial class ForStatementEditView
{
    private System.ComponentModel.IContainer components = null;

    #region Component Designer generated code

    private void InitializeComponent()
    {
        txtInitializer = new SingleLineTextField();
        txtCondition = new SingleLineTextField();
        txtIncrementer = new SingleLineTextField();
        tableLayoutPanel = new TableLayoutPanel();
        tableLayoutPanel.SuspendLayout();
        SuspendLayout();
        tableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        tableLayoutPanel.AutoSize = true;
        tableLayoutPanel.ColumnCount = 1;
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanel.Controls.Add(txtInitializer, 0, 0);
        tableLayoutPanel.Controls.Add(txtCondition, 0, 1);
        tableLayoutPanel.Controls.Add(txtIncrementer, 0, 2);
        tableLayoutPanel.Location = new Point(0, 0);
        tableLayoutPanel.Name = "tableLayoutPanel";
        tableLayoutPanel.RowCount = 3;
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.RowStyles.Add(new RowStyle());
        tableLayoutPanel.Size = new Size(380, 150);
        tableLayoutPanel.TabIndex = 0;
        txtInitializer.Dock = DockStyle.Top; txtInitializer.Label = "Initializer:"; txtInitializer.Name = "txtInitializer"; txtInitializer.Size = new Size(374, 50); txtInitializer.TabIndex = 0;
        txtCondition.Dock = DockStyle.Top; txtCondition.Label = "Condition:"; txtCondition.Name = "txtCondition"; txtCondition.Size = new Size(374, 50); txtCondition.TabIndex = 1;
        txtIncrementer.Dock = DockStyle.Top; txtIncrementer.Label = "Incrementer:"; txtIncrementer.Name = "txtIncrementer"; txtIncrementer.Size = new Size(374, 50); txtIncrementer.TabIndex = 2;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoSize = true;
        Controls.Add(tableLayoutPanel);
        Name = "ForStatementEditView";
        Size = new Size(380, 150);
        tableLayoutPanel.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TableLayoutPanel tableLayoutPanel;
    private SingleLineTextField txtInitializer;
    private SingleLineTextField txtCondition;
    private SingleLineTextField txtIncrementer;
}
