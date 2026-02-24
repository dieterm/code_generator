using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Presentation.WinForms.Views.Domains
{
    partial class EntitySelectViewEditView
    {
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        private void InitializeComponent()
        {
            lblTitle = new Label();
            txtName = new SingleLineTextField();
            cmbDisplayMemberPath = new ComboboxField();
            cmbValueMemberPath = new ComboboxField();
            txtDisplayFormat = new SingleLineTextField();
            cmbSearchPropertyPath = new ComboboxField();
            cmbSortPropertyPath = new ComboboxField();
            chkSortAscending = new BooleanField();
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
            lblTitle.Size = new Size(172, 21);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Select View Properties";
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel.AutoSize = true;
            tableLayoutPanel.ColumnCount = 1;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.Controls.Add(txtName, 0, 0);
            tableLayoutPanel.Controls.Add(cmbDisplayMemberPath, 0, 1);
            tableLayoutPanel.Controls.Add(cmbValueMemberPath, 0, 2);
            tableLayoutPanel.Controls.Add(txtDisplayFormat, 0, 3);
            tableLayoutPanel.Controls.Add(cmbSearchPropertyPath, 0, 4);
            tableLayoutPanel.Controls.Add(cmbSortPropertyPath, 0, 5);
            tableLayoutPanel.Controls.Add(chkSortAscending, 0, 6);
            tableLayoutPanel.Location = new Point(10, 40);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 7;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.Size = new Size(380, 380);
            tableLayoutPanel.TabIndex = 1;
            // 
            // txtName
            // 
            txtName.Dock = DockStyle.Top;
            txtName.Label = "View Name:";
            txtName.Location = new Point(3, 3);
            txtName.Name = "txtName";
            txtName.Size = new Size(374, 50);
            txtName.TabIndex = 0;
            // 
            // cmbDisplayMemberPath
            // 
            cmbDisplayMemberPath.Dock = DockStyle.Top;
            cmbDisplayMemberPath.Label = "Display Member:";
            cmbDisplayMemberPath.Location = new Point(3, 59);
            cmbDisplayMemberPath.Name = "cmbDisplayMemberPath";
            cmbDisplayMemberPath.Size = new Size(374, 50);
            cmbDisplayMemberPath.TabIndex = 1;
            // 
            // cmbValueMemberPath
            // 
            cmbValueMemberPath.Dock = DockStyle.Top;
            cmbValueMemberPath.Label = "Value Member:";
            cmbValueMemberPath.Location = new Point(3, 115);
            cmbValueMemberPath.Name = "cmbValueMemberPath";
            cmbValueMemberPath.Size = new Size(374, 50);
            cmbValueMemberPath.TabIndex = 2;
            // 
            // txtDisplayFormat
            // 
            txtDisplayFormat.Dock = DockStyle.Top;
            txtDisplayFormat.Label = "Display Format:";
            txtDisplayFormat.Location = new Point(3, 171);
            txtDisplayFormat.Name = "txtDisplayFormat";
            txtDisplayFormat.Size = new Size(374, 50);
            txtDisplayFormat.TabIndex = 3;
            // 
            // cmbSearchPropertyPath
            // 
            cmbSearchPropertyPath.Dock = DockStyle.Top;
            cmbSearchPropertyPath.Label = "Search Property:";
            cmbSearchPropertyPath.Location = new Point(3, 227);
            cmbSearchPropertyPath.Name = "cmbSearchPropertyPath";
            cmbSearchPropertyPath.Size = new Size(374, 50);
            cmbSearchPropertyPath.TabIndex = 4;
            // 
            // cmbSortPropertyPath
            // 
            cmbSortPropertyPath.Dock = DockStyle.Top;
            cmbSortPropertyPath.Label = "Sort Property:";
            cmbSortPropertyPath.Location = new Point(3, 283);
            cmbSortPropertyPath.Name = "cmbSortPropertyPath";
            cmbSortPropertyPath.Size = new Size(374, 50);
            cmbSortPropertyPath.TabIndex = 5;
            // 
            // chkSortAscending
            // 
            chkSortAscending.Dock = DockStyle.Top;
            chkSortAscending.Label = "Sort Ascending:";
            chkSortAscending.Location = new Point(3, 339);
            chkSortAscending.Name = "chkSortAscending";
            chkSortAscending.Size = new Size(374, 30);
            chkSortAscending.TabIndex = 6;
            // 
            // EntitySelectViewEditView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel);
            Controls.Add(lblTitle);
            Name = "EntitySelectViewEditView";
            Padding = new Padding(10);
            Size = new Size(400, 440);
            tableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private TableLayoutPanel tableLayoutPanel;
        private SingleLineTextField txtName;
        private ComboboxField cmbDisplayMemberPath;
        private ComboboxField cmbValueMemberPath;
        private SingleLineTextField txtDisplayFormat;
        private ComboboxField cmbSearchPropertyPath;
        private ComboboxField cmbSortPropertyPath;
        private BooleanField chkSortAscending;
    }
}
