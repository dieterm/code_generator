using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Presentation.WinForms.Views.Domains
{
    partial class EntityEditViewFieldEditView
    {
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        private void InitializeComponent()
        {
            lblTitle = new Label();
            txtPropertyName = new SingleLineTextField();
            cmbControlType = new ComboboxField();
            txtLabel = new SingleLineTextField();
            txtTooltip = new SingleLineTextField();
            txtPlaceholder = new SingleLineTextField();
            chkIsReadOnly = new BooleanField();
            chkIsRequired = new BooleanField();
            numDisplayOrder = new IntegerField();
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
            lblTitle.Size = new Size(126, 21);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Field Properties";
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel.AutoSize = true;
            tableLayoutPanel.ColumnCount = 1;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.Controls.Add(txtPropertyName, 0, 0);
            tableLayoutPanel.Controls.Add(cmbControlType, 0, 1);
            tableLayoutPanel.Controls.Add(txtLabel, 0, 2);
            tableLayoutPanel.Controls.Add(txtTooltip, 0, 3);
            tableLayoutPanel.Controls.Add(txtPlaceholder, 0, 4);
            tableLayoutPanel.Controls.Add(chkIsReadOnly, 0, 5);
            tableLayoutPanel.Controls.Add(chkIsRequired, 0, 6);
            tableLayoutPanel.Controls.Add(numDisplayOrder, 0, 7);
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
            tableLayoutPanel.Size = new Size(380, 420);
            tableLayoutPanel.TabIndex = 1;
            // 
            // txtPropertyName
            // 
            txtPropertyName.Dock = DockStyle.Top;
            txtPropertyName.Label = "Property Name:";
            txtPropertyName.Location = new Point(3, 3);
            txtPropertyName.Name = "txtPropertyName";
            txtPropertyName.Size = new Size(374, 50);
            txtPropertyName.TabIndex = 0;
            // 
            // cmbControlType
            // 
            cmbControlType.Dock = DockStyle.Top;
            cmbControlType.Label = "Control Type:";
            cmbControlType.Location = new Point(3, 59);
            cmbControlType.Name = "cmbControlType";
            cmbControlType.Size = new Size(374, 50);
            cmbControlType.TabIndex = 1;
            // 
            // txtLabel
            // 
            txtLabel.Dock = DockStyle.Top;
            txtLabel.Label = "Label:";
            txtLabel.Location = new Point(3, 115);
            txtLabel.Name = "txtLabel";
            txtLabel.Size = new Size(374, 50);
            txtLabel.TabIndex = 2;
            // 
            // txtTooltip
            // 
            txtTooltip.Dock = DockStyle.Top;
            txtTooltip.Label = "Tooltip:";
            txtTooltip.Location = new Point(3, 171);
            txtTooltip.Name = "txtTooltip";
            txtTooltip.Size = new Size(374, 50);
            txtTooltip.TabIndex = 3;
            // 
            // txtPlaceholder
            // 
            txtPlaceholder.Dock = DockStyle.Top;
            txtPlaceholder.Label = "Placeholder:";
            txtPlaceholder.Location = new Point(3, 227);
            txtPlaceholder.Name = "txtPlaceholder";
            txtPlaceholder.Size = new Size(374, 50);
            txtPlaceholder.TabIndex = 4;
            // 
            // chkIsReadOnly
            // 
            chkIsReadOnly.Dock = DockStyle.Top;
            chkIsReadOnly.Label = "Read Only:";
            chkIsReadOnly.Location = new Point(3, 283);
            chkIsReadOnly.Name = "chkIsReadOnly";
            chkIsReadOnly.Size = new Size(374, 30);
            chkIsReadOnly.TabIndex = 5;
            // 
            // chkIsRequired
            // 
            chkIsRequired.Dock = DockStyle.Top;
            chkIsRequired.Label = "Required:";
            chkIsRequired.Location = new Point(3, 319);
            chkIsRequired.Name = "chkIsRequired";
            chkIsRequired.Size = new Size(374, 30);
            chkIsRequired.TabIndex = 6;
            // 
            // numDisplayOrder
            // 
            numDisplayOrder.Dock = DockStyle.Top;
            numDisplayOrder.Label = "Display Order:";
            numDisplayOrder.Location = new Point(3, 355);
            numDisplayOrder.Name = "numDisplayOrder";
            numDisplayOrder.Size = new Size(374, 50);
            numDisplayOrder.TabIndex = 7;
            // 
            // EntityEditViewFieldEditView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel);
            Controls.Add(lblTitle);
            Name = "EntityEditViewFieldEditView";
            Padding = new Padding(10);
            Size = new Size(400, 480);
            tableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private TableLayoutPanel tableLayoutPanel;
        private SingleLineTextField txtPropertyName;
        private ComboboxField cmbControlType;
        private SingleLineTextField txtLabel;
        private SingleLineTextField txtTooltip;
        private SingleLineTextField txtPlaceholder;
        private BooleanField chkIsReadOnly;
        private BooleanField chkIsRequired;
        private IntegerField numDisplayOrder;
    }
}
