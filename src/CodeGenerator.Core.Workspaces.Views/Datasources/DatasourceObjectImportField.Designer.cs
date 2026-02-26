namespace CodeGenerator.Presentation.WinForms.Views
{
    partial class DatasourceObjectImportField
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_viewModel != null)
                {
                    _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
                    _viewModel.Items.CollectionChanged -= Items_CollectionChanged;
                }
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            grpObjects = new GroupBox();
            lstObjects = new ListView();
            panelButtons = new Panel();
            btnLoad = new Button();
            btnAddSelected = new Button();
            btnAddAll = new Button();
            lblInfo = new Label();
            lblStatus = new Label();
            lblError = new Label();
            grpObjects.SuspendLayout();
            panelButtons.SuspendLayout();
            SuspendLayout();
            // 
            // grpObjects
            // 
            grpObjects.Controls.Add(lstObjects);
            grpObjects.Controls.Add(lblInfo);
            grpObjects.Controls.Add(panelButtons);
            grpObjects.Controls.Add(lblStatus);
            grpObjects.Controls.Add(lblError);
            grpObjects.Dock = DockStyle.Fill;
            grpObjects.Location = new Point(0, 0);
            grpObjects.Name = "grpObjects";
            grpObjects.Size = new Size(380, 250);
            grpObjects.TabIndex = 0;
            grpObjects.TabStop = false;
            grpObjects.Text = "Available Objects";
            // 
            // panelButtons
            // 
            panelButtons.Controls.Add(btnLoad);
            panelButtons.Controls.Add(btnAddSelected);
            panelButtons.Controls.Add(btnAddAll);
            panelButtons.Dock = DockStyle.Top;
            panelButtons.Location = new Point(3, 19);
            panelButtons.Name = "panelButtons";
            panelButtons.Size = new Size(374, 35);
            panelButtons.TabIndex = 0;
            // 
            // btnLoad
            // 
            btnLoad.Location = new Point(3, 6);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(120, 26);
            btnLoad.TabIndex = 0;
            btnLoad.Text = "Load";
            btnLoad.UseVisualStyleBackColor = true;
            btnLoad.Click += BtnLoad_Click;
            // 
            // btnAddSelected
            // 
            btnAddSelected.Enabled = false;
            btnAddSelected.Location = new Point(129, 6);
            btnAddSelected.Name = "btnAddSelected";
            btnAddSelected.Size = new Size(100, 26);
            btnAddSelected.TabIndex = 1;
            btnAddSelected.Text = "Add Selected";
            btnAddSelected.UseVisualStyleBackColor = true;
            btnAddSelected.Click += BtnAddSelected_Click;
            // 
            // btnAddAll
            // 
            btnAddAll.Enabled = false;
            btnAddAll.Location = new Point(235, 6);
            btnAddAll.Name = "btnAddAll";
            btnAddAll.Size = new Size(75, 26);
            btnAddAll.TabIndex = 2;
            btnAddAll.Text = "Add All";
            btnAddAll.UseVisualStyleBackColor = true;
            btnAddAll.Visible = false;
            btnAddAll.Click += BtnAddAll_Click;
            // 
            // lblInfo
            // 
            lblInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblInfo.Location = new Point(6, 57);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(368, 20);
            lblInfo.TabIndex = 1;
            lblInfo.Visible = false;
            // 
            // lstObjects
            // 
            lstObjects.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstObjects.FullRowSelect = true;
            lstObjects.GridLines = true;
            lstObjects.Location = new Point(6, 60);
            lstObjects.MultiSelect = false;
            lstObjects.Name = "lstObjects";
            lstObjects.Size = new Size(368, 150);
            lstObjects.TabIndex = 2;
            lstObjects.UseCompatibleStateImageBehavior = false;
            lstObjects.View = View.Details;
            lstObjects.SelectedIndexChanged += LstObjects_SelectedIndexChanged;
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
            // DatasourceObjectImportField
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(grpObjects);
            Name = "DatasourceObjectImportField";
            Size = new Size(380, 250);
            grpObjects.ResumeLayout(false);
            panelButtons.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private GroupBox grpObjects;
        private Panel panelButtons;
        private Button btnLoad;
        private Button btnAddSelected;
        private Button btnAddAll;
        private Label lblInfo;
        private ListView lstObjects;
        private Label lblStatus;
        private Label lblError;
    }
}
