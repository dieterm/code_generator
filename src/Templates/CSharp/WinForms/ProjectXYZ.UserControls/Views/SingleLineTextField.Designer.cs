namespace ProjectXYZ.UserControls.Views
{
    partial class SingleLineTextField
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
            if (disposing && (components != null))
            {
                components.Dispose();
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
            lblLabel = new Label();
            txtValue = new Syncfusion.Windows.Forms.Tools.TextBoxExt();
            lblErrorMessage = new Label();
            ((System.ComponentModel.ISupportInitialize)txtValue).BeginInit();
            SuspendLayout();
            // 
            // lblLabel
            // 
            lblLabel.AutoSize = true;
            lblLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblLabel.Location = new Point(3, 4);
            lblLabel.Name = "lblLabel";
            lblLabel.Size = new Size(68, 15);
            lblLabel.TabIndex = 0;
            lblLabel.Text = "Field Label:";
            // 
            // txtValue
            // 
            txtValue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtValue.BeforeTouchSize = new Size(99, 23);
            txtValue.Location = new Point(115, 0);
            txtValue.Name = "txtValue";
            txtValue.Size = new Size(99, 23);
            txtValue.TabIndex = 1;
            // 
            // lblErrorMessage
            // 
            lblErrorMessage.AutoSize = true;
            lblErrorMessage.ForeColor = Color.Red;
            lblErrorMessage.Location = new Point(115, 26);
            lblErrorMessage.Name = "lblErrorMessage";
            lblErrorMessage.Size = new Size(81, 15);
            lblErrorMessage.TabIndex = 2;
            lblErrorMessage.Text = "Error message";
            // 
            // SingleLineTextField
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(lblErrorMessage);
            Controls.Add(txtValue);
            Controls.Add(lblLabel);
            Name = "SingleLineTextField";
            Size = new Size(214, 45);
            ((System.ComponentModel.ISupportInitialize)txtValue).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblLabel;
        private Syncfusion.Windows.Forms.Tools.TextBoxExt txtValue;
        private Label lblErrorMessage;
    }
}
