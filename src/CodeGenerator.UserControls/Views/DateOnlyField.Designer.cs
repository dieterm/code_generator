namespace CodeGenerator.UserControls.Views
{
    partial class DateOnlyField
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
            dtpValue = new DateTimePicker();
            chkHasValue = new CheckBox();
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
            // dtpValue
            // 
            dtpValue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dtpValue.Format = DateTimePickerFormat.Short;
            dtpValue.Location = new Point(140, 3);
            dtpValue.Name = "dtpValue";
            dtpValue.Size = new Size(200, 23);
            dtpValue.TabIndex = 2;
            // 
            // chkHasValue
            // 
            chkHasValue.AutoSize = true;
            chkHasValue.Checked = true;
            chkHasValue.CheckState = CheckState.Checked;
            chkHasValue.Location = new Point(115, 6);
            chkHasValue.Name = "chkHasValue";
            chkHasValue.Size = new Size(15, 14);
            chkHasValue.TabIndex = 1;
            chkHasValue.UseVisualStyleBackColor = true;
            // 
            // lblErrorMessage
            // 
            lblErrorMessage.AutoSize = true;
            lblErrorMessage.ForeColor = Color.Red;
            lblErrorMessage.Location = new Point(140, 29);
            lblErrorMessage.Name = "lblErrorMessage";
            lblErrorMessage.Size = new Size(81, 15);
            lblErrorMessage.TabIndex = 3;
            lblErrorMessage.Text = "Error message";
            lblErrorMessage.Visible = false;
            // 
            // DateOnlyField
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(lblErrorMessage);
            Controls.Add(chkHasValue);
            Controls.Add(dtpValue);
            Controls.Add(lblLabel);
            Name = "DateOnlyField";
            Size = new Size(343, 50);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblLabel;
        private DateTimePicker dtpValue;
        private CheckBox chkHasValue;
        private Label lblErrorMessage;
    }
}
