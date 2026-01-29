using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Presentation.WinForms.Views.Domains
{
    partial class EntityListViewColumnEditView
    {
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        private void InitializeComponent()
        {
            lblTitle = new Label();
            txtPropertyPath = new SingleLineTextField();
            txtHeaderText = new SingleLineTextField();
            cmbColumnType = new ComboboxField();
            numWidth = new IntegerField();
            numDisplayOrder = new IntegerField();
            txtFormatString = new SingleLineTextField();
            chkIsVisible = new BooleanField();
            chkIsSortable = new BooleanField();
            chkIsFilterable = new BooleanField();
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
            lblTitle.Size = new Size(144, 21);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Column Properties";
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel.AutoSize = true;
            tableLayoutPanel.ColumnCount = 1;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.Controls.Add(txtPropertyPath, 0, 0);
            tableLayoutPanel.Controls.Add(txtHeaderText, 0, 1);
            tableLayoutPanel.Controls.Add(cmbColumnType, 0, 2);
            tableLayoutPanel.Controls.Add(numWidth, 0, 3);
            tableLayoutPanel.Controls.Add(numDisplayOrder, 0, 4);
            tableLayoutPanel.Controls.Add(txtFormatString, 0, 5);
            tableLayoutPanel.Controls.Add(chkIsVisible, 0, 6);
            tableLayoutPanel.Controls.Add(chkIsSortable, 0, 7);
            tableLayoutPanel.Controls.Add(chkIsFilterable, 0, 8);
            tableLayoutPanel.Location = new Point(10, 40);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 9;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.Size = new Size(380, 480);
            tableLayoutPanel.TabIndex = 1;
            // 
            // txtPropertyPath
            // 
            txtPropertyPath.Dock = DockStyle.Top;
            txtPropertyPath.Label = "Property Path:";
            txtPropertyPath.Location = new Point(3, 3);
            txtPropertyPath.Name = "txtPropertyPath";
            txtPropertyPath.Size = new Size(374, 50);
            txtPropertyPath.TabIndex = 0;
            // 
            // txtHeaderText
            // 
            txtHeaderText.Dock = DockStyle.Top;
            txtHeaderText.Label = "Header Text:";
            txtHeaderText.Location = new Point(3, 59);
            txtHeaderText.Name = "txtHeaderText";
            txtHeaderText.Size = new Size(374, 50);
            txtHeaderText.TabIndex = 1;
            // 
            // cmbColumnType
            // 
            cmbColumnType.Dock = DockStyle.Top;
            cmbColumnType.Label = "Column Type:";
            cmbColumnType.Location = new Point(3, 115);
            cmbColumnType.Name = "cmbColumnType";
            cmbColumnType.Size = new Size(374, 50);
            cmbColumnType.TabIndex = 2;
            // 
            // numWidth
            // 
            numWidth.Dock = DockStyle.Top;
            numWidth.Label = "Width:";
            numWidth.Location = new Point(3, 171);
            numWidth.Name = "numWidth";
            numWidth.Size = new Size(374, 50);
            numWidth.TabIndex = 3;
            // 
            // numDisplayOrder
            // 
            numDisplayOrder.Dock = DockStyle.Top;
            numDisplayOrder.Label = "Display Order:";
            numDisplayOrder.Location = new Point(3, 227);
            numDisplayOrder.Name = "numDisplayOrder";
            numDisplayOrder.Size = new Size(374, 50);
            numDisplayOrder.TabIndex = 4;
            // 
            // txtFormatString
            // 
            txtFormatString.Dock = DockStyle.Top;
            txtFormatString.Label = "Format String:";
            txtFormatString.Location = new Point(3, 283);
            txtFormatString.Name = "txtFormatString";
            txtFormatString.Size = new Size(374, 50);
            txtFormatString.TabIndex = 5;
            // 
            // chkIsVisible
            // 
            chkIsVisible.Dock = DockStyle.Top;
            chkIsVisible.Label = "Visible:";
            chkIsVisible.Location = new Point(3, 339);
            chkIsVisible.Name = "chkIsVisible";
            chkIsVisible.Size = new Size(374, 30);
            chkIsVisible.TabIndex = 6;
            // 
            // chkIsSortable
            // 
            chkIsSortable.Dock = DockStyle.Top;
            chkIsSortable.Label = "Sortable:";
            chkIsSortable.Location = new Point(3, 375);
            chkIsSortable.Name = "chkIsSortable";
            chkIsSortable.Size = new Size(374, 30);
            chkIsSortable.TabIndex = 7;
            // 
            // chkIsFilterable
            // 
            chkIsFilterable.Dock = DockStyle.Top;
            chkIsFilterable.Label = "Filterable:";
            chkIsFilterable.Location = new Point(3, 411);
            chkIsFilterable.Name = "chkIsFilterable";
            chkIsFilterable.Size = new Size(374, 30);
            chkIsFilterable.TabIndex = 8;
            // 
            // EntityListViewColumnEditView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel);
            Controls.Add(lblTitle);
            Name = "EntityListViewColumnEditView";
            Padding = new Padding(10);
            Size = new Size(400, 540);
            tableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private TableLayoutPanel tableLayoutPanel;
        private SingleLineTextField txtPropertyPath;
        private SingleLineTextField txtHeaderText;
        private ComboboxField cmbColumnType;
        private IntegerField numWidth;
        private IntegerField numDisplayOrder;
        private SingleLineTextField txtFormatString;
        private BooleanField chkIsVisible;
        private BooleanField chkIsSortable;
        private BooleanField chkIsFilterable;
    }
}
