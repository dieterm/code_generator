using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Presentation.WinForms.Views.Workspace.Domains.Specifications
{
    partial class DomainSpecificationEditView
    {
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        private void InitializeComponent()
        {
            lblTitle = new Label();
            txtName = new SingleLineTextField();
            txtDescription = new SingleLineTextField();
            txtCriteria = new SingleLineTextField();
            txtCategory = new SingleLineTextField();
            chkIsComposite = new BooleanField();
            chkIsReusable = new BooleanField();
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
            lblTitle.Size = new Size(200, 21);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Specification Properties";
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel.AutoScroll = true;
            tableLayoutPanel.ColumnCount = 1;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.Controls.Add(txtName, 0, 0);
            tableLayoutPanel.Controls.Add(txtDescription, 0, 1);
            tableLayoutPanel.Controls.Add(txtCriteria, 0, 2);
            tableLayoutPanel.Controls.Add(txtCategory, 0, 3);
            tableLayoutPanel.Controls.Add(chkIsComposite, 0, 4);
            tableLayoutPanel.Controls.Add(chkIsReusable, 0, 5);
            tableLayoutPanel.Location = new Point(10, 40);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 6;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.Size = new Size(480, 440);
            tableLayoutPanel.TabIndex = 1;
            // 
            // txtName
            // 
            txtName.Dock = DockStyle.Top;
            txtName.Label = "Specification Name:";
            txtName.Location = new Point(3, 3);
            txtName.Name = "txtName";
            txtName.Size = new Size(474, 50);
            txtName.TabIndex = 0;
            // 
            // txtDescription
            // 
            txtDescription.Dock = DockStyle.Top;
            txtDescription.Label = "Description:";
            txtDescription.Location = new Point(3, 59);
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(474, 50);
            txtDescription.TabIndex = 1;
            // 
            // txtCriteria
            // 
            txtCriteria.Dock = DockStyle.Top;
            txtCriteria.Label = "Criteria:";
            txtCriteria.Location = new Point(3, 115);
            txtCriteria.Name = "txtCriteria";
            txtCriteria.Size = new Size(474, 50);
            txtCriteria.TabIndex = 2;
            // 
            // txtCategory
            // 
            txtCategory.Dock = DockStyle.Top;
            txtCategory.Label = "Category:";
            txtCategory.Location = new Point(3, 171);
            txtCategory.Name = "txtCategory";
            txtCategory.Size = new Size(474, 50);
            txtCategory.TabIndex = 3;
            // 
            // chkIsComposite
            // 
            chkIsComposite.Dock = DockStyle.Top;
            chkIsComposite.Label = "Is Composite:";
            chkIsComposite.Location = new Point(3, 227);
            chkIsComposite.Name = "chkIsComposite";
            chkIsComposite.Size = new Size(474, 30);
            chkIsComposite.TabIndex = 4;
            // 
            // chkIsReusable
            // 
            chkIsReusable.Dock = DockStyle.Top;
            chkIsReusable.Label = "Is Reusable:";
            chkIsReusable.Location = new Point(3, 263);
            chkIsReusable.Name = "chkIsReusable";
            chkIsReusable.Size = new Size(474, 30);
            chkIsReusable.TabIndex = 5;
            // 
            // DomainSpecificationEditView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel);
            Controls.Add(lblTitle);
            Name = "DomainSpecificationEditView";
            Padding = new Padding(10);
            Size = new Size(500, 490);
            tableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private TableLayoutPanel tableLayoutPanel;
        private SingleLineTextField txtName;
        private SingleLineTextField txtDescription;
        private SingleLineTextField txtCriteria;
        private SingleLineTextField txtCategory;
        private BooleanField chkIsComposite;
        private BooleanField chkIsReusable;
    }
}
