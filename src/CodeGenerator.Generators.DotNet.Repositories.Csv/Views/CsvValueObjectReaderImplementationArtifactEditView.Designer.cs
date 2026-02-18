using CodeGenerator.Core.CodeElements.Views.EditFields;
using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Generators.DotNet.Repositories.Csv.Views
{
    partial class CsvValueObjectReaderImplementationArtifactEditView
    {
        private System.ComponentModel.IContainer? components = null;

        #region Component Designer generated code

        private void InitializeComponent()
        {
            lblTitle = new Label();
            tableLayoutPanel = new TableLayoutPanel();
            txtName = new SingleLineTextField();
            txtDescription = new MultiLineTextField();
            cmbValueType = new ComboboxField();
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
            lblTitle.Text = "CSV Value Object Reader Implementation";
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel.AutoSize = true;
            tableLayoutPanel.ColumnCount = 1;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.Controls.Add(txtName, 0, 0);
            tableLayoutPanel.Controls.Add(txtDescription, 0, 1);
            tableLayoutPanel.Controls.Add(cmbValueType, 0, 2);
            tableLayoutPanel.Controls.Add(codeFileField, 0, 3);
            tableLayoutPanel.Location = new Point(10, 40);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 4;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.Size = new Size(380, 260);
            tableLayoutPanel.TabIndex = 1;
            // 
            // txtName
            // 
            txtName.Dock = DockStyle.Top;
            txtName.Label = "Name:";
            txtName.Name = "txtName";
            txtName.Size = new Size(374, 50);
            txtName.TabIndex = 0;
            // 
            // txtDescription
            // 
            txtDescription.Dock = DockStyle.Top;
            txtDescription.Label = "Description:";
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(374, 100);
            txtDescription.TabIndex = 1;
            // 
            // cmbValueType
            // 
            cmbValueType.Dock = DockStyle.Top;
            cmbValueType.Label = "Value Type:";
            cmbValueType.Name = "cmbValueType";
            cmbValueType.Size = new Size(374, 50);
            cmbValueType.TabIndex = 2;
            // 
            // codeFileField
            // 
            codeFileField.Dock = DockStyle.Top;
            codeFileField.Label = "Code File:";
            codeFileField.Name = "codeFileField";
            codeFileField.Size = new Size(374, 50);
            codeFileField.TabIndex = 3;
            // 
            // CsvValueObjectReaderImplementationArtifactEditView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel);
            Controls.Add(lblTitle);
            Name = "CsvValueObjectReaderImplementationArtifactEditView";
            Padding = new Padding(10);
            Size = new Size(400, 320);
            tableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private TableLayoutPanel tableLayoutPanel;
        private SingleLineTextField txtName;
        private MultiLineTextField txtDescription;
        private ComboboxField cmbValueType;
        private CodeFileElementField codeFileField;
    }
}
