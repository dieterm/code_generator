using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Presentation.WinForms.Views
{
    partial class MysqlDatasourceEditView
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
            txtPort = new IntegerField();
            txtDatabase = new SingleLineTextField();
            txtUsername = new SingleLineTextField();
            txtPassword = new SingleLineTextField();
            grpObjects = new GroupBox();
            lstObjects = new ListView();
            colName = new ColumnHeader();
            colSchema = new ColumnHeader();
            colType = new ColumnHeader();
            panelObjectsButtons = new Panel();
            btnLoadObjects = new Button();
            btnAddObject = new Button();
            lblStatus = new Label();
            lblError = new Label();
            grpConnection.SuspendLayout();
            tableLayoutConnection.SuspendLayout();
            grpObjects.SuspendLayout();
            panelObjectsButtons.SuspendLayout();
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
            lblTitle.Size = new Size(144, 31);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "MySQL / MariaDB";
            // 
            // grpConnection
            // 
            grpConnection.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpConnection.Controls.Add(tableLayoutConnection);
            grpConnection.Location = new Point(10, 44);
            grpConnection.Name = "grpConnection";
            grpConnection.Size = new Size(380, 280);
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
            tableLayoutConnection.Controls.Add(txtPort, 0, 2);
            tableLayoutConnection.Controls.Add(txtDatabase, 0, 3);
            tableLayoutConnection.Controls.Add(txtUsername, 0, 4);
            tableLayoutConnection.Controls.Add(txtPassword, 0, 5);
            tableLayoutConnection.Location = new Point(6, 22);
            tableLayoutConnection.Name = "tableLayoutConnection";
            tableLayoutConnection.RowCount = 6;
            tableLayoutConnection.RowStyles.Add(new RowStyle());
            tableLayoutConnection.RowStyles.Add(new RowStyle());
            tableLayoutConnection.RowStyles.Add(new RowStyle());
            tableLayoutConnection.RowStyles.Add(new RowStyle());
            tableLayoutConnection.RowStyles.Add(new RowStyle());
            tableLayoutConnection.RowStyles.Add(new RowStyle());
            tableLayoutConnection.Size = new Size(368, 252);
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
            txtServer.Label = "Server:";
            txtServer.Location = new Point(3, 45);
            txtServer.Name = "txtServer";
            txtServer.Size = new Size(362, 36);
            txtServer.TabIndex = 1;
            // 
            // txtPort
            // 
            txtPort.Dock = DockStyle.Top;
            txtPort.Label = "Port:";
            txtPort.Location = new Point(3, 87);
            txtPort.Name = "txtPort";
            txtPort.Size = new Size(362, 36);
            txtPort.TabIndex = 2;
            // 
            // txtDatabase
            // 
            txtDatabase.Dock = DockStyle.Top;
            txtDatabase.ErrorMessageVisible = true;
            txtDatabase.Label = "Database:";
            txtDatabase.Location = new Point(3, 129);
            txtDatabase.Name = "txtDatabase";
            txtDatabase.Size = new Size(362, 36);
            txtDatabase.TabIndex = 3;
            // 
            // txtUsername
            // 
            txtUsername.Dock = DockStyle.Top;
            txtUsername.ErrorMessageVisible = true;
            txtUsername.Label = "Username:";
            txtUsername.Location = new Point(3, 171);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(362, 36);
            txtUsername.TabIndex = 4;
            // 
            // txtPassword
            // 
            txtPassword.Dock = DockStyle.Top;
            txtPassword.ErrorMessageVisible = true;
            txtPassword.Label = "Password:";
            txtPassword.Location = new Point(3, 213);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(362, 36);
            txtPassword.TabIndex = 5;
            // 
            // grpObjects
            // 
            grpObjects.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpObjects.Controls.Add(lstObjects);
            grpObjects.Controls.Add(panelObjectsButtons);
            grpObjects.Controls.Add(lblStatus);
            grpObjects.Controls.Add(lblError);
            grpObjects.Location = new Point(10, 330);
            grpObjects.Name = "grpObjects";
            grpObjects.Size = new Size(380, 250);
            grpObjects.TabIndex = 2;
            grpObjects.TabStop = false;
            grpObjects.Text = "Available Tables and Views";
            // 
            // lstObjects
            // 
            lstObjects.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstObjects.Columns.AddRange(new ColumnHeader[] { colName, colSchema, colType });
            lstObjects.FullRowSelect = true;
            lstObjects.GridLines = true;
            lstObjects.Location = new Point(6, 60);
            lstObjects.MultiSelect = false;
            lstObjects.Name = "lstObjects";
            lstObjects.Size = new Size(368, 150);
            lstObjects.TabIndex = 1;
            lstObjects.UseCompatibleStateImageBehavior = false;
            lstObjects.View = View.Details;
            // 
            // colName
            // 
            colName.Text = "Name";
            colName.Width = 180;
            // 
            // colSchema
            // 
            colSchema.Text = "Schema";
            colSchema.Width = 100;
            // 
            // colType
            // 
            colType.Text = "Type";
            colType.Width = 80;
            // 
            // panelObjectsButtons
            // 
            panelObjectsButtons.Controls.Add(btnLoadObjects);
            panelObjectsButtons.Controls.Add(btnAddObject);
            panelObjectsButtons.Dock = DockStyle.Top;
            panelObjectsButtons.Location = new Point(3, 19);
            panelObjectsButtons.Name = "panelObjectsButtons";
            panelObjectsButtons.Size = new Size(374, 35);
            panelObjectsButtons.TabIndex = 0;
            // 
            // btnLoadObjects
            // 
            btnLoadObjects.Location = new Point(3, 6);
            btnLoadObjects.Name = "btnLoadObjects";
            btnLoadObjects.Size = new Size(120, 26);
            btnLoadObjects.TabIndex = 0;
            btnLoadObjects.Text = "Load Tables/Views";
            btnLoadObjects.UseVisualStyleBackColor = true;
            // 
            // btnAddObject
            // 
            btnAddObject.Enabled = false;
            btnAddObject.Location = new Point(129, 6);
            btnAddObject.Name = "btnAddObject";
            btnAddObject.Size = new Size(100, 26);
            btnAddObject.TabIndex = 1;
            btnAddObject.Text = "Add Selected";
            btnAddObject.UseVisualStyleBackColor = true;
            // 
            // lblStatus
            // 
            lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblStatus.Location = new Point(6, 213);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(368, 15);
            lblStatus.TabIndex = 2;
            // 
            // lblError
            // 
            lblError.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblError.ForeColor = Color.Red;
            lblError.Location = new Point(6, 228);
            lblError.Name = "lblError";
            lblError.Size = new Size(368, 18);
            lblError.TabIndex = 3;
            lblError.Visible = false;
            // 
            // MysqlDatasourceEditView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(grpObjects);
            Controls.Add(grpConnection);
            Controls.Add(lblTitle);
            Name = "MysqlDatasourceEditView";
            Padding = new Padding(10);
            Size = new Size(400, 590);
            grpConnection.ResumeLayout(false);
            grpConnection.PerformLayout();
            tableLayoutConnection.ResumeLayout(false);
            grpObjects.ResumeLayout(false);
            panelObjectsButtons.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private GroupBox grpConnection;
        private TableLayoutPanel tableLayoutConnection;
        private SingleLineTextField txtName;
        private SingleLineTextField txtServer;
        private IntegerField txtPort;
        private SingleLineTextField txtDatabase;
        private SingleLineTextField txtUsername;
        private SingleLineTextField txtPassword;
        private GroupBox grpObjects;
        private Panel panelObjectsButtons;
        private Button btnLoadObjects;
        private Button btnAddObject;
        private ListView lstObjects;
        private ColumnHeader colName;
        private ColumnHeader colSchema;
        private ColumnHeader colType;
        private Label lblStatus;
        private Label lblError;
    }
}
