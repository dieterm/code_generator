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
            lblTemplateName = new Label();
            lblTemplateDescription = new Label();
            pnlParameters = new Panel();
            btnExecute = new Button();
            pnlHeader = new Panel();
            btnToggleEditMode = new Button();
            pnlFooter = new Panel();
            btnSave = new Button();
            pnlEditMode = new Panel();
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
            pnlParameterDetails = new Panel();
            cboType = new ComboBox();
            lblType = new Label();
            txtAllowedValues = new TextBox();
            lblAllowedValues = new Label();
            txtDefaultValue = new TextBox();
            lblDefaultValue = new Label();
            chkRequired = new CheckBox();
            txtTooltip = new TextBox();
            lblTooltip = new Label();
            txtLabel = new TextBox();
            lblLabel = new Label();
            txtDescription = new TextBox();
            lblDescription = new Label();
            txtParameterName = new TextBox();
            lblParameterName = new Label();
            pnlHeader.SuspendLayout();
            pnlFooter.SuspendLayout();
            pnlEditMode.SuspendLayout();
            pnlTemplateMetadata.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            pnlParameterButtons.SuspendLayout();
            pnlParameterDetails.SuspendLayout();
            SuspendLayout();
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
            lblTemplateDescription.Size = new Size(220, 35);
            lblTemplateDescription.TabIndex = 1;
            lblTemplateDescription.Text = "Template description goes here...";
            // 
            // pnlParameters
            // 
            pnlParameters.AutoScroll = true;
            pnlParameters.Dock = DockStyle.Fill;
            pnlParameters.Location = new Point(0, 70);
            pnlParameters.Name = "pnlParameters";
            pnlParameters.Padding = new Padding(3);
            pnlParameters.Size = new Size(300, 230);
            pnlParameters.TabIndex = 2;
            // 
            // btnExecute
            // 
            btnExecute.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnExecute.Location = new Point(209, 7);
            btnExecute.Name = "btnExecute";
            btnExecute.Size = new Size(85, 30);
            btnExecute.TabIndex = 3;
            btnExecute.Text = "Execute";
            btnExecute.UseVisualStyleBackColor = true;
            // 
            // pnlHeader
            // 
            pnlHeader.Controls.Add(btnToggleEditMode);
            pnlHeader.Controls.Add(lblTemplateName);
            pnlHeader.Controls.Add(lblTemplateDescription);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(300, 70);
            pnlHeader.TabIndex = 4;
            // 
            // btnToggleEditMode
            // 
            btnToggleEditMode.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnToggleEditMode.Location = new Point(229, 5);
            btnToggleEditMode.Name = "btnToggleEditMode";
            btnToggleEditMode.Size = new Size(65, 23);
            btnToggleEditMode.TabIndex = 2;
            btnToggleEditMode.Text = "Edit";
            btnToggleEditMode.UseVisualStyleBackColor = true;
            // 
            // pnlFooter
            // 
            pnlFooter.Controls.Add(btnSave);
            pnlFooter.Controls.Add(btnExecute);
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Location = new Point(0, 300);
            pnlFooter.Name = "pnlFooter";
            pnlFooter.Size = new Size(300, 50);
            pnlFooter.TabIndex = 5;
            // 
            // btnSave
            // 
            btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSave.Location = new Point(118, 7);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(85, 30);
            btnSave.TabIndex = 4;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Visible = false;
            // 
            // pnlEditMode
            // 
            pnlEditMode.Controls.Add(splitContainer);
            pnlEditMode.Controls.Add(pnlTemplateMetadata);
            pnlEditMode.Dock = DockStyle.Fill;
            pnlEditMode.Location = new Point(0, 70);
            pnlEditMode.Name = "pnlEditMode";
            pnlEditMode.Size = new Size(300, 230);
            pnlEditMode.TabIndex = 6;
            pnlEditMode.Visible = false;
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
            pnlTemplateMetadata.Size = new Size(300, 81);
            pnlTemplateMetadata.TabIndex = 1;
            // 
            // txtEditDescription
            // 
            txtEditDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtEditDescription.Location = new Point(90, 55);
            txtEditDescription.Name = "txtEditDescription";
            txtEditDescription.Size = new Size(205, 23);
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
            txtEditDisplayName.Size = new Size(205, 23);
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
            txtEditTemplateId.Size = new Size(205, 23);
            txtEditTemplateId.TabIndex = 1;
            // 
            // lblEditTemplateId
            // 
            lblEditTemplateId.AutoSize = true;
            lblEditTemplateId.Location = new Point(5, 6);
            lblEditTemplateId.Name = "lblEditTemplateId";
            lblEditTemplateId.Size = new Size(72, 15);
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
            splitContainer.Panel2.Controls.Add(pnlParameterDetails);
            splitContainer.Size = new Size(300, 175);
            splitContainer.SplitterDistance = 70;
            splitContainer.TabIndex = 0;
            // 
            // lstParameters
            // 
            lstParameters.Dock = DockStyle.Fill;
            lstParameters.FormattingEnabled = true;
            lstParameters.ItemHeight = 15;
            lstParameters.Location = new Point(0, 0);
            lstParameters.Name = "lstParameters";
            lstParameters.Size = new Size(200, 70);
            lstParameters.TabIndex = 0;
            // 
            // pnlParameterButtons
            // 
            pnlParameterButtons.Controls.Add(btnMoveDown);
            pnlParameterButtons.Controls.Add(btnMoveUp);
            pnlParameterButtons.Controls.Add(btnRemoveParameter);
            pnlParameterButtons.Controls.Add(btnAddParameter);
            pnlParameterButtons.Dock = DockStyle.Right;
            pnlParameterButtons.Location = new Point(200, 0);
            pnlParameterButtons.Name = "pnlParameterButtons";
            pnlParameterButtons.Size = new Size(100, 70);
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
            // pnlParameterDetails
            // 
            pnlParameterDetails.AutoScroll = true;
            pnlParameterDetails.Controls.Add(cboType);
            pnlParameterDetails.Controls.Add(lblType);
            pnlParameterDetails.Controls.Add(txtAllowedValues);
            pnlParameterDetails.Controls.Add(lblAllowedValues);
            pnlParameterDetails.Controls.Add(txtDefaultValue);
            pnlParameterDetails.Controls.Add(lblDefaultValue);
            pnlParameterDetails.Controls.Add(chkRequired);
            pnlParameterDetails.Controls.Add(txtTooltip);
            pnlParameterDetails.Controls.Add(lblTooltip);
            pnlParameterDetails.Controls.Add(txtLabel);
            pnlParameterDetails.Controls.Add(lblLabel);
            pnlParameterDetails.Controls.Add(txtDescription);
            pnlParameterDetails.Controls.Add(lblDescription);
            pnlParameterDetails.Controls.Add(txtParameterName);
            pnlParameterDetails.Controls.Add(lblParameterName);
            pnlParameterDetails.Dock = DockStyle.Fill;
            pnlParameterDetails.Location = new Point(0, 0);
            pnlParameterDetails.Name = "pnlParameterDetails";
            pnlParameterDetails.Size = new Size(300, 101);
            pnlParameterDetails.TabIndex = 0;
            // 
            // cboType
            // 
            cboType.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cboType.DropDownStyle = ComboBoxStyle.DropDownList;
            cboType.FormattingEnabled = true;
            cboType.Location = new Point(90, 55);
            cboType.Name = "cboType";
            cboType.Size = new Size(205, 23);
            cboType.TabIndex = 14;
            // 
            // lblType
            // 
            lblType.AutoSize = true;
            lblType.Location = new Point(5, 58);
            lblType.Name = "lblType";
            lblType.Size = new Size(35, 15);
            lblType.TabIndex = 13;
            lblType.Text = "Type:";
            // 
            // txtAllowedValues
            // 
            txtAllowedValues.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtAllowedValues.Location = new Point(90, 186);
            txtAllowedValues.Name = "txtAllowedValues";
            txtAllowedValues.Size = new Size(205, 23);
            txtAllowedValues.TabIndex = 12;
            // 
            // lblAllowedValues
            // 
            lblAllowedValues.AutoSize = true;
            lblAllowedValues.Location = new Point(5, 189);
            lblAllowedValues.Name = "lblAllowedValues";
            lblAllowedValues.Size = new Size(67, 15);
            lblAllowedValues.TabIndex = 11;
            lblAllowedValues.Text = "Allowed (;):";
            // 
            // txtDefaultValue
            // 
            txtDefaultValue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDefaultValue.Location = new Point(90, 160);
            txtDefaultValue.Name = "txtDefaultValue";
            txtDefaultValue.Size = new Size(205, 23);
            txtDefaultValue.TabIndex = 10;
            // 
            // lblDefaultValue
            // 
            lblDefaultValue.AutoSize = true;
            lblDefaultValue.Location = new Point(5, 163);
            lblDefaultValue.Name = "lblDefaultValue";
            lblDefaultValue.Size = new Size(48, 15);
            lblDefaultValue.TabIndex = 9;
            lblDefaultValue.Text = "Default:";
            // 
            // chkRequired
            // 
            chkRequired.AutoSize = true;
            chkRequired.Location = new Point(90, 81);
            chkRequired.Name = "chkRequired";
            chkRequired.Size = new Size(73, 19);
            chkRequired.TabIndex = 8;
            chkRequired.Text = "Required";
            chkRequired.UseVisualStyleBackColor = true;
            // 
            // txtTooltip
            // 
            txtTooltip.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtTooltip.Location = new Point(90, 134);
            txtTooltip.Name = "txtTooltip";
            txtTooltip.Size = new Size(205, 23);
            txtTooltip.TabIndex = 7;
            // 
            // lblTooltip
            // 
            lblTooltip.AutoSize = true;
            lblTooltip.Location = new Point(5, 137);
            lblTooltip.Name = "lblTooltip";
            lblTooltip.Size = new Size(47, 15);
            lblTooltip.TabIndex = 6;
            lblTooltip.Text = "Tooltip:";
            // 
            // txtLabel
            // 
            txtLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtLabel.Location = new Point(90, 108);
            txtLabel.Name = "txtLabel";
            txtLabel.Size = new Size(205, 23);
            txtLabel.TabIndex = 5;
            // 
            // lblLabel
            // 
            lblLabel.AutoSize = true;
            lblLabel.Location = new Point(5, 111);
            lblLabel.Name = "lblLabel";
            lblLabel.Size = new Size(38, 15);
            lblLabel.TabIndex = 4;
            lblLabel.Text = "Label:";
            // 
            // txtDescription
            // 
            txtDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDescription.Location = new Point(90, 29);
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(205, 23);
            txtDescription.TabIndex = 3;
            // 
            // lblDescription
            // 
            lblDescription.AutoSize = true;
            lblDescription.Location = new Point(5, 32);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(70, 15);
            lblDescription.TabIndex = 2;
            lblDescription.Text = "Description:";
            // 
            // txtParameterName
            // 
            txtParameterName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtParameterName.Location = new Point(90, 3);
            txtParameterName.Name = "txtParameterName";
            txtParameterName.Size = new Size(205, 23);
            txtParameterName.TabIndex = 1;
            // 
            // lblParameterName
            // 
            lblParameterName.AutoSize = true;
            lblParameterName.Location = new Point(5, 6);
            lblParameterName.Name = "lblParameterName";
            lblParameterName.Size = new Size(42, 15);
            lblParameterName.TabIndex = 0;
            lblParameterName.Text = "Name:";
            // 
            // TemplateParametersView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pnlParameters);
            Controls.Add(pnlEditMode);
            Controls.Add(pnlFooter);
            Controls.Add(pnlHeader);
            Name = "TemplateParametersView";
            Size = new Size(300, 350);
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlFooter.ResumeLayout(false);
            pnlEditMode.ResumeLayout(false);
            pnlTemplateMetadata.ResumeLayout(false);
            pnlTemplateMetadata.PerformLayout();
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            pnlParameterButtons.ResumeLayout(false);
            pnlParameterDetails.ResumeLayout(false);
            pnlParameterDetails.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblTemplateName;
        private System.Windows.Forms.Label lblTemplateDescription;
        private System.Windows.Forms.Panel pnlParameters;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Panel pnlFooter;
        private System.Windows.Forms.Button btnToggleEditMode;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Panel pnlEditMode;
        private System.Windows.Forms.Panel pnlTemplateMetadata;
        private System.Windows.Forms.TextBox txtEditDescription;
        private System.Windows.Forms.Label lblEditDescription;
        private System.Windows.Forms.TextBox txtEditDisplayName;
        private System.Windows.Forms.Label lblEditDisplayName;
        private System.Windows.Forms.TextBox txtEditTemplateId;
        private System.Windows.Forms.Label lblEditTemplateId;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.ListBox lstParameters;
        private System.Windows.Forms.Panel pnlParameterButtons;
        private System.Windows.Forms.Button btnMoveDown;
        private System.Windows.Forms.Button btnMoveUp;
        private System.Windows.Forms.Button btnRemoveParameter;
        private System.Windows.Forms.Button btnAddParameter;
        private System.Windows.Forms.Panel pnlParameterDetails;
        private System.Windows.Forms.TextBox txtParameterName;
        private System.Windows.Forms.Label lblParameterName;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TextBox txtLabel;
        private System.Windows.Forms.Label lblLabel;
        private System.Windows.Forms.TextBox txtTooltip;
        private System.Windows.Forms.Label lblTooltip;
        private System.Windows.Forms.CheckBox chkRequired;
        private System.Windows.Forms.TextBox txtDefaultValue;
        private System.Windows.Forms.Label lblDefaultValue;
        private System.Windows.Forms.TextBox txtAllowedValues;
        private System.Windows.Forms.Label lblAllowedValues;
        private System.Windows.Forms.ComboBox cboType;
        private System.Windows.Forms.Label lblType;
    }
}
