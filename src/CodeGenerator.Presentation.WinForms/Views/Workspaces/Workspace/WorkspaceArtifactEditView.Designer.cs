namespace CodeGenerator.Presentation.WinForms.Views.Workspace
{
    partial class WorkspaceArtifactEditView
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
            tabFields = new TabControl();
            tabGeneral = new TabPage();
            tabDocumentation = new TabPage();
            lblTitle = new Label();
            fieldCollection1 = new CodeGenerator.UserControls.Views.FieldCollection();
            tabCustomProperties = new TabPage();
            tabFields.SuspendLayout();
            tabGeneral.SuspendLayout();
            SuspendLayout();
            // 
            // tabFields
            // 
            tabFields.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabFields.Controls.Add(tabGeneral);
            tabFields.Controls.Add(tabDocumentation);
            tabFields.Controls.Add(tabCustomProperties);
            tabFields.Location = new Point(3, 33);
            tabFields.Name = "tabFields";
            tabFields.SelectedIndex = 0;
            tabFields.Size = new Size(511, 485);
            tabFields.TabIndex = 0;
            // 
            // tabGeneral
            // 
            tabGeneral.Controls.Add(fieldCollection1);
            tabGeneral.Location = new Point(4, 24);
            tabGeneral.Name = "tabGeneral";
            tabGeneral.Padding = new Padding(3);
            tabGeneral.Size = new Size(503, 457);
            tabGeneral.TabIndex = 0;
            tabGeneral.Text = "General";
            tabGeneral.UseVisualStyleBackColor = true;
            // 
            // tabDocumentation
            // 
            tabDocumentation.Location = new Point(4, 24);
            tabDocumentation.Name = "tabDocumentation";
            tabDocumentation.Padding = new Padding(3);
            tabDocumentation.Size = new Size(503, 457);
            tabDocumentation.TabIndex = 1;
            tabDocumentation.Text = "Documentation";
            tabDocumentation.UseVisualStyleBackColor = true;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTitle.Location = new Point(7, 6);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(169, 21);
            lblTitle.TabIndex = 1;
            lblTitle.Text = "<artifact> Properties";
            // 
            // fieldCollection1
            // 
            fieldCollection1.Dock = DockStyle.Fill;
            fieldCollection1.Location = new Point(3, 3);
            fieldCollection1.Name = "fieldCollection1";
            fieldCollection1.Size = new Size(497, 451);
            fieldCollection1.TabIndex = 0;
            // 
            // tabCustomProperties
            // 
            tabCustomProperties.Location = new Point(4, 24);
            tabCustomProperties.Name = "tabCustomProperties";
            tabCustomProperties.Padding = new Padding(3);
            tabCustomProperties.Size = new Size(503, 457);
            tabCustomProperties.TabIndex = 2;
            tabCustomProperties.Text = "Custom Properties";
            tabCustomProperties.UseVisualStyleBackColor = true;
            // 
            // WorkspaceArtifactEditView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(lblTitle);
            Controls.Add(tabFields);
            Name = "WorkspaceArtifactEditView";
            Size = new Size(517, 521);
            tabFields.ResumeLayout(false);
            tabGeneral.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TabControl tabFields;
        private TabPage tabGeneral;
        private TabPage tabDocumentation;
        private UserControls.Views.FieldCollection fieldCollection1;
        private Label lblTitle;
        private TabPage tabCustomProperties;
    }
}
