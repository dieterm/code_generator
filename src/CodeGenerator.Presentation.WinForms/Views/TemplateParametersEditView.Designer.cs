namespace CodeGenerator.Presentation.WinForms.Views
{
    partial class TemplateParametersEditView
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
            pnlTemplateMetadata = new Panel();
            txtEditDescription = new TextBox();
            lblEditDescription = new Label();
            txtEditDisplayName = new TextBox();
            lblEditDisplayName = new Label();
            txtEditTemplateId = new TextBox();
            lblEditTemplateId = new Label();
            splitContainer = new SplitContainer();
            lstParameters = new ListBox();
            pnlParameterButtons = new Panel();
            btnMoveDown = new Button();
            btnMoveUp = new Button();
            btnRemoveParameter = new Button();
            btnAddParameter = new Button();
            parameterEditView = new TemplateParameterEditView();
            pnlFooter = new Panel();
            btnSave = new Button();
            pnlTemplateMetadata.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            pnlParameterButtons.SuspendLayout();
            pnlFooter.SuspendLayout();
            SuspendLayout();
            // 
            // pnlTemplateMetadata
            // 
            pnlTemplateMetadata.Controls.Add(txtEditDescription);
            pnlTemplateMetadata.Controls.Add(lblEditDescription);
            pnlTemplateMetadata.Controls.Add(txtEditDisplayName);
            pnlTemplateMetadata.Controls.Add(lblEditDisplayName);
            pnlTemplateMetadata.Controls.Add(txtEditTemplateId);
            pnlTemplateMetadata.Controls.Add(lblEditTemplateId);
            pnlTemplateMetadata.Dock = DockStyle.Top;
            pnlTemplateMetadata.Location = new Point(0, 0);
            pnlTemplateMetadata.Name = "pnlTemplateMetadata";
            pnlTemplateMetadata.Size = new Size(350, 81);
            pnlTemplateMetadata.TabIndex = 0;
            // 
            // txtEditDescription
            // 
            txtEditDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtEditDescription.Location = new Point(90, 55);
            txtEditDescription.Name = "txtEditDescription";
            txtEditDescription.Size = new Size(255, 23);
            txtEditDescription.TabIndex = 5;
            // 
            // lblEditDescription
            // 
            lblEditDescription.AutoSize = true;
            lblEditDescription.Location = new Point(5, 58);
            lblEditDescription.Name = "lblEditDescription";
            lblEditDescription.Size = new Size(70, 15);
            lblEditDescription.TabIndex = 4;
            lblEditDescription.Text = "Description:";
            // 
            // txtEditDisplayName
            // 
            txtEditDisplayName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtEditDisplayName.Location = new Point(90, 29);
            txtEditDisplayName.Name = "txtEditDisplayName";
            txtEditDisplayName.Size = new Size(255, 23);
            txtEditDisplayName.TabIndex = 3;
            // 
            // lblEditDisplayName
            // 
            lblEditDisplayName.AutoSize = true;
            lblEditDisplayName.Location = new Point(5, 32);
            lblEditDisplayName.Name = "lblEditDisplayName";
            lblEditDisplayName.Size = new Size(83, 15);
            lblEditDisplayName.TabIndex = 2;
            lblEditDisplayName.Text = "Display Name:";
            // 
            // txtEditTemplateId
            // 
            txtEditTemplateId.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtEditTemplateId.Location = new Point(90, 3);
            txtEditTemplateId.Name = "txtEditTemplateId";
            txtEditTemplateId.Size = new Size(255, 23);
            txtEditTemplateId.TabIndex = 1;
            // 
            // lblEditTemplateId
            // 
            lblEditTemplateId.AutoSize = true;
            lblEditTemplateId.Location = new Point(5, 6);
            lblEditTemplateId.Name = "lblEditTemplateId";
            lblEditTemplateId.Size = new Size(73, 15);
            lblEditTemplateId.TabIndex = 0;
            lblEditTemplateId.Text = "Template ID:";
            // 
            // splitContainer
            // 
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.Location = new Point(0, 81);
            splitContainer.Name = "splitContainer";
            splitContainer.Orientation = Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.Controls.Add(lstParameters);
            splitContainer.Panel1.Controls.Add(pnlParameterButtons);
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.Controls.Add(parameterEditView);
            splitContainer.Size = new Size(350, 319);
            splitContainer.SplitterDistance = 100;
            splitContainer.TabIndex = 1;
            // 
            // lstParameters
            // 
            lstParameters.Dock = DockStyle.Fill;
            lstParameters.FormattingEnabled = true;
            lstParameters.ItemHeight = 15;
            lstParameters.Location = new Point(0, 0);
            lstParameters.Name = "lstParameters";
            lstParameters.Size = new Size(250, 100);
            lstParameters.TabIndex = 0;
            // 
            // pnlParameterButtons
            // 
            pnlParameterButtons.Controls.Add(btnMoveDown);
            pnlParameterButtons.Controls.Add(btnMoveUp);
            pnlParameterButtons.Controls.Add(btnRemoveParameter);
            pnlParameterButtons.Controls.Add(btnAddParameter);
            pnlParameterButtons.Dock = DockStyle.Right;
            pnlParameterButtons.Location = new Point(250, 0);
            pnlParameterButtons.Name = "pnlParameterButtons";
            pnlParameterButtons.Size = new Size(100, 100);
            pnlParameterButtons.TabIndex = 1;
            // 
            // btnMoveDown
            // 
            btnMoveDown.Location = new Point(3, 76);
            btnMoveDown.Name = "btnMoveDown";
            btnMoveDown.Size = new Size(94, 23);
            btnMoveDown.TabIndex = 3;
            btnMoveDown.Text = "Move Down";
            btnMoveDown.UseVisualStyleBackColor = true;
            // 
            // btnMoveUp
            // 
            btnMoveUp.Location = new Point(3, 52);
            btnMoveUp.Name = "btnMoveUp";
            btnMoveUp.Size = new Size(94, 23);
            btnMoveUp.TabIndex = 2;
            btnMoveUp.Text = "Move Up";
            btnMoveUp.UseVisualStyleBackColor = true;
            // 
            // btnRemoveParameter
            // 
            btnRemoveParameter.Location = new Point(3, 27);
            btnRemoveParameter.Name = "btnRemoveParameter";
            btnRemoveParameter.Size = new Size(94, 23);
            btnRemoveParameter.TabIndex = 1;
            btnRemoveParameter.Text = "Remove";
            btnRemoveParameter.UseVisualStyleBackColor = true;
            // 
            // btnAddParameter
            // 
            btnAddParameter.Location = new Point(3, 3);
            btnAddParameter.Name = "btnAddParameter";
            btnAddParameter.Size = new Size(94, 23);
            btnAddParameter.TabIndex = 0;
            btnAddParameter.Text = "Add";
            btnAddParameter.UseVisualStyleBackColor = true;
            // 
            // parameterEditView
            // 
            parameterEditView.Dock = DockStyle.Fill;
            parameterEditView.Location = new Point(0, 0);
            parameterEditView.Name = "parameterEditView";
            parameterEditView.Size = new Size(350, 215);
            parameterEditView.TabIndex = 0;
            // 
            // pnlFooter
            // 
            pnlFooter.Controls.Add(btnSave);
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Location = new Point(0, 400);
            pnlFooter.Name = "pnlFooter";
            pnlFooter.Size = new Size(350, 50);
            pnlFooter.TabIndex = 2;
            // 
            // btnSave
            // 
            btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSave.Location = new Point(259, 12);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(85, 30);
            btnSave.TabIndex = 0;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            // 
            // TemplateParametersEditView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(splitContainer);
            Controls.Add(pnlTemplateMetadata);
            Controls.Add(pnlFooter);
            Name = "TemplateParametersEditView";
            Size = new Size(350, 450);
            pnlTemplateMetadata.ResumeLayout(false);
            pnlTemplateMetadata.PerformLayout();
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            pnlParameterButtons.ResumeLayout(false);
            pnlFooter.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlTemplateMetadata;
        private TextBox txtEditDescription;
        private Label lblEditDescription;
        private TextBox txtEditDisplayName;
        private Label lblEditDisplayName;
        private TextBox txtEditTemplateId;
        private Label lblEditTemplateId;
        private SplitContainer splitContainer;
        private ListBox lstParameters;
        private Panel pnlParameterButtons;
        private Button btnMoveDown;
        private Button btnMoveUp;
        private Button btnRemoveParameter;
        private Button btnAddParameter;
        private TemplateParameterEditView parameterEditView;
        private Panel pnlFooter;
        private Button btnSave;
    }
}
