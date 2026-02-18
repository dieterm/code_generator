namespace CodeGenerator.UserControls.Views
{
    partial class LabelField
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

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            lblLabel = new Label();
            lblValue = new Label();
            lblErrorMessage = new Label();
            toolTip = new ToolTip(components);
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
            // lblValue
            // 
            lblValue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblValue.AutoEllipsis = true;
            lblValue.BackColor = SystemColors.ControlLight;
            lblValue.BorderStyle = BorderStyle.FixedSingle;
            lblValue.Location = new Point(115, 0);
            lblValue.Name = "lblValue";
            lblValue.Padding = new Padding(3, 3, 3, 3);
            lblValue.Size = new Size(99, 23);
            lblValue.TabIndex = 1;
            lblValue.TextAlign = ContentAlignment.MiddleLeft;
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
            // LabelField
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(lblErrorMessage);
            Controls.Add(lblValue);
            Controls.Add(lblLabel);
            Name = "LabelField";
            Size = new Size(214, 45);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblLabel;
        private Label lblValue;
        private Label lblErrorMessage;
        private ToolTip toolTip;
    }
}
