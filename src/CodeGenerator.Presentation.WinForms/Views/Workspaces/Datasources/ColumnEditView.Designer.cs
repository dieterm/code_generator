using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Presentation.WinForms.Views
{
    partial class ColumnEditView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblTitle = new Label();
            grpGeneral = new GroupBox();
            tableLayoutGeneral = new TableLayoutPanel();
            txtName = new SingleLineTextField();
            cbxDataType = new ComboboxField();
            txtMaxLength = new IntegerField();
            txtPrecision = new IntegerField();
            txtScale = new IntegerField();
            txtAllowedValues = new SingleLineTextField();
            txtDefaultValue = new SingleLineTextField();
            grpOptions = new GroupBox();
            flowLayoutOptions = new FlowLayoutPanel();
            chkNullable = new BooleanField();
            chkPrimaryKey = new BooleanField();
            chkAutoIncrement = new BooleanField();
            grpGeneral.SuspendLayout();
            tableLayoutGeneral.SuspendLayout();
            grpOptions.SuspendLayout();
            flowLayoutOptions.SuspendLayout();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTitle.Location = new Point(10, 10);
            lblTitle.Name = "lblTitle";
            lblTitle.Padding = new Padding(0, 0, 0, 10);
            lblTitle.Size = new Size(127, 31);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Column Details";
            // 
            // grpGeneral
            // 
            grpGeneral.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpGeneral.Controls.Add(tableLayoutGeneral);
            grpGeneral.Location = new Point(10, 44);
            grpGeneral.Name = "grpGeneral";
            grpGeneral.Size = new Size(480, 380);
            grpGeneral.TabIndex = 1;
            grpGeneral.TabStop = false;
            grpGeneral.Text = "General";
            // 
            // tableLayoutGeneral
            // 
            tableLayoutGeneral.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutGeneral.AutoSize = true;
            tableLayoutGeneral.ColumnCount = 1;
            tableLayoutGeneral.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutGeneral.Controls.Add(txtName, 0, 0);
            tableLayoutGeneral.Controls.Add(cbxDataType, 0, 1);
            tableLayoutGeneral.Controls.Add(txtMaxLength, 0, 2);
            tableLayoutGeneral.Controls.Add(txtPrecision, 0, 3);
            tableLayoutGeneral.Controls.Add(txtScale, 0, 4);
            tableLayoutGeneral.Controls.Add(txtAllowedValues, 0, 5);
            tableLayoutGeneral.Controls.Add(txtDefaultValue, 0, 6);
            tableLayoutGeneral.Location = new Point(6, 22);
            tableLayoutGeneral.Name = "tableLayoutGeneral";
            tableLayoutGeneral.RowCount = 7;
            tableLayoutGeneral.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            tableLayoutGeneral.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            tableLayoutGeneral.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            tableLayoutGeneral.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            tableLayoutGeneral.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            tableLayoutGeneral.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            tableLayoutGeneral.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            tableLayoutGeneral.Size = new Size(468, 350);
            tableLayoutGeneral.TabIndex = 0;
            // 
            // txtName
            // 
            txtName.Dock = DockStyle.Top;
            txtName.ErrorMessageVisible = true;
            txtName.Label = "Column Name:";
            txtName.Location = new Point(3, 3);
            txtName.Name = "txtName";
            txtName.Size = new Size(462, 44);
            txtName.TabIndex = 0;
            // 
            // cbxDataType
            // 
            cbxDataType.Dock = DockStyle.Top;
            cbxDataType.ErrorMessageVisible = true;
            cbxDataType.Label = "Data Type:";
            cbxDataType.Location = new Point(3, 53);
            cbxDataType.Name = "cbxDataType";
            cbxDataType.Size = new Size(462, 44);
            cbxDataType.TabIndex = 1;
            // 
            // txtMaxLength
            // 
            txtMaxLength.Dock = DockStyle.Top;
            txtMaxLength.Label = "Max Length:";
            txtMaxLength.Location = new Point(3, 103);
            txtMaxLength.Name = "txtMaxLength";
            txtMaxLength.Size = new Size(462, 44);
            txtMaxLength.TabIndex = 2;
            // 
            // txtPrecision
            // 
            txtPrecision.Dock = DockStyle.Top;
            txtPrecision.Label = "Precision:";
            txtPrecision.Location = new Point(3, 153);
            txtPrecision.Name = "txtPrecision";
            txtPrecision.Size = new Size(462, 44);
            txtPrecision.TabIndex = 3;
            // 
            // txtScale
            // 
            txtScale.Dock = DockStyle.Top;
            txtScale.Label = "Scale:";
            txtScale.Location = new Point(3, 203);
            txtScale.Name = "txtScale";
            txtScale.Size = new Size(462, 44);
            txtScale.TabIndex = 4;
            // 
            // txtAllowedValues
            // 
            txtAllowedValues.Dock = DockStyle.Top;
            txtAllowedValues.ErrorMessageVisible = true;
            txtAllowedValues.Label = "Allowed Values:";
            txtAllowedValues.Location = new Point(3, 253);
            txtAllowedValues.Name = "txtAllowedValues";
            txtAllowedValues.Size = new Size(462, 44);
            txtAllowedValues.TabIndex = 5;
            // 
            // txtDefaultValue
            // 
            txtDefaultValue.Dock = DockStyle.Top;
            txtDefaultValue.ErrorMessageVisible = true;
            txtDefaultValue.Label = "Default Value:";
            txtDefaultValue.Location = new Point(3, 303);
            txtDefaultValue.Name = "txtDefaultValue";
            txtDefaultValue.Size = new Size(462, 44);
            txtDefaultValue.TabIndex = 6;
            // 
            // grpOptions
            // 
            grpOptions.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpOptions.Controls.Add(flowLayoutOptions);
            grpOptions.Location = new Point(10, 430);
            grpOptions.Name = "grpOptions";
            grpOptions.Size = new Size(480, 135);
            grpOptions.TabIndex = 2;
            grpOptions.TabStop = false;
            grpOptions.Text = "Options";
            // 
            // flowLayoutOptions
            // 
            flowLayoutOptions.Controls.Add(chkNullable);
            flowLayoutOptions.Controls.Add(chkPrimaryKey);
            flowLayoutOptions.Controls.Add(chkAutoIncrement);
            flowLayoutOptions.Dock = DockStyle.Fill;
            flowLayoutOptions.FlowDirection = FlowDirection.TopDown;
            flowLayoutOptions.Location = new Point(3, 19);
            flowLayoutOptions.Name = "flowLayoutOptions";
            flowLayoutOptions.Size = new Size(474, 113);
            flowLayoutOptions.TabIndex = 0;
            // 
            // chkNullable
            // 
            chkNullable.Dock = DockStyle.Top;
            chkNullable.Label = "Nullable";
            chkNullable.Location = new Point(3, 3);
            chkNullable.Name = "chkNullable";
            chkNullable.Size = new Size(318, 31);
            chkNullable.TabIndex = 0;
            // 
            // chkPrimaryKey
            // 
            chkPrimaryKey.Dock = DockStyle.Top;
            chkPrimaryKey.Label = "Primary Key";
            chkPrimaryKey.Location = new Point(3, 40);
            chkPrimaryKey.Name = "chkPrimaryKey";
            chkPrimaryKey.Size = new Size(318, 28);
            chkPrimaryKey.TabIndex = 1;
            // 
            // chkAutoIncrement
            // 
            chkAutoIncrement.Label = "Auto Increment";
            chkAutoIncrement.Location = new Point(3, 74);
            chkAutoIncrement.Name = "chkAutoIncrement";
            chkAutoIncrement.Size = new Size(318, 28);
            chkAutoIncrement.TabIndex = 2;
            // 
            // ColumnEditView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            Controls.Add(grpOptions);
            Controls.Add(grpGeneral);
            Controls.Add(lblTitle);
            Name = "ColumnEditView";
            Padding = new Padding(10);
            Size = new Size(500, 576);
            grpGeneral.ResumeLayout(false);
            grpGeneral.PerformLayout();
            tableLayoutGeneral.ResumeLayout(false);
            grpOptions.ResumeLayout(false);
            flowLayoutOptions.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private GroupBox grpGeneral;
        private TableLayoutPanel tableLayoutGeneral;
        private SingleLineTextField txtName;
        private ComboboxField cbxDataType;
        private IntegerField txtMaxLength;
        private IntegerField txtPrecision;
        private IntegerField txtScale;
        private SingleLineTextField txtAllowedValues;
        private SingleLineTextField txtDefaultValue;
        private GroupBox grpOptions;
        private FlowLayoutPanel flowLayoutOptions;
        private BooleanField chkNullable;
        private BooleanField chkPrimaryKey;
        private BooleanField chkAutoIncrement;
    }
}
