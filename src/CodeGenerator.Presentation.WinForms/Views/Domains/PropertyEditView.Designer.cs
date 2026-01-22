using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Presentation.WinForms.Views.Domains
{
    partial class PropertyEditView
    {
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        private void InitializeComponent()
        {
            lblTitle = new Label();
            txtName = new SingleLineTextField();
            cmbDataType = new ComboboxField();
            chkIsNullable = new BooleanField();
            numMaxLength = new IntegerField();
            numPrecision = new IntegerField();
            numScale = new IntegerField();
            txtDescription = new SingleLineTextField();
            txtExampleValue = new SingleLineTextField();
            tableLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblTitle.Location = new Point(10, 10);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(152, 21);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Property Properties";
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel.AutoSize = true;
            tableLayoutPanel.ColumnCount = 1;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.Controls.Add(txtName, 0, 0);
            tableLayoutPanel.Controls.Add(cmbDataType, 0, 1);
            tableLayoutPanel.Controls.Add(chkIsNullable, 0, 2);
            tableLayoutPanel.Controls.Add(numMaxLength, 0, 3);
            tableLayoutPanel.Controls.Add(numPrecision, 0, 4);
            tableLayoutPanel.Controls.Add(numScale, 0, 5);
            tableLayoutPanel.Controls.Add(txtDescription, 0, 6);
            tableLayoutPanel.Controls.Add(txtExampleValue, 0, 7);
            tableLayoutPanel.Location = new Point(10, 40);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 8;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.Size = new Size(380, 450);
            tableLayoutPanel.TabIndex = 1;
            // 
            // txtName
            // 
            txtName.Dock = DockStyle.Top;
            txtName.Label = "Property Name:";
            txtName.Location = new Point(3, 3);
            txtName.Name = "txtName";
            txtName.Size = new Size(374, 50);
            txtName.TabIndex = 0;
            // 
            // cmbDataType
            // 
            cmbDataType.Dock = DockStyle.Top;
            cmbDataType.Label = "Data Type:";
            cmbDataType.Location = new Point(3, 59);
            cmbDataType.Name = "cmbDataType";
            cmbDataType.Size = new Size(374, 50);
            cmbDataType.TabIndex = 1;
            // 
            // chkIsNullable
            // 
            chkIsNullable.Dock = DockStyle.Top;
            chkIsNullable.Label = "Is Nullable:";
            chkIsNullable.Location = new Point(3, 115);
            chkIsNullable.Name = "chkIsNullable";
            chkIsNullable.Size = new Size(374, 30);
            chkIsNullable.TabIndex = 2;
            // 
            // numMaxLength
            // 
            numMaxLength.Dock = DockStyle.Top;
            numMaxLength.Label = "Max Length:";
            numMaxLength.Location = new Point(3, 151);
            numMaxLength.Name = "numMaxLength";
            numMaxLength.Size = new Size(374, 50);
            numMaxLength.TabIndex = 3;
            // 
            // numPrecision
            // 
            numPrecision.Dock = DockStyle.Top;
            numPrecision.Label = "Precision:";
            numPrecision.Location = new Point(3, 207);
            numPrecision.Name = "numPrecision";
            numPrecision.Size = new Size(374, 50);
            numPrecision.TabIndex = 4;
            // 
            // numScale
            // 
            numScale.Dock = DockStyle.Top;
            numScale.Label = "Scale:";
            numScale.Location = new Point(3, 263);
            numScale.Name = "numScale";
            numScale.Size = new Size(374, 50);
            numScale.TabIndex = 5;
            // 
            // txtDescription
            // 
            txtDescription.Dock = DockStyle.Top;
            txtDescription.Label = "Description:";
            txtDescription.Location = new Point(3, 319);
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(374, 50);
            txtDescription.TabIndex = 6;
            // 
            // txtExampleValue
            // 
            txtExampleValue.Dock = DockStyle.Top;
            txtExampleValue.Label = "Example Value:";
            txtExampleValue.Location = new Point(3, 375);
            txtExampleValue.Name = "txtExampleValue";
            txtExampleValue.Size = new Size(374, 50);
            txtExampleValue.TabIndex = 7;
            // 
            // PropertyEditView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel);
            Controls.Add(lblTitle);
            Name = "PropertyEditView";
            Padding = new Padding(10);
            Size = new Size(400, 510);
            tableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private TableLayoutPanel tableLayoutPanel;
        private SingleLineTextField txtName;
        private ComboboxField cmbDataType;
        private BooleanField chkIsNullable;
        private IntegerField numMaxLength;
        private IntegerField numPrecision;
        private IntegerField numScale;
        private SingleLineTextField txtDescription;
        private SingleLineTextField txtExampleValue;
    }
}
