namespace CodeGenerator.Presentation.WinForms.Views
{
    partial class TemplateExecutionView
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
            pnlParameters = new Panel();
            pnlFooter = new Panel();
            btnSetDefaults = new Button();
            btnEditTemplate = new Button();
            btnExecute = new Button();
            pnlFooter.SuspendLayout();
            SuspendLayout();
            // 
            // pnlParameters
            // 
            pnlParameters.AutoScroll = true;
            pnlParameters.Dock = DockStyle.Fill;
            pnlParameters.Location = new Point(0, 0);
            pnlParameters.Name = "pnlParameters";
            pnlParameters.Padding = new Padding(5);
            pnlParameters.Size = new Size(300, 250);
            pnlParameters.TabIndex = 0;
            // 
            // pnlFooter
            // 
            pnlFooter.Controls.Add(btnSetDefaults);
            pnlFooter.Controls.Add(btnEditTemplate);
            pnlFooter.Controls.Add(btnExecute);
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Location = new Point(0, 250);
            pnlFooter.Name = "pnlFooter";
            pnlFooter.Size = new Size(300, 50);
            pnlFooter.TabIndex = 1;
            // 
            // btnSetDefaults
            // 
            btnSetDefaults.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnSetDefaults.Location = new Point(12, 12);
            btnSetDefaults.Name = "btnSetDefaults";
            btnSetDefaults.Size = new Size(95, 30);
            btnSetDefaults.TabIndex = 0;
            btnSetDefaults.Text = "Set Defaults";
            btnSetDefaults.UseVisualStyleBackColor = true;
            // 
            // btnEditTemplate
            // 
            btnEditTemplate.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnEditTemplate.Location = new Point(113, 12);
            btnEditTemplate.Name = "btnEditTemplate";
            btnEditTemplate.Size = new Size(90, 30);
            btnEditTemplate.TabIndex = 1;
            btnEditTemplate.Text = "Edit Template";
            btnEditTemplate.UseVisualStyleBackColor = true;
            btnEditTemplate.Visible = false;
            // 
            // btnExecute
            // 
            btnExecute.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnExecute.Location = new Point(209, 12);
            btnExecute.Name = "btnExecute";
            btnExecute.Size = new Size(85, 30);
            btnExecute.TabIndex = 2;
            btnExecute.Text = "Execute";
            btnExecute.UseVisualStyleBackColor = true;
            // 
            // TemplateExecutionView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pnlParameters);
            Controls.Add(pnlFooter);
            Name = "TemplateExecutionView";
            Size = new Size(300, 300);
            pnlFooter.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlParameters;
        private Panel pnlFooter;
        private Button btnSetDefaults;
        private Button btnEditTemplate;
        private Button btnExecute;
    }
}