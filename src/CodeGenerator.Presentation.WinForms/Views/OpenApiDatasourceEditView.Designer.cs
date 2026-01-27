using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Presentation.WinForms.Views
{
    partial class OpenApiDatasourceEditView
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
            grpSchemas = new GroupBox();
            splitContainer = new SplitContainer();
            lstSchemas = new ListView();
            colSchemaName = new ColumnHeader();
            colSchemaType = new ColumnHeader();
            colPropertyCount = new ColumnHeader();
            colDescription = new ColumnHeader();
            lstProperties = new ListView();
            colPropertyName = new ColumnHeader();
            colDataType = new ColumnHeader();
            colRequired = new ColumnHeader();
            panelSearch = new Panel();
            txtSearch = new TextBox();
            lblSearch = new Label();
            panelButtons = new Panel();
            btnAnalyze = new Button();
            btnImportSelected = new Button();
            btnImportAll = new Button();
            lblFileInfo = new Label();
            lblStatus = new Label();
            lblError = new Label();
            grpFile.SuspendLayout();
            tableLayoutFile.SuspendLayout();
            grpSchemas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
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
            lblTitle.Size = new Size(137, 31);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "OpenAPI/Swagger";
            // 
            // grpFile
            // 
            grpFile.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpFile.Controls.Add(tableLayoutFile);
            grpFile.Location = new Point(10, 44);
            grpFile.Name = "grpFile";
            grpFile.Size = new Size(480, 100);
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
            tableLayoutFile.Size = new Size(468, 72);
            tableLayoutFile.TabIndex = 0;
            // 
            // txtName
            // 
            txtName.Dock = DockStyle.Top;
            txtName.ErrorMessageVisible = true;
            txtName.Label = "Datasource Name:";
            txtName.Location = new Point(3, 3);
            txtName.Name = "txtName";
            txtName.Size = new Size(462, 30);
            txtName.TabIndex = 0;
            // 
            // fileField
            // 
            fileField.Dock = DockStyle.Top;
            fileField.ErrorMessageVisible = true;
            fileField.Label = "OpenAPI File:";
            fileField.Location = new Point(3, 39);
            fileField.Name = "fileField";
            fileField.Size = new Size(462, 30);
            fileField.TabIndex = 1;
            // 
            // grpSchemas
            // 
            grpSchemas.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpSchemas.Controls.Add(splitContainer);
            grpSchemas.Controls.Add(panelSearch);
            grpSchemas.Controls.Add(panelButtons);
            grpSchemas.Controls.Add(lblFileInfo);
            grpSchemas.Controls.Add(lblStatus);
            grpSchemas.Controls.Add(lblError);
            grpSchemas.Location = new Point(10, 150);
            grpSchemas.Name = "grpSchemas";
            grpSchemas.Size = new Size(480, 390);
            grpSchemas.TabIndex = 2;
            grpSchemas.TabStop = false;
            grpSchemas.Text = "Schemas";
            // 
            // splitContainer
            // 
            splitContainer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainer.Location = new Point(6, 110);
            splitContainer.Name = "splitContainer";
            splitContainer.Orientation = Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.Controls.Add(lstSchemas);
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.Controls.Add(lstProperties);
            splitContainer.Size = new Size(468, 240);
            splitContainer.SplitterDistance = 120;
            splitContainer.TabIndex = 3;
            // 
            // lstSchemas
            // 
            lstSchemas.Columns.AddRange(new ColumnHeader[] { colSchemaName, colSchemaType, colPropertyCount, colDescription });
            lstSchemas.Dock = DockStyle.Fill;
            lstSchemas.FullRowSelect = true;
            lstSchemas.GridLines = true;
            lstSchemas.Location = new Point(0, 0);
            lstSchemas.MultiSelect = false;
            lstSchemas.Name = "lstSchemas";
            lstSchemas.Size = new Size(468, 120);
            lstSchemas.TabIndex = 0;
            lstSchemas.UseCompatibleStateImageBehavior = false;
            lstSchemas.View = View.Details;
            // 
            // colSchemaName
            // 
            colSchemaName.Text = "Schema Name";
            colSchemaName.Width = 150;
            // 
            // colSchemaType
            // 
            colSchemaType.Text = "Type";
            colSchemaType.Width = 70;
            // 
            // colPropertyCount
            // 
            colPropertyCount.Text = "Properties";
            colPropertyCount.Width = 70;
            // 
            // colDescription
            // 
            colDescription.Text = "Description";
            colDescription.Width = 150;
            // 
            // lstProperties
            // 
            lstProperties.Columns.AddRange(new ColumnHeader[] { colPropertyName, colDataType, colRequired });
            lstProperties.Dock = DockStyle.Fill;
            lstProperties.FullRowSelect = true;
            lstProperties.GridLines = true;
            lstProperties.Location = new Point(0, 0);
            lstProperties.MultiSelect = false;
            lstProperties.Name = "lstProperties";
            lstProperties.Size = new Size(468, 116);
            lstProperties.TabIndex = 0;
            lstProperties.UseCompatibleStateImageBehavior = false;
            lstProperties.View = View.Details;
            // 
            // colPropertyName
            // 
            colPropertyName.Text = "Property Name";
            colPropertyName.Width = 150;
            // 
            // colDataType
            // 
            colDataType.Text = "Data Type";
            colDataType.Width = 150;
            // 
            // colRequired
            // 
            colRequired.Text = "Required";
            colRequired.Width = 70;
            // 
            // panelSearch
            // 
            panelSearch.Controls.Add(txtSearch);
            panelSearch.Controls.Add(lblSearch);
            panelSearch.Dock = DockStyle.Top;
            panelSearch.Location = new Point(3, 54);
            panelSearch.Name = "panelSearch";
            panelSearch.Size = new Size(474, 30);
            panelSearch.TabIndex = 1;
            // 
            // txtSearch
            // 
            txtSearch.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSearch.Location = new Point(50, 4);
            txtSearch.Name = "txtSearch";
            txtSearch.PlaceholderText = "Filter by name...";
            txtSearch.Size = new Size(421, 23);
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
            panelButtons.Size = new Size(474, 35);
            panelButtons.TabIndex = 0;
            // 
            // btnAnalyze
            // 
            btnAnalyze.Location = new Point(3, 6);
            btnAnalyze.Name = "btnAnalyze";
            btnAnalyze.Size = new Size(100, 26);
            btnAnalyze.TabIndex = 0;
            btnAnalyze.Text = "Analyze File";
            btnAnalyze.UseVisualStyleBackColor = true;
            // 
            // btnImportSelected
            // 
            btnImportSelected.Enabled = false;
            btnImportSelected.Location = new Point(109, 6);
            btnImportSelected.Name = "btnImportSelected";
            btnImportSelected.Size = new Size(110, 26);
            btnImportSelected.TabIndex = 1;
            btnImportSelected.Text = "Import Selected";
            btnImportSelected.UseVisualStyleBackColor = true;
            // 
            // btnImportAll
            // 
            btnImportAll.Enabled = false;
            btnImportAll.Location = new Point(225, 6);
            btnImportAll.Name = "btnImportAll";
            btnImportAll.Size = new Size(90, 26);
            btnImportAll.TabIndex = 2;
            btnImportAll.Text = "Import All";
            btnImportAll.UseVisualStyleBackColor = true;
            // 
            // lblFileInfo
            // 
            lblFileInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblFileInfo.Location = new Point(6, 87);
            lblFileInfo.Name = "lblFileInfo";
            lblFileInfo.Size = new Size(468, 20);
            lblFileInfo.TabIndex = 2;
            // 
            // lblStatus
            // 
            lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblStatus.Location = new Point(6, 353);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(468, 15);
            lblStatus.TabIndex = 4;
            // 
            // lblError
            // 
            lblError.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblError.ForeColor = Color.Red;
            lblError.Location = new Point(6, 368);
            lblError.Name = "lblError";
            lblError.Size = new Size(468, 18);
            lblError.TabIndex = 5;
            lblError.Visible = false;
            // 
            // OpenApiDatasourceEditView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(grpSchemas);
            Controls.Add(grpFile);
            Controls.Add(lblTitle);
            Name = "OpenApiDatasourceEditView";
            Padding = new Padding(10);
            Size = new Size(500, 550);
            grpFile.ResumeLayout(false);
            grpFile.PerformLayout();
            tableLayoutFile.ResumeLayout(false);
            grpSchemas.ResumeLayout(false);
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
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
        private GroupBox grpSchemas;
        private SplitContainer splitContainer;
        private ListView lstSchemas;
        private ColumnHeader colSchemaName;
        private ColumnHeader colSchemaType;
        private ColumnHeader colPropertyCount;
        private ColumnHeader colDescription;
        private ListView lstProperties;
        private ColumnHeader colPropertyName;
        private ColumnHeader colDataType;
        private ColumnHeader colRequired;
        private Panel panelSearch;
        private TextBox txtSearch;
        private Label lblSearch;
        private Panel panelButtons;
        private Button btnAnalyze;
        private Button btnImportSelected;
        private Button btnImportAll;
        private Label lblFileInfo;
        private Label lblStatus;
        private Label lblError;
    }
}
