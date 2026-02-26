using CodeGenerator.Presentation.WinForms.Views;
using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Core.Workspaces.Datasources.SqlServer.Views
{
    partial class SqlServerDatasourceEditView
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
            grpConnection = new GroupBox();
            tableLayoutConnection = new TableLayoutPanel();
            txtName = new SingleLineTextField();
            txtServer = new SingleLineTextField();
            txtDatabase = new SingleLineTextField();
            chkIntegratedSecurity = new BooleanField();
            txtUsername = new SingleLineTextField();
            txtPassword = new SingleLineTextField();
            chkTrustServerCertificate = new BooleanField();
            objectImportField = new DatasourceObjectImportField();
            grpConnection.SuspendLayout();
            tableLayoutConnection.SuspendLayout();
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
            lblTitle.Size = new Size(92, 31);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "SQL Server";
            // 
            // grpConnection
            // 
            grpConnection.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpConnection.Controls.Add(tableLayoutConnection);
            grpConnection.Location = new Point(10, 44);
            grpConnection.Name = "grpConnection";
            grpConnection.Size = new Size(380, 310);
            grpConnection.TabIndex = 1;
            grpConnection.TabStop = false;
            grpConnection.Text = "Connection Settings";
            // 
            // tableLayoutConnection
            // 
            tableLayoutConnection.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutConnection.AutoSize = true;
            tableLayoutConnection.ColumnCount = 1;
            tableLayoutConnection.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutConnection.Controls.Add(txtName, 0, 0);
            tableLayoutConnection.Controls.Add(txtServer, 0, 1);
            tableLayoutConnection.Controls.Add(txtDatabase, 0, 2);
            tableLayoutConnection.Controls.Add(chkIntegratedSecurity, 0, 3);
            tableLayoutConnection.Controls.Add(txtUsername, 0, 4);
            tableLayoutConnection.Controls.Add(txtPassword, 0, 5);
            tableLayoutConnection.Controls.Add(chkTrustServerCertificate, 0, 6);
            tableLayoutConnection.Location = new Point(6, 22);
            tableLayoutConnection.Name = "tableLayoutConnection";
            tableLayoutConnection.RowCount = 7;
            tableLayoutConnection.RowStyles.Add(new RowStyle());
            tableLayoutConnection.RowStyles.Add(new RowStyle());
            tableLayoutConnection.RowStyles.Add(new RowStyle());
            tableLayoutConnection.RowStyles.Add(new RowStyle());
            tableLayoutConnection.RowStyles.Add(new RowStyle());
            tableLayoutConnection.RowStyles.Add(new RowStyle());
            tableLayoutConnection.RowStyles.Add(new RowStyle());
            tableLayoutConnection.Size = new Size(368, 282);
            tableLayoutConnection.TabIndex = 0;
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
            // txtServer
            // 
            txtServer.Dock = DockStyle.Top;
            txtServer.ErrorMessageVisible = true;
            txtServer.Label = "Server (e.g., localhost\\SQLEXPRESS):";
            txtServer.Location = new Point(3, 45);
            txtServer.Name = "txtServer";
            txtServer.Size = new Size(362, 36);
            txtServer.TabIndex = 1;
            // 
            // txtDatabase
            // 
            txtDatabase.Dock = DockStyle.Top;
            txtDatabase.ErrorMessageVisible = true;
            txtDatabase.Label = "Database:";
            txtDatabase.Location = new Point(3, 87);
            txtDatabase.Name = "txtDatabase";
            txtDatabase.Size = new Size(362, 36);
            txtDatabase.TabIndex = 2;
            // 
            // chkIntegratedSecurity
            // 
            chkIntegratedSecurity.Dock = DockStyle.Top;
            chkIntegratedSecurity.Label = "Windows Authentication:";
            chkIntegratedSecurity.Location = new Point(3, 129);
            chkIntegratedSecurity.Name = "chkIntegratedSecurity";
            chkIntegratedSecurity.Size = new Size(362, 30);
            chkIntegratedSecurity.TabIndex = 3;
            // 
            // txtUsername
            // 
            txtUsername.Dock = DockStyle.Top;
            txtUsername.ErrorMessageVisible = true;
            txtUsername.Label = "Username:";
            txtUsername.Location = new Point(3, 165);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(362, 36);
            txtUsername.TabIndex = 4;
            // 
            // txtPassword
            // 
            txtPassword.Dock = DockStyle.Top;
            txtPassword.ErrorMessageVisible = true;
            txtPassword.Label = "Password:";
            txtPassword.Location = new Point(3, 207);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(362, 36);
            txtPassword.TabIndex = 5;
            // 
            // chkTrustServerCertificate
            // 
            chkTrustServerCertificate.Dock = DockStyle.Top;
            chkTrustServerCertificate.Label = "Trust Server Certificate:";
            chkTrustServerCertificate.Location = new Point(3, 249);
            chkTrustServerCertificate.Name = "chkTrustServerCertificate";
            chkTrustServerCertificate.Size = new Size(362, 30);
            chkTrustServerCertificate.TabIndex = 6;
            // 
            // objectImportField
            // 
            objectImportField.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            objectImportField.Location = new Point(10, 360);
            objectImportField.Name = "objectImportField";
            objectImportField.Size = new Size(380, 250);
            objectImportField.TabIndex = 2;
            // 
            // SqlServerDatasourceEditView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(objectImportField);
            Controls.Add(grpConnection);
            Controls.Add(lblTitle);
            Name = "SqlServerDatasourceEditView";
            Padding = new Padding(10);
            Size = new Size(400, 620);
            grpConnection.ResumeLayout(false);
            grpConnection.PerformLayout();
            tableLayoutConnection.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private GroupBox grpConnection;
        private TableLayoutPanel tableLayoutConnection;
        private SingleLineTextField txtName;
        private SingleLineTextField txtServer;
        private SingleLineTextField txtDatabase;
        private BooleanField chkIntegratedSecurity;
        private SingleLineTextField txtUsername;
        private SingleLineTextField txtPassword;
        private BooleanField chkTrustServerCertificate;
        private DatasourceObjectImportField objectImportField;
    }
}
