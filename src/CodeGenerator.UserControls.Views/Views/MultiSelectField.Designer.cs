namespace CodeGenerator.UserControls.Views
{
    partial class MultiSelectField
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
            components = new System.ComponentModel.Container();
            lblLabel = new Label();
            clbItems = new CheckedListBox();
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
            // clbItems
            // 
            clbItems.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            clbItems.CheckOnClick = true;
            clbItems.FormattingEnabled = true;
            clbItems.IntegralHeight = false;
            clbItems.Location = new Point(115, 0);
            clbItems.Name = "clbItems";
            clbItems.Size = new Size(300, 95);
            clbItems.TabIndex = 1;
            // 
            // lblErrorMessage
            // 
            lblErrorMessage.AutoSize = true;
            lblErrorMessage.ForeColor = Color.Red;
            lblErrorMessage.Location = new Point(115, 98);
            lblErrorMessage.Name = "lblErrorMessage";
            lblErrorMessage.Size = new Size(81, 15);
            lblErrorMessage.TabIndex = 2;
            lblErrorMessage.Text = "Error message";
            lblErrorMessage.Visible = false;
            // 
            // MultiSelectField
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(lblErrorMessage);
            Controls.Add(clbItems);
            Controls.Add(lblLabel);
            Name = "MultiSelectField";
            Size = new Size(415, 115);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblLabel;
        private CheckedListBox clbItems;
        private Label lblErrorMessage;
        private ToolTip toolTip;
    }
}
