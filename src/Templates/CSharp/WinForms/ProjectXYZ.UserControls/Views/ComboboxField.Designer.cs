namespace ProjectXYZ.UserControls.Views
{
    partial class ComboboxField
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
            lblErrorMessage = new Label();
            lblLabel = new Label();
            cbxItems = new Syncfusion.WinForms.ListView.SfComboBox();
            ((System.ComponentModel.ISupportInitialize)cbxItems).BeginInit();
            SuspendLayout();
            // 
            // lblErrorMessage
            // 
            lblErrorMessage.AutoSize = true;
            lblErrorMessage.ForeColor = Color.Red;
            lblErrorMessage.Location = new Point(115, 26);
            lblErrorMessage.Name = "lblErrorMessage";
            lblErrorMessage.Size = new Size(81, 15);
            lblErrorMessage.TabIndex = 5;
            lblErrorMessage.Text = "Error message";
            // 
            // lblLabel
            // 
            lblLabel.AutoSize = true;
            lblLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblLabel.Location = new Point(3, 4);
            lblLabel.Name = "lblLabel";
            lblLabel.Size = new Size(68, 15);
            lblLabel.TabIndex = 3;
            lblLabel.Text = "Field Label:";
            // 
            // cbxItems
            // 
            cbxItems.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cbxItems.DropDownPosition = Syncfusion.WinForms.Core.Enums.PopupRelativeAlignment.Center;
            cbxItems.DropDownStyle = Syncfusion.WinForms.ListView.Enums.DropDownStyle.DropDownList;
            cbxItems.Location = new Point(115, 0);
            cbxItems.Name = "cbxItems";
            cbxItems.Size = new Size(300, 23);
            cbxItems.Style.TokenStyle.CloseButtonBackColor = Color.FromArgb(255, 255, 255);
            cbxItems.TabIndex = 6;
            // 
            // ComboboxField
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(cbxItems);
            Controls.Add(lblErrorMessage);
            Controls.Add(lblLabel);
            Name = "ComboboxField";
            Size = new Size(415, 46);
            ((System.ComponentModel.ISupportInitialize)cbxItems).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblErrorMessage;
        private Label lblLabel;
        private Syncfusion.WinForms.ListView.SfComboBox cbxItems;
    }
}
