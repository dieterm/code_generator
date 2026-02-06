using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Presentation.WinForms.Views
{
    partial class CsvDatasourceEditView
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
            txtFieldDelimiter = new SingleLineTextField();
            txtRowTerminator = new SingleLineTextField();
            grpFileInfo = new GroupBox();
            panelFileInfoButtons = new Panel();
            btnAnalyze = new Button();
            btnImport = new Button();
            lblFileInfo = new Label();
            lstColumns = new ListView();
            colColumnName = new ColumnHeader();
            colDataType = new ColumnHeader();
            lblStatus = new Label();
            lblError = new Label();
            grpFile.SuspendLayout();
            tableLayoutFile.SuspendLayout();
            grpFileInfo.SuspendLayout();
            panelFileInfoButtons.SuspendLayout();
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
            lblTitle.Size = new Size(68, 31);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "CSV File";
            // 
            // grpFile
            // 
            grpFile.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpFile.Controls.Add(tableLayoutFile);
            grpFile.Location = new Point(10, 44);
            grpFile.Name = "grpFile";
            grpFile.Size = new Size(380, 220);
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
            tableLayoutFile.Controls.Add(txtFieldDelimiter, 0, 3);
            tableLayoutFile.Controls.Add(txtRowTerminator, 0, 4);
            tableLayoutFile.Location = new Point(6, 22);
            tableLayoutFile.Name = "tableLayoutFile";
            tableLayoutFile.RowCount = 5;
            tableLayoutFile.RowStyles.Add(new RowStyle());
            tableLayoutFile.RowStyles.Add(new RowStyle());
            tableLayoutFile.RowStyles.Add(new RowStyle());
            tableLayoutFile.RowStyles.Add(new RowStyle());
            tableLayoutFile.RowStyles.Add(new RowStyle());
            tableLayoutFile.Size = new Size(368, 190);
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
            fileField.Label = "CSV File:";
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
            chkFirstRowIsHeader.Size = new Size(362, 30);
            chkFirstRowIsHeader.TabIndex = 2;
            // 
            // txtFieldDelimiter
            // 
            txtFieldDelimiter.Dock = DockStyle.Top;
            txtFieldDelimiter.ErrorMessageVisible = false;
            txtFieldDelimiter.Label = "Field Delimiter:";
            txtFieldDelimiter.Location = new Point(3, 123);
            txtFieldDelimiter.Name = "txtFieldDelimiter";
            txtFieldDelimiter.Size = new Size(362, 30);
            txtFieldDelimiter.TabIndex = 3;
            // 
            // txtRowTerminator
            // 
            txtRowTerminator.Dock = DockStyle.Top;
            txtRowTerminator.ErrorMessageVisible = false;
            txtRowTerminator.Label = "Row Terminator:";
            txtRowTerminator.Location = new Point(3, 159);
            txtRowTerminator.Name = "txtRowTerminator";
            txtRowTerminator.Size = new Size(362, 30);
            txtRowTerminator.TabIndex = 4;
            // 
            // grpFileInfo
            // 
            grpFileInfo.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpFileInfo.Controls.Add(lstColumns);
            grpFileInfo.Controls.Add(panelFileInfoButtons);
            grpFileInfo.Controls.Add(lblFileInfo);
            grpFileInfo.Controls.Add(lblStatus);
            grpFileInfo.Controls.Add(lblError);
            grpFileInfo.Location = new Point(10, 270);
            grpFileInfo.Name = "grpFileInfo";
            grpFileInfo.Size = new Size(380, 250);
            grpFileInfo.TabIndex = 2;
            grpFileInfo.TabStop = false;
            grpFileInfo.Text = "File Structure";
            // 
            // panelFileInfoButtons
            // 
            panelFileInfoButtons.Controls.Add(btnAnalyze);
            panelFileInfoButtons.Controls.Add(btnImport);
            panelFileInfoButtons.Dock = DockStyle.Top;
            panelFileInfoButtons.Location = new Point(3, 19);
            panelFileInfoButtons.Name = "panelFileInfoButtons";
            panelFileInfoButtons.Size = new Size(374, 35);
            panelFileInfoButtons.TabIndex = 0;
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
            // btnImport
            // 
            btnImport.Enabled = false;
            btnImport.Location = new Point(109, 6);
            btnImport.Name = "btnImport";
            btnImport.Size = new Size(100, 26);
            btnImport.TabIndex = 1;
            btnImport.Text = "Import Table";
            btnImport.UseVisualStyleBackColor = true;
            // 
            // lblFileInfo
            // 
            lblFileInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblFileInfo.Location = new Point(6, 57);
            lblFileInfo.Name = "lblFileInfo";
            lblFileInfo.Size = new Size(368, 20);
            lblFileInfo.TabIndex = 1;
            // 
            // lstColumns
            // 
            lstColumns.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstColumns.Columns.AddRange(new ColumnHeader[] { colColumnName, colDataType });
            lstColumns.FullRowSelect = true;
            lstColumns.GridLines = true;
            lstColumns.Location = new Point(6, 80);
            lstColumns.MultiSelect = false;
            lstColumns.Name = "lstColumns";
            lstColumns.Size = new Size(368, 130);
            lstColumns.TabIndex = 2;
            lstColumns.UseCompatibleStateImageBehavior = false;
            lstColumns.View = View.Details;
            // 
            // colColumnName
            // 
            colColumnName.Text = "Column Name";
            colColumnName.Width = 200;
            // 
            // colDataType
            // 
            colDataType.Text = "Inferred Type";
            colDataType.Width = 120;
            // 
            // lblStatus
            // 
            lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblStatus.Location = new Point(6, 213);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(368, 15);
            lblStatus.TabIndex = 3;
            // 
            // lblError
            // 
            lblError.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblError.ForeColor = Color.Red;
            lblError.Location = new Point(6, 228);
            lblError.Name = "lblError";
            lblError.Size = new Size(368, 18);
            lblError.TabIndex = 4;
            lblError.Visible = false;
            // 
            // CsvDatasourceEditView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(grpFileInfo);
            Controls.Add(grpFile);
            Controls.Add(lblTitle);
            Name = "CsvDatasourceEditView";
            Padding = new Padding(10);
            Size = new Size(400, 530);
            grpFile.ResumeLayout(false);
            grpFile.PerformLayout();
            tableLayoutFile.ResumeLayout(false);
            grpFileInfo.ResumeLayout(false);
            panelFileInfoButtons.ResumeLayout(false);
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
        private SingleLineTextField txtFieldDelimiter;
        private SingleLineTextField txtRowTerminator;
        private GroupBox grpFileInfo;
        private Panel panelFileInfoButtons;
        private Button btnAnalyze;
        private Button btnImport;
        private Label lblFileInfo;
        private ListView lstColumns;
        private ColumnHeader colColumnName;
        private ColumnHeader colDataType;
        private Label lblStatus;
        private Label lblError;
    }
}
