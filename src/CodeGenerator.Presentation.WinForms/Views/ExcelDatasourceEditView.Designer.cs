using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Presentation.WinForms.Views
{
    partial class ExcelDatasourceEditView
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
            chkFirstRowIsHeader = new BooleanField();
            grpSheets = new GroupBox();
            lstSheets = new ListView();
            colName = new ColumnHeader();
            colColumns = new ColumnHeader();
            colRows = new ColumnHeader();
            panelSheetsButtons = new Panel();
            btnLoadSheets = new Button();
            btnAddSheet = new Button();
            btnAddAllSheets = new Button();
            lblStatus = new Label();
            lblError = new Label();
            grpFile.SuspendLayout();
            tableLayoutFile.SuspendLayout();
            grpSheets.SuspendLayout();
            panelSheetsButtons.SuspendLayout();
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
            lblTitle.Size = new Size(79, 31);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Excel File";
            // 
            // grpFile
            // 
            grpFile.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpFile.Controls.Add(tableLayoutFile);
            grpFile.Location = new Point(10, 44);
            grpFile.Name = "grpFile";
            grpFile.Size = new Size(380, 160);
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
            tableLayoutFile.Controls.Add(chkFirstRowIsHeader, 0, 2);
            tableLayoutFile.Location = new Point(6, 22);
            tableLayoutFile.Name = "tableLayoutFile";
            tableLayoutFile.RowCount = 3;
            tableLayoutFile.RowStyles.Add(new RowStyle());
            tableLayoutFile.RowStyles.Add(new RowStyle());
            tableLayoutFile.RowStyles.Add(new RowStyle());
            tableLayoutFile.Size = new Size(368, 130);
            tableLayoutFile.TabIndex = 0;
            // 
            // txtName
            // 
            txtName.Dock = DockStyle.Top;
            txtName.ErrorMessageVisible = true;
            txtName.Label = "Datasource Name:";
            txtName.Location = new Point(3, 3);
            txtName.Name = "txtName";
            txtName.Size = new Size(362, 36);
            txtName.TabIndex = 0;
            // 
            // fileField
            // 
            fileField.Dock = DockStyle.Top;
            fileField.ErrorMessageVisible = true;
            fileField.Label = "Excel File:";
            fileField.Location = new Point(3, 45);
            fileField.Name = "fileField";
            fileField.Size = new Size(362, 36);
            fileField.TabIndex = 1;
            // 
            // chkFirstRowIsHeader
            // 
            chkFirstRowIsHeader.Dock = DockStyle.Top;
            chkFirstRowIsHeader.Label = "First Row is Header";
            chkFirstRowIsHeader.Location = new Point(3, 87);
            chkFirstRowIsHeader.Name = "chkFirstRowIsHeader";
            chkFirstRowIsHeader.Size = new Size(362, 36);
            chkFirstRowIsHeader.TabIndex = 2;
            // 
            // grpSheets
            // 
            grpSheets.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpSheets.Controls.Add(lstSheets);
            grpSheets.Controls.Add(panelSheetsButtons);
            grpSheets.Controls.Add(lblStatus);
            grpSheets.Controls.Add(lblError);
            grpSheets.Location = new Point(10, 210);
            grpSheets.Name = "grpSheets";
            grpSheets.Size = new Size(380, 300);
            grpSheets.TabIndex = 2;
            grpSheets.TabStop = false;
            grpSheets.Text = "Available Sheets";
            // 
            // lstSheets
            // 
            lstSheets.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstSheets.Columns.AddRange(new ColumnHeader[] { colName, colColumns, colRows });
            lstSheets.FullRowSelect = true;
            lstSheets.GridLines = true;
            lstSheets.Location = new Point(6, 60);
            lstSheets.MultiSelect = false;
            lstSheets.Name = "lstSheets";
            lstSheets.Size = new Size(368, 200);
            lstSheets.TabIndex = 1;
            lstSheets.UseCompatibleStateImageBehavior = false;
            lstSheets.View = View.Details;
            // 
            // colName
            // 
            colName.Text = "Sheet Name";
            colName.Width = 180;
            // 
            // colColumns
            // 
            colColumns.Text = "Columns";
            colColumns.Width = 80;
            // 
            // colRows
            // 
            colRows.Text = "Rows";
            colRows.Width = 80;
            // 
            // panelSheetsButtons
            // 
            panelSheetsButtons.Controls.Add(btnLoadSheets);
            panelSheetsButtons.Controls.Add(btnAddSheet);
            panelSheetsButtons.Controls.Add(btnAddAllSheets);
            panelSheetsButtons.Dock = DockStyle.Top;
            panelSheetsButtons.Location = new Point(3, 19);
            panelSheetsButtons.Name = "panelSheetsButtons";
            panelSheetsButtons.Size = new Size(374, 35);
            panelSheetsButtons.TabIndex = 0;
            // 
            // btnLoadSheets
            // 
            btnLoadSheets.Location = new Point(3, 6);
            btnLoadSheets.Name = "btnLoadSheets";
            btnLoadSheets.Size = new Size(100, 26);
            btnLoadSheets.TabIndex = 0;
            btnLoadSheets.Text = "Load Sheets";
            btnLoadSheets.UseVisualStyleBackColor = true;
            // 
            // btnAddSheet
            // 
            btnAddSheet.Enabled = false;
            btnAddSheet.Location = new Point(109, 6);
            btnAddSheet.Name = "btnAddSheet";
            btnAddSheet.Size = new Size(100, 26);
            btnAddSheet.TabIndex = 1;
            btnAddSheet.Text = "Add Selected";
            btnAddSheet.UseVisualStyleBackColor = true;
            // 
            // btnAddAllSheets
            // 
            btnAddAllSheets.Enabled = false;
            btnAddAllSheets.Location = new Point(215, 6);
            btnAddAllSheets.Name = "btnAddAllSheets";
            btnAddAllSheets.Size = new Size(75, 26);
            btnAddAllSheets.TabIndex = 2;
            btnAddAllSheets.Text = "Add All";
            btnAddAllSheets.UseVisualStyleBackColor = true;
            // 
            // lblStatus
            // 
            lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblStatus.Location = new Point(6, 263);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(368, 15);
            lblStatus.TabIndex = 2;
            // 
            // lblError
            // 
            lblError.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblError.ForeColor = Color.Red;
            lblError.Location = new Point(6, 278);
            lblError.Name = "lblError";
            lblError.Size = new Size(368, 18);
            lblError.TabIndex = 3;
            lblError.Visible = false;
            // 
            // ExcelDatasourceEditView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(grpSheets);
            Controls.Add(grpFile);
            Controls.Add(lblTitle);
            Name = "ExcelDatasourceEditView";
            Padding = new Padding(10);
            Size = new Size(400, 520);
            grpFile.ResumeLayout(false);
            grpFile.PerformLayout();
            tableLayoutFile.ResumeLayout(false);
            grpSheets.ResumeLayout(false);
            panelSheetsButtons.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private GroupBox grpFile;
        private TableLayoutPanel tableLayoutFile;
        private SingleLineTextField txtName;
        private FileField fileField;
        private BooleanField chkFirstRowIsHeader;
        private GroupBox grpSheets;
        private Panel panelSheetsButtons;
        private Button btnLoadSheets;
        private Button btnAddSheet;
        private Button btnAddAllSheets;
        private ListView lstSheets;
        private ColumnHeader colName;
        private ColumnHeader colColumns;
        private ColumnHeader colRows;
        private Label lblStatus;
        private Label lblError;
    }
}
