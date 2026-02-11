namespace CodeGenerator.Core.CodeElements.Views.Statements;

partial class CompositeStatementEditView
{
    private System.ComponentModel.IContainer components = null;

    #region Component Designer generated code

    private void InitializeComponent()
    {
        lblInfo = new Label();
        SuspendLayout();
        lblInfo.AutoSize = true;
        lblInfo.Location = new Point(10, 10);
        lblInfo.Name = "lblInfo";
        lblInfo.Size = new Size(250, 15);
        lblInfo.TabIndex = 0;
        lblInfo.Text = "Manage child statements via the tree.";
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoSize = true;
        Controls.Add(lblInfo);
        Name = "CompositeStatementEditView";
        Size = new Size(380, 35);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblInfo;
}
