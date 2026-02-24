namespace CodeGenerator.UserControls.Views.Views
{
    partial class SingleButtonField
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
            btnCommand = new Button();
            SuspendLayout();
            // 
            // lblLabel
            // 
            lblLabel.AutoSize = true;
            lblLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblLabel.Location = new Point(3, 4);
            lblLabel.Name = "lblLabel";
            lblLabel.Size = new Size(68, 15);
            lblLabel.TabIndex = 2;
            lblLabel.Text = "Field Label:";
            // 
            // btnCommand
            // 
            btnCommand.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnCommand.Location = new Point(115, 1);
            btnCommand.Name = "btnCommand";
            btnCommand.Size = new Size(251, 23);
            btnCommand.TabIndex = 4;
            btnCommand.Text = "button1";
            btnCommand.UseVisualStyleBackColor = true;
            // 
            // SingleButtonField
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(btnCommand);
            Controls.Add(lblLabel);
            Name = "SingleButtonField";
            Size = new Size(369, 28);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label lblLabel;
        private Button btnCommand;
    }
}
