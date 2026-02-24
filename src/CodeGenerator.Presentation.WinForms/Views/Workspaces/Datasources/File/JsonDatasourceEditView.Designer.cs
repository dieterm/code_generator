using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Presentation.WinForms.Views
{
    partial class JsonDatasourceEditView
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
            grpFileInfo = new GroupBox();
            panelFileInfoButtons = new Panel();
            btnAnalyze = new Button();
            btnImport = new Button();
            lblFileInfo = new Label();
            lstProperties = new ListView();
            colPropertyName = new ColumnHeader();
            colDataType = new ColumnHeader();
            colNullable = new ColumnHeader();
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
            lblTitle.Size = new Size(74, 31);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "JSON File";
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
            fileField.Label = "JSON File:";
            fileField.Location = new Point(3, 39);
            fileField.Name = "fileField";
            fileField.Size = new Size(362, 30);
            fileField.TabIndex = 1;
            // 
            // grpFileInfo
            // 
            grpFileInfo.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpFileInfo.Controls.Add(lstProperties);
            grpFileInfo.Controls.Add(panelFileInfoButtons);
            grpFileInfo.Controls.Add(lblFileInfo);
            grpFileInfo.Controls.Add(lblStatus);
            grpFileInfo.Controls.Add(lblError);
            grpFileInfo.Location = new Point(10, 150);
            grpFileInfo.Name = "grpFileInfo";
            grpFileInfo.Size = new Size(380, 300);
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
            // lstProperties
            // 
            lstProperties.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstProperties.Columns.AddRange(new ColumnHeader[] { colPropertyName, colDataType, colNullable });
            lstProperties.FullRowSelect = true;
            lstProperties.GridLines = true;
            lstProperties.Location = new Point(6, 80);
            lstProperties.MultiSelect = false;
            lstProperties.Name = "lstProperties";
            lstProperties.Size = new Size(368, 180);
            lstProperties.TabIndex = 2;
            lstProperties.UseCompatibleStateImageBehavior = false;
            lstProperties.View = View.Details;
            // 
            // colPropertyName
            // 
            colPropertyName.Text = "Property Name";
            colPropertyName.Width = 160;
            // 
            // colDataType
            // 
            colDataType.Text = "Data Type";
            colDataType.Width = 100;
            // 
            // colNullable
            // 
            colNullable.Text = "Nullable";
            colNullable.Width = 60;
            // 
            // lblStatus
            // 
            lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblStatus.Location = new Point(6, 263);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(368, 15);
            lblStatus.TabIndex = 3;
            // 
            // lblError
            // 
            lblError.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblError.ForeColor = Color.Red;
            lblError.Location = new Point(6, 278);
            lblError.Name = "lblError";
            lblError.Size = new Size(368, 18);
            lblError.TabIndex = 4;
            lblError.Visible = false;
            // 
            // JsonDatasourceEditView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(grpFileInfo);
            Controls.Add(grpFile);
            Controls.Add(lblTitle);
            Name = "JsonDatasourceEditView";
            Padding = new Padding(10);
            Size = new Size(400, 460);
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
        private GroupBox grpFileInfo;
        private Panel panelFileInfoButtons;
        private Button btnAnalyze;
        private Button btnImport;
        private Label lblFileInfo;
        private ListView lstProperties;
        private ColumnHeader colPropertyName;
        private ColumnHeader colDataType;
        private ColumnHeader colNullable;
        private Label lblStatus;
        private Label lblError;
    }
}
