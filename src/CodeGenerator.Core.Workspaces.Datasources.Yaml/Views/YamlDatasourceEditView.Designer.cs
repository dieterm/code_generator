using CodeGenerator.Presentation.WinForms.Views;
using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Core.Workspaces.Datasources.Yaml.Views
{
    partial class YamlDatasourceEditView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblTitle = new Label();
            grpFile = new GroupBox();
            tableLayoutFile = new TableLayoutPanel();
            txtName = new SingleLineTextField();
            fileField = new FileField();
            objectImportField = new DatasourceObjectImportField();
            grpFile.SuspendLayout();
            tableLayoutFile.SuspendLayout();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTitle.Location = new Point(10, 10);
            lblTitle.Name = "lblTitle";
            lblTitle.Padding = new Padding(0, 0, 0, 10);
            lblTitle.Size = new Size(84, 31);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "YAML File";
            // 
            // grpFile
            // 
            grpFile.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpFile.Controls.Add(tableLayoutFile);
            grpFile.Location = new Point(10, 44);
            grpFile.Name = "grpFile";
            grpFile.Size = new Size(380, 100);
            grpFile.TabIndex = 1;
            grpFile.TabStop = false;
            grpFile.Text = "File Settings";
            // 
            // tableLayoutFile
            // 
            tableLayoutFile.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutFile.AutoSize = true;
            tableLayoutFile.ColumnCount = 1;
            tableLayoutFile.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutFile.Controls.Add(txtName, 0, 0);
            tableLayoutFile.Controls.Add(fileField, 0, 1);
            tableLayoutFile.Location = new Point(6, 22);
            tableLayoutFile.Name = "tableLayoutFile";
            tableLayoutFile.RowCount = 2;
            tableLayoutFile.RowStyles.Add(new RowStyle());
            tableLayoutFile.RowStyles.Add(new RowStyle());
            tableLayoutFile.Size = new Size(368, 72);
            tableLayoutFile.TabIndex = 0;
            // 
            // txtName
            // 
            txtName.Dock = DockStyle.Top;
            txtName.ErrorMessageVisible = true;
            txtName.Label = "Datasource Name:";
            txtName.Location = new Point(3, 3);
            txtName.Name = "txtName";
            txtName.Size = new Size(362, 30);
            txtName.TabIndex = 0;
            // 
            // fileField
            // 
            fileField.Dock = DockStyle.Top;
            fileField.ErrorMessageVisible = true;
            fileField.Label = "YAML File:";
            fileField.Location = new Point(3, 39);
            fileField.Name = "fileField";
            fileField.Size = new Size(362, 30);
            fileField.TabIndex = 1;
            // 
            // objectImportField
            // 
            objectImportField.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            objectImportField.Location = new Point(10, 150);
            objectImportField.Name = "objectImportField";
            objectImportField.Size = new Size(380, 300);
            objectImportField.TabIndex = 2;
            // 
            // YamlDatasourceEditView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(objectImportField);
            Controls.Add(grpFile);
            Controls.Add(lblTitle);
            Name = "YamlDatasourceEditView";
            Padding = new Padding(10);
            Size = new Size(400, 460);
            grpFile.ResumeLayout(false);
            grpFile.PerformLayout();
            tableLayoutFile.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private GroupBox grpFile;
        private TableLayoutPanel tableLayoutFile;
        private SingleLineTextField txtName;
        private FileField fileField;
        private DatasourceObjectImportField objectImportField;
    }
}
