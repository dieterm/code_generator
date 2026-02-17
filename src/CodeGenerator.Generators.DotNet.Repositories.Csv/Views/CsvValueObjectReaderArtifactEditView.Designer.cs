using CodeGenerator.Core.CodeElements.Views.EditFields;

namespace CodeGenerator.Generators.DotNet.Repositories.Csv.Views
{
    partial class CsvValueObjectReaderArtifactEditView
    {
        private System.ComponentModel.IContainer? components = null;

        #region Component Designer generated code

        private void InitializeComponent()
        {
            lblTitle = new Label();
            tableLayoutPanel = new TableLayoutPanel();
            codeFileField = new CodeFileElementField();
            tableLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblTitle.Location = new Point(10, 10);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(200, 21);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "CSV Repository Base";
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel.AutoSize = true;
            tableLayoutPanel.ColumnCount = 1;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.Controls.Add(codeFileField, 0, 0);
            tableLayoutPanel.Location = new Point(10, 40);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 1;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.Size = new Size(380, 60);
            tableLayoutPanel.TabIndex = 1;
            // 
            // codeFileField
            // 
            codeFileField.Dock = DockStyle.Top;
            codeFileField.Label = "Code File:";
            codeFileField.Location = new Point(3, 3);
            codeFileField.Name = "codeFileField";
            codeFileField.Size = new Size(374, 50);
            codeFileField.TabIndex = 0;
            // 
            // CsvRepositoryBaseArtifactEditView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel);
            Controls.Add(lblTitle);
            Name = "CsvRepositoryBaseArtifactEditView";
            Padding = new Padding(10);
            Size = new Size(400, 120);
            tableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private TableLayoutPanel tableLayoutPanel;
        private CodeFileElementField codeFileField;
    }
}
