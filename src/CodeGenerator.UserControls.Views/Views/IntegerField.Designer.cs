namespace CodeGenerator.UserControls.Views
{
    partial class IntegerField
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
            nudValue = new NumericUpDown();
            lblErrorMessage = new Label();
            toolTip = new ToolTip(components);
            ((System.ComponentModel.ISupportInitialize)nudValue).BeginInit();
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
            // nudValue
            // 
            nudValue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            nudValue.Location = new Point(115, 3);
            nudValue.Name = "nudValue";
            nudValue.Size = new Size(225, 23);
            nudValue.TabIndex = 1;
            // 
            // lblErrorMessage
            // 
            lblErrorMessage.AutoSize = true;
            lblErrorMessage.ForeColor = Color.Red;
            lblErrorMessage.Location = new Point(115, 29);
            lblErrorMessage.Name = "lblErrorMessage";
            lblErrorMessage.Size = new Size(81, 15);
            lblErrorMessage.TabIndex = 2;
            lblErrorMessage.Text = "Error message";
            lblErrorMessage.Visible = false;
            // 
            // IntegerField
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(lblErrorMessage);
            Controls.Add(nudValue);
            Controls.Add(lblLabel);
            Name = "IntegerField";
            Size = new Size(343, 49);
            ((System.ComponentModel.ISupportInitialize)nudValue).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblLabel;
        private NumericUpDown nudValue;
        private Label lblErrorMessage;
        private ToolTip toolTip;
    }
}
