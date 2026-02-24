using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Presentation.WinForms.Views
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
            grpDirectoryInfo = new GroupBox();
            panelDirectoryInfoButtons = new Panel();
            btnScan = new Button();
            btnImport = new Button();
            lblDirectoryInfo = new Label();
            lstSummary = new ListView();
            colProperty = new ColumnHeader();
            colValue = new ColumnHeader();
            lblStatus = new Label();
            lblError = new Label();
            grpDirectory.SuspendLayout();
            tableLayoutDirectory.SuspendLayout();
            grpDirectoryInfo.SuspendLayout();
            panelDirectoryInfoButtons.SuspendLayout();
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
            lblTitle.Size = new Size(74, 31);
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
            tableLayoutDirectory.Size = new Size(368, 130);
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
            // grpDirectoryInfo
            // 
            grpDirectoryInfo.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpDirectoryInfo.Controls.Add(lstSummary);
            grpDirectoryInfo.Controls.Add(panelDirectoryInfoButtons);
            grpDirectoryInfo.Controls.Add(lblDirectoryInfo);
            grpDirectoryInfo.Controls.Add(lblStatus);
            grpDirectoryInfo.Controls.Add(lblError);
            grpDirectoryInfo.Location = new Point(10, 210);
            grpDirectoryInfo.Name = "grpDirectoryInfo";
            grpDirectoryInfo.Size = new Size(380, 240);
            grpDirectoryInfo.TabIndex = 2;
            grpDirectoryInfo.TabStop = false;
            grpDirectoryInfo.Text = "Directory Summary";
            // 
            // panelDirectoryInfoButtons
            // 
            panelDirectoryInfoButtons.Controls.Add(btnScan);
            panelDirectoryInfoButtons.Controls.Add(btnImport);
            panelDirectoryInfoButtons.Dock = DockStyle.Top;
            panelDirectoryInfoButtons.Location = new Point(3, 19);
            panelDirectoryInfoButtons.Name = "panelDirectoryInfoButtons";
            panelDirectoryInfoButtons.Size = new Size(374, 35);
            panelDirectoryInfoButtons.TabIndex = 0;
            // 
            // btnScan
            // 
            btnScan.Location = new Point(3, 6);
            btnScan.Name = "btnScan";
            btnScan.Size = new Size(110, 26);
            btnScan.TabIndex = 0;
            btnScan.Text = "Scan Directory";
            btnScan.UseVisualStyleBackColor = true;
            // 
            // btnImport
            // 
            btnImport.Enabled = false;
            btnImport.Location = new Point(119, 6);
            btnImport.Name = "btnImport";
            btnImport.Size = new Size(100, 26);
            btnImport.TabIndex = 1;
            btnImport.Text = "Import Table";
            btnImport.UseVisualStyleBackColor = true;
            // 
            // lblDirectoryInfo
            // 
            lblDirectoryInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblDirectoryInfo.Location = new Point(6, 57);
            lblDirectoryInfo.Name = "lblDirectoryInfo";
            lblDirectoryInfo.Size = new Size(368, 20);
            lblDirectoryInfo.TabIndex = 1;
            // 
            // lstSummary
            // 
            lstSummary.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstSummary.Columns.AddRange(new ColumnHeader[] { colProperty, colValue });
            lstSummary.FullRowSelect = true;
            lstSummary.GridLines = true;
            lstSummary.Location = new Point(6, 80);
            lstSummary.MultiSelect = false;
            lstSummary.Name = "lstSummary";
            lstSummary.Size = new Size(368, 120);
            lstSummary.TabIndex = 2;
            lstSummary.UseCompatibleStateImageBehavior = false;
            lstSummary.View = View.Details;
            // 
            // colProperty
            // 
            colProperty.Text = "Property";
            colProperty.Width = 140;
            // 
            // colValue
            // 
            colValue.Text = "Value";
            colValue.Width = 200;
            // 
            // lblStatus
            // 
            lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblStatus.Location = new Point(6, 203);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(368, 15);
            lblStatus.TabIndex = 3;
            // 
            // lblError
            // 
            lblError.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblError.ForeColor = Color.Red;
            lblError.Location = new Point(6, 218);
            lblError.Name = "lblError";
            lblError.Size = new Size(368, 18);
            lblError.TabIndex = 4;
            lblError.Visible = false;
            // 
            // DirectoryDatasourceEditView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(grpDirectoryInfo);
            Controls.Add(grpDirectory);
            Controls.Add(lblTitle);
            Name = "DirectoryDatasourceEditView";
            Padding = new Padding(10);
            Size = new Size(400, 460);
            grpDirectory.ResumeLayout(false);
            grpDirectory.PerformLayout();
            tableLayoutDirectory.ResumeLayout(false);
            grpDirectoryInfo.ResumeLayout(false);
            panelDirectoryInfoButtons.ResumeLayout(false);
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
        private GroupBox grpDirectoryInfo;
        private Panel panelDirectoryInfoButtons;
        private Button btnScan;
        private Button btnImport;
        private Label lblDirectoryInfo;
        private ListView lstSummary;
        private ColumnHeader colProperty;
        private ColumnHeader colValue;
        private Label lblStatus;
        private Label lblError;
    }
}
