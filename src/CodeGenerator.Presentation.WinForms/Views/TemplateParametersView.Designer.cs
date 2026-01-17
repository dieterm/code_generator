namespace CodeGenerator.Presentation.WinForms.Views
{
    partial class TemplateParametersView
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
            pnlHeader = new Panel();
            btnToggleEditMode = new Button();
            lblTemplateName = new Label();
            lblTemplateDescription = new Label();
            pnlPlaceholder = new Panel();
            executionView = new TemplateExecutionView();
            editView = new TemplateParametersEditView();
            pnlHeader.SuspendLayout();
            pnlPlaceholder.SuspendLayout();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.Controls.Add(btnToggleEditMode);
            pnlHeader.Controls.Add(lblTemplateName);
            pnlHeader.Controls.Add(lblTemplateDescription);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(350, 70);
            pnlHeader.TabIndex = 0;
            // 
            // btnToggleEditMode
            // 
            btnToggleEditMode.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnToggleEditMode.Location = new Point(279, 5);
            btnToggleEditMode.Name = "btnToggleEditMode";
            btnToggleEditMode.Size = new Size(65, 23);
            btnToggleEditMode.TabIndex = 2;
            btnToggleEditMode.Text = "Edit";
            btnToggleEditMode.UseVisualStyleBackColor = true;
            // 
            // lblTemplateName
            // 
            lblTemplateName.AutoSize = true;
            lblTemplateName.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTemplateName.Location = new Point(3, 5);
            lblTemplateName.Name = "lblTemplateName";
            lblTemplateName.Size = new Size(131, 21);
            lblTemplateName.TabIndex = 0;
            lblTemplateName.Text = "Template Name";
            // 
            // lblTemplateDescription
            // 
            lblTemplateDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblTemplateDescription.Location = new Point(3, 30);
            lblTemplateDescription.Name = "lblTemplateDescription";
            lblTemplateDescription.Size = new Size(270, 35);
            lblTemplateDescription.TabIndex = 1;
            lblTemplateDescription.Text = "Template description goes here...";
            // 
            // pnlPlaceholder
            // 
            pnlPlaceholder.Controls.Add(executionView);
            pnlPlaceholder.Controls.Add(editView);
            pnlPlaceholder.Dock = DockStyle.Fill;
            pnlPlaceholder.Location = new Point(0, 70);
            pnlPlaceholder.Name = "pnlPlaceholder";
            pnlPlaceholder.Size = new Size(350, 380);
            pnlPlaceholder.TabIndex = 1;
            // 
            // executionView
            // 
            executionView.Dock = DockStyle.Fill;
            executionView.Location = new Point(0, 0);
            executionView.Name = "executionView";
            executionView.Size = new Size(350, 380);
            executionView.TabIndex = 0;
            // 
            // editView
            // 
            editView.Dock = DockStyle.Fill;
            editView.Location = new Point(0, 0);
            editView.Name = "editView";
            editView.Size = new Size(350, 380);
            editView.TabIndex = 1;
            editView.Visible = false;
            // 
            // TemplateParametersView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pnlPlaceholder);
            Controls.Add(pnlHeader);
            Name = "TemplateParametersView";
            Size = new Size(350, 450);
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlPlaceholder.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlHeader;
        private Button btnToggleEditMode;
        private Label lblTemplateName;
        private Label lblTemplateDescription;
        private Panel pnlPlaceholder;
        private TemplateExecutionView executionView;
        private TemplateParametersEditView editView;
    }
}
