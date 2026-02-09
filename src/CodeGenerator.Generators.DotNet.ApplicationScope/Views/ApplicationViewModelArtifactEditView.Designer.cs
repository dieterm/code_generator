using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Generators.DotNet.ApplicationScope.Views
{
    partial class ApplicationViewModelArtifactEditView
    {
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        private void InitializeComponent()
        {
            lblTitle = new Label();
            txtApplicationName = new SingleLineTextField();
            tableLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblTitle.Location = new Point(10, 10);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(234, 21);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Application ViewModel Properties";
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel.AutoSize = true;
            tableLayoutPanel.ColumnCount = 1;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.Controls.Add(txtApplicationName, 0, 0);
            tableLayoutPanel.Location = new Point(10, 40);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 1;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.Size = new Size(380, 60);
            tableLayoutPanel.TabIndex = 1;
            // 
            // txtApplicationName
            // 
            txtApplicationName.Dock = DockStyle.Top;
            txtApplicationName.Label = "Application Name:";
            txtApplicationName.Location = new Point(3, 3);
            txtApplicationName.Name = "txtApplicationName";
            txtApplicationName.Size = new Size(374, 50);
            txtApplicationName.TabIndex = 0;
            // 
            // ApplicationViewModelArtifactEditView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel);
            Controls.Add(lblTitle);
            Name = "ApplicationViewModelArtifactEditView";
            Padding = new Padding(10);
            Size = new Size(400, 120);
            tableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private TableLayoutPanel tableLayoutPanel;
        private SingleLineTextField txtApplicationName;
    }
}
