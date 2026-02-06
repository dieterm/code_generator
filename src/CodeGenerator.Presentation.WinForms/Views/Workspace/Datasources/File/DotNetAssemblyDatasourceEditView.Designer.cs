using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Presentation.WinForms.Views
{
    partial class DotNetAssemblyDatasourceEditView
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
            grpTypes = new GroupBox();
            panelSearch = new Panel();
            txtSearch = new TextBox();
            lblSearch = new Label();
            panelButtons = new Panel();
            btnAnalyze = new Button();
            btnImportSelected = new Button();
            btnImportAll = new Button();
            lblAssemblyInfo = new Label();
            treeTypes = new TreeView();
            lblStatus = new Label();
            lblError = new Label();
            grpFile.SuspendLayout();
            tableLayoutFile.SuspendLayout();
            grpTypes.SuspendLayout();
            panelSearch.SuspendLayout();
            panelButtons.SuspendLayout();
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
            lblTitle.Size = new Size(120, 31);
            lblTitle.TabIndex = 0;
            lblTitle.Text = ".NET Assembly";
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
            fileField.Label = "Assembly File:";
            fileField.Location = new Point(3, 39);
            fileField.Name = "fileField";
            fileField.Size = new Size(362, 30);
            fileField.TabIndex = 1;
            // 
            // grpTypes
            // 
            grpTypes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpTypes.Controls.Add(treeTypes);
            grpTypes.Controls.Add(panelSearch);
            grpTypes.Controls.Add(panelButtons);
            grpTypes.Controls.Add(lblAssemblyInfo);
            grpTypes.Controls.Add(lblStatus);
            grpTypes.Controls.Add(lblError);
            grpTypes.Location = new Point(10, 150);
            grpTypes.Name = "grpTypes";
            grpTypes.Size = new Size(380, 340);
            grpTypes.TabIndex = 2;
            grpTypes.TabStop = false;
            grpTypes.Text = "Types";
            // 
            // panelSearch
            // 
            panelSearch.Controls.Add(txtSearch);
            panelSearch.Controls.Add(lblSearch);
            panelSearch.Dock = DockStyle.Top;
            panelSearch.Location = new Point(3, 54);
            panelSearch.Name = "panelSearch";
            panelSearch.Size = new Size(374, 30);
            panelSearch.TabIndex = 1;
            // 
            // txtSearch
            // 
            txtSearch.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSearch.Location = new Point(50, 4);
            txtSearch.Name = "txtSearch";
            txtSearch.PlaceholderText = "Filter by name or namespace...";
            txtSearch.Size = new Size(321, 23);
            txtSearch.TabIndex = 1;
            // 
            // lblSearch
            // 
            lblSearch.AutoSize = true;
            lblSearch.Location = new Point(3, 7);
            lblSearch.Name = "lblSearch";
            lblSearch.Size = new Size(45, 15);
            lblSearch.TabIndex = 0;
            lblSearch.Text = "Search:";
            // 
            // panelButtons
            // 
            panelButtons.Controls.Add(btnAnalyze);
            panelButtons.Controls.Add(btnImportSelected);
            panelButtons.Controls.Add(btnImportAll);
            panelButtons.Dock = DockStyle.Top;
            panelButtons.Location = new Point(3, 19);
            panelButtons.Name = "panelButtons";
            panelButtons.Size = new Size(374, 35);
            panelButtons.TabIndex = 0;
            // 
            // btnAnalyze
            // 
            btnAnalyze.Location = new Point(3, 6);
            btnAnalyze.Name = "btnAnalyze";
            btnAnalyze.Size = new Size(120, 26);
            btnAnalyze.TabIndex = 0;
            btnAnalyze.Text = "Analyze Assembly";
            btnAnalyze.UseVisualStyleBackColor = true;
            // 
            // btnImportSelected
            // 
            btnImportSelected.Enabled = false;
            btnImportSelected.Location = new Point(129, 6);
            btnImportSelected.Name = "btnImportSelected";
            btnImportSelected.Size = new Size(110, 26);
            btnImportSelected.TabIndex = 1;
            btnImportSelected.Text = "Import Selected";
            btnImportSelected.UseVisualStyleBackColor = true;
            // 
            // btnImportAll
            // 
            btnImportAll.Enabled = false;
            btnImportAll.Location = new Point(245, 6);
            btnImportAll.Name = "btnImportAll";
            btnImportAll.Size = new Size(90, 26);
            btnImportAll.TabIndex = 2;
            btnImportAll.Text = "Import All";
            btnImportAll.UseVisualStyleBackColor = true;
            // 
            // lblAssemblyInfo
            // 
            lblAssemblyInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblAssemblyInfo.Location = new Point(6, 87);
            lblAssemblyInfo.Name = "lblAssemblyInfo";
            lblAssemblyInfo.Size = new Size(368, 20);
            lblAssemblyInfo.TabIndex = 2;
            // 
            // treeTypes
            // 
            treeTypes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            treeTypes.Location = new Point(6, 110);
            treeTypes.Name = "treeTypes";
            treeTypes.Size = new Size(368, 190);
            treeTypes.TabIndex = 3;
            // 
            // lblStatus
            // 
            lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblStatus.Location = new Point(6, 303);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(368, 15);
            lblStatus.TabIndex = 4;
            // 
            // lblError
            // 
            lblError.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblError.ForeColor = Color.Red;
            lblError.Location = new Point(6, 318);
            lblError.Name = "lblError";
            lblError.Size = new Size(368, 18);
            lblError.TabIndex = 5;
            lblError.Visible = false;
            // 
            // DotNetAssemblyDatasourceEditView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(grpTypes);
            Controls.Add(grpFile);
            Controls.Add(lblTitle);
            Name = "DotNetAssemblyDatasourceEditView";
            Padding = new Padding(10);
            Size = new Size(400, 500);
            grpFile.ResumeLayout(false);
            grpFile.PerformLayout();
            tableLayoutFile.ResumeLayout(false);
            grpTypes.ResumeLayout(false);
            panelSearch.ResumeLayout(false);
            panelSearch.PerformLayout();
            panelButtons.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private GroupBox grpFile;
        private TableLayoutPanel tableLayoutFile;
        private SingleLineTextField txtName;
        private FileField fileField;
        private GroupBox grpTypes;
        private Panel panelSearch;
        private TextBox txtSearch;
        private Label lblSearch;
        private Panel panelButtons;
        private Button btnAnalyze;
        private Button btnImportSelected;
        private Button btnImportAll;
        private Label lblAssemblyInfo;
        private TreeView treeTypes;
        private Label lblStatus;
        private Label lblError;
    }
}
