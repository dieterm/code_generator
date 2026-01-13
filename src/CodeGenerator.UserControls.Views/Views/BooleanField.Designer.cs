namespace CodeGenerator.UserControls.Views
{
    partial class BooleanField
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
            components = new System.ComponentModel.Container();
            lblLabel = new Label();
            rbYes = new RadioButton();
            rbNo = new RadioButton();
            lblErrorMessage = new Label();
            toolTip = new ToolTip(components);
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
            // rbYes
            // 
            rbYes.AutoSize = true;
            rbYes.Location = new Point(115, 5);
            rbYes.Name = "rbYes";
            rbYes.Size = new Size(35, 19);
            rbYes.TabIndex = 1;
            rbYes.TabStop = true;
            rbYes.Text = "Ja";
            rbYes.UseVisualStyleBackColor = true;
            // 
            // rbNo
            // 
            rbNo.AutoSize = true;
            rbNo.Location = new Point(170, 5);
            rbNo.Name = "rbNo";
            rbNo.Size = new Size(46, 19);
            rbNo.TabIndex = 2;
            rbNo.TabStop = true;
            rbNo.Text = "Nee";
            rbNo.UseVisualStyleBackColor = true;
            // 
            // lblErrorMessage
            // 
            lblErrorMessage.AutoSize = true;
            lblErrorMessage.ForeColor = Color.Red;
            lblErrorMessage.Location = new Point(115, 27);
            lblErrorMessage.Name = "lblErrorMessage";
            lblErrorMessage.Size = new Size(81, 15);
            lblErrorMessage.TabIndex = 3;
            lblErrorMessage.Text = "Error message";
            lblErrorMessage.Visible = false;
            // 
            // BooleanField
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(lblErrorMessage);
            Controls.Add(rbNo);
            Controls.Add(rbYes);
            Controls.Add(lblLabel);
            Name = "BooleanField";
            Size = new Size(343, 47);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblLabel;
        private RadioButton rbYes;
        private RadioButton rbNo;
        private Label lblErrorMessage;
        private ToolTip toolTip;
    }
}
