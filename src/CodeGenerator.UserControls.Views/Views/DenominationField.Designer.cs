namespace CodeGenerator.UserControls.Views
{
    partial class DenominationField
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
            txtDutch = new SingleLineTextField();
            txtFrench = new SingleLineTextField();
            txtEnglish = new SingleLineTextField();
            txtGerman = new SingleLineTextField();
            toolTip = new ToolTip(components);
            SuspendLayout();
            // 
            // txtDutch
            // 
            txtDutch.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDutch.ErrorMessageVisible = true;
            txtDutch.Label = "NL:";
            txtDutch.Location = new Point(0, 0);
            txtDutch.Name = "txtDutch";
            txtDutch.Size = new Size(382, 45);
            txtDutch.TabIndex = 0;
            // 
            // txtFrench
            // 
            txtFrench.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtFrench.ErrorMessageVisible = false;
            txtFrench.Label = "FR:";
            txtFrench.Location = new Point(0, 51);
            txtFrench.Name = "txtFrench";
            txtFrench.Size = new Size(382, 26);
            txtFrench.TabIndex = 1;
            // 
            // txtEnglish
            // 
            txtEnglish.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtEnglish.ErrorMessageVisible = false;
            txtEnglish.Label = "EN:";
            txtEnglish.Location = new Point(0, 83);
            txtEnglish.Name = "txtEnglish";
            txtEnglish.Size = new Size(382, 26);
            txtEnglish.TabIndex = 2;
            // 
            // txtGerman
            // 
            txtGerman.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtGerman.ErrorMessageVisible = false;
            txtGerman.Label = "DE:";
            txtGerman.Location = new Point(0, 115);
            txtGerman.Name = "txtGerman";
            txtGerman.Size = new Size(382, 26);
            txtGerman.TabIndex = 3;
            // 
            // DenominationField
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(txtGerman);
            Controls.Add(txtEnglish);
            Controls.Add(txtFrench);
            Controls.Add(txtDutch);
            Name = "DenominationField";
            Size = new Size(382, 141);
            ResumeLayout(false);
        }

        #endregion

        private SingleLineTextField txtDutch;
        private SingleLineTextField txtFrench;
        private SingleLineTextField txtEnglish;
        private SingleLineTextField txtGerman;
        private ToolTip toolTip;
    }
}
