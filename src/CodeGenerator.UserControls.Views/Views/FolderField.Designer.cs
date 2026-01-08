namespace CodeGenerator.UserControls.Views
{
    partial class FolderField
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            lblLabel = new Label();
            txtValue = new TextBox();
            btnBrowse = new Button();
            btnOpenFolder = new Button();
            lblErrorMessage = new Label();
            SuspendLayout();
            // 
            // lblLabel
            // 
            lblLabel.AutoSize = true;
            lblLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblLabel.Location = new Point(3, 7);
            lblLabel.Name = "lblLabel";
            lblLabel.Size = new Size(68, 15);
            lblLabel.TabIndex = 0;
            lblLabel.Text = "Field Label:";
            // 
            // txtValue
            // 
            txtValue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtValue.Location = new Point(115, 3);
            txtValue.Name = "txtValue";
            txtValue.ReadOnly = true;
            txtValue.Size = new Size(250, 23);
            txtValue.TabIndex = 1;
            // 
            // btnBrowse
            // 
            btnBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowse.BackgroundImageLayout = ImageLayout.Center;
            btnBrowse.Image = Resources.ButtonIcons.folder_open;
            btnBrowse.Location = new Point(371, 2);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(30, 25);
            btnBrowse.TabIndex = 2;
            btnBrowse.UseVisualStyleBackColor = true;
            // 
            // btnOpenFolder
            // 
            btnOpenFolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOpenFolder.BackgroundImageLayout = ImageLayout.Center;
            btnOpenFolder.Image = Resources.ButtonIcons.square_arrow_out_up_right;
            btnOpenFolder.Location = new Point(407, 2);
            btnOpenFolder.Name = "btnOpenFolder";
            btnOpenFolder.Size = new Size(30, 25);
            btnOpenFolder.TabIndex = 3;
            btnOpenFolder.UseVisualStyleBackColor = true;
            // 
            // lblErrorMessage
            // 
            lblErrorMessage.AutoSize = true;
            lblErrorMessage.ForeColor = Color.Red;
            lblErrorMessage.Location = new Point(115, 29);
            lblErrorMessage.Name = "lblErrorMessage";
            lblErrorMessage.Size = new Size(81, 15);
            lblErrorMessage.TabIndex = 4;
            lblErrorMessage.Text = "Error message";
            lblErrorMessage.Visible = false;
            // 
            // FolderField
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(lblErrorMessage);
            Controls.Add(btnOpenFolder);
            Controls.Add(btnBrowse);
            Controls.Add(txtValue);
            Controls.Add(lblLabel);
            Name = "FolderField";
            Size = new Size(440, 49);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblLabel;
        private TextBox txtValue;
        private Button btnBrowse;
        private Button btnOpenFolder;
        private Label lblErrorMessage;
    }
}
