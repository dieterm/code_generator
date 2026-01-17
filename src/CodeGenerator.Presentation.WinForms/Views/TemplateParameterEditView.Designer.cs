namespace CodeGenerator.Presentation.WinForms.Views
{
    partial class TemplateParameterEditView
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
            pnlMain = new Panel();
            pnlDatasourceOptions = new Panel();
            numTableDataMaxRows = new NumericUpDown();
            lblTableDataMaxRows = new Label();
            txtTableDataFilter = new TextBox();
            lblTableDataFilter = new Label();
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
            pnlMain.SuspendLayout();
            pnlDatasourceOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numTableDataMaxRows).BeginInit();
            SuspendLayout();
            // 
            // pnlMain
            // 
            pnlMain.AutoScroll = true;
            pnlMain.Controls.Add(pnlDatasourceOptions);
            pnlMain.Controls.Add(cboType);
            pnlMain.Controls.Add(lblType);
            pnlMain.Controls.Add(txtAllowedValues);
            pnlMain.Controls.Add(lblAllowedValues);
            pnlMain.Controls.Add(txtDefaultValue);
            pnlMain.Controls.Add(lblDefaultValue);
            pnlMain.Controls.Add(chkRequired);
            pnlMain.Controls.Add(txtTooltip);
            pnlMain.Controls.Add(lblTooltip);
            pnlMain.Controls.Add(txtLabel);
            pnlMain.Controls.Add(lblLabel);
            pnlMain.Controls.Add(txtDescription);
            pnlMain.Controls.Add(lblDescription);
            pnlMain.Controls.Add(txtParameterName);
            pnlMain.Controls.Add(lblParameterName);
            pnlMain.Dock = DockStyle.Fill;
            pnlMain.Location = new Point(0, 0);
            pnlMain.Name = "pnlMain";
            pnlMain.Size = new Size(300, 280);
            pnlMain.TabIndex = 0;
            // 
            // pnlDatasourceOptions
            // 
            pnlDatasourceOptions.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlDatasourceOptions.Controls.Add(numTableDataMaxRows);
            pnlDatasourceOptions.Controls.Add(lblTableDataMaxRows);
            pnlDatasourceOptions.Controls.Add(txtTableDataFilter);
            pnlDatasourceOptions.Controls.Add(lblTableDataFilter);
            pnlDatasourceOptions.Location = new Point(0, 212);
            pnlDatasourceOptions.Name = "pnlDatasourceOptions";
            pnlDatasourceOptions.Size = new Size(300, 55);
            pnlDatasourceOptions.TabIndex = 15;
            pnlDatasourceOptions.Visible = false;
            // 
            // numTableDataMaxRows
            // 
            numTableDataMaxRows.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            numTableDataMaxRows.Location = new Point(90, 29);
            numTableDataMaxRows.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            numTableDataMaxRows.Name = "numTableDataMaxRows";
            numTableDataMaxRows.Size = new Size(205, 23);
            numTableDataMaxRows.TabIndex = 3;
            // 
            // lblTableDataMaxRows
            // 
            lblTableDataMaxRows.AutoSize = true;
            lblTableDataMaxRows.Location = new Point(5, 31);
            lblTableDataMaxRows.Name = "lblTableDataMaxRows";
            lblTableDataMaxRows.Size = new Size(61, 15);
            lblTableDataMaxRows.TabIndex = 2;
            lblTableDataMaxRows.Text = "Max Rows:";
            // 
            // txtTableDataFilter
            // 
            txtTableDataFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtTableDataFilter.Location = new Point(90, 3);
            txtTableDataFilter.Name = "txtTableDataFilter";
            txtTableDataFilter.Size = new Size(205, 23);
            txtTableDataFilter.TabIndex = 1;
            // 
            // lblTableDataFilter
            // 
            lblTableDataFilter.AutoSize = true;
            lblTableDataFilter.Location = new Point(5, 6);
            lblTableDataFilter.Name = "lblTableDataFilter";
            lblTableDataFilter.Size = new Size(79, 15);
            lblTableDataFilter.TabIndex = 0;
            lblTableDataFilter.Text = "WHERE Filter:";
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
            // TemplateParameterEditView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pnlMain);
            Name = "TemplateParameterEditView";
            Size = new Size(300, 280);
            pnlMain.ResumeLayout(false);
            pnlMain.PerformLayout();
            pnlDatasourceOptions.ResumeLayout(false);
            pnlDatasourceOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numTableDataMaxRows).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlMain;
        private Panel pnlDatasourceOptions;
        private NumericUpDown numTableDataMaxRows;
        private Label lblTableDataMaxRows;
        private TextBox txtTableDataFilter;
        private Label lblTableDataFilter;
        private ComboBox cboType;
        private Label lblType;
        private TextBox txtAllowedValues;
        private Label lblAllowedValues;
        private TextBox txtDefaultValue;
        private Label lblDefaultValue;
        private CheckBox chkRequired;
        private TextBox txtTooltip;
        private Label lblTooltip;
        private TextBox txtLabel;
        private Label lblLabel;
        private TextBox txtDescription;
        private Label lblDescription;
        private TextBox txtParameterName;
        private Label lblParameterName;
    }
}
