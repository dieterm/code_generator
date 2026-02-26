using CodeGenerator.Presentation.WinForms.Views;
using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Core.Workspaces.Datasources.Directory.Views
{
    partial class DirectoryDatasourceEditView
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
            grpDirectory = new GroupBox();
            tableLayoutDirectory = new TableLayoutPanel();
            txtName = new SingleLineTextField();
            folderField = new FolderField();
            txtSearchPattern = new SingleLineTextField();
            chkIncludeSubdirectories = new CheckboxField();
            objectImportField = new DatasourceObjectImportField();
            grpDirectory.SuspendLayout();
            tableLayoutDirectory.SuspendLayout();
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
            lblTitle.Size = new Size(82, 31);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Directory";
            // 
            // grpDirectory
            // 
            grpDirectory.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpDirectory.Controls.Add(tableLayoutDirectory);
            grpDirectory.Location = new Point(10, 44);
            grpDirectory.Name = "grpDirectory";
            grpDirectory.Size = new Size(380, 160);
            grpDirectory.TabIndex = 1;
            grpDirectory.TabStop = false;
            grpDirectory.Text = "Directory Settings";
            // 
            // tableLayoutDirectory
            // 
            tableLayoutDirectory.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutDirectory.AutoSize = true;
            tableLayoutDirectory.ColumnCount = 1;
            tableLayoutDirectory.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutDirectory.Controls.Add(txtName, 0, 0);
            tableLayoutDirectory.Controls.Add(folderField, 0, 1);
            tableLayoutDirectory.Controls.Add(txtSearchPattern, 0, 2);
            tableLayoutDirectory.Controls.Add(chkIncludeSubdirectories, 0, 3);
            tableLayoutDirectory.Location = new Point(6, 22);
            tableLayoutDirectory.Name = "tableLayoutDirectory";
            tableLayoutDirectory.RowCount = 4;
            tableLayoutDirectory.RowStyles.Add(new RowStyle());
            tableLayoutDirectory.RowStyles.Add(new RowStyle());
            tableLayoutDirectory.RowStyles.Add(new RowStyle());
            tableLayoutDirectory.RowStyles.Add(new RowStyle());
            tableLayoutDirectory.Size = new Size(368, 139);
            tableLayoutDirectory.TabIndex = 0;
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
            // folderField
            // 
            folderField.Dock = DockStyle.Top;
            folderField.ErrorMessageVisible = true;
            folderField.Label = "Directory:";
            folderField.Location = new Point(3, 39);
            folderField.Name = "folderField";
            folderField.Size = new Size(362, 30);
            folderField.TabIndex = 1;
            // 
            // txtSearchPattern
            // 
            txtSearchPattern.Dock = DockStyle.Top;
            txtSearchPattern.ErrorMessageVisible = true;
            txtSearchPattern.Label = "Search Pattern:";
            txtSearchPattern.Location = new Point(3, 75);
            txtSearchPattern.Name = "txtSearchPattern";
            txtSearchPattern.Size = new Size(362, 30);
            txtSearchPattern.TabIndex = 2;
            // 
            // chkIncludeSubdirectories
            // 
            chkIncludeSubdirectories.Dock = DockStyle.Top;
            chkIncludeSubdirectories.Label = "Include Subdirectories";
            chkIncludeSubdirectories.Location = new Point(3, 111);
            chkIncludeSubdirectories.Name = "chkIncludeSubdirectories";
            chkIncludeSubdirectories.Size = new Size(362, 25);
            chkIncludeSubdirectories.TabIndex = 3;
            // 
            // objectImportField
            // 
            objectImportField.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            objectImportField.Location = new Point(10, 210);
            objectImportField.Name = "objectImportField";
            objectImportField.Size = new Size(380, 240);
            objectImportField.TabIndex = 2;
            // 
            // DirectoryDatasourceEditView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(objectImportField);
            Controls.Add(grpDirectory);
            Controls.Add(lblTitle);
            Name = "DirectoryDatasourceEditView";
            Padding = new Padding(10);
            Size = new Size(400, 460);
            grpDirectory.ResumeLayout(false);
            grpDirectory.PerformLayout();
            tableLayoutDirectory.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private GroupBox grpDirectory;
        private TableLayoutPanel tableLayoutDirectory;
        private SingleLineTextField txtName;
        private FolderField folderField;
        private SingleLineTextField txtSearchPattern;
        private CheckboxField chkIncludeSubdirectories;
        private DatasourceObjectImportField objectImportField;
    }
}
