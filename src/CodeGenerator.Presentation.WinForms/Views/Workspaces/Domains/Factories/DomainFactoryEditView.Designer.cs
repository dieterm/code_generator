using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Presentation.WinForms.Views.Workspace.Domains.Factories
{
    partial class DomainFactoryEditView
    {
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        private void InitializeComponent()
        {
            lblTitle = new Label();
            txtName = new SingleLineTextField();
            txtDescription = new SingleLineTextField();
            txtCategory = new SingleLineTextField();
            chkCreatesAggregates = new BooleanField();
            chkIsStateless = new BooleanField();
            chkHasDependencies = new BooleanField();
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
            lblTitle.Size = new Size(150, 21);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Factory Properties";
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel.AutoScroll = true;
            tableLayoutPanel.ColumnCount = 1;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.Controls.Add(txtName, 0, 0);
            tableLayoutPanel.Controls.Add(txtDescription, 0, 1);
            tableLayoutPanel.Controls.Add(txtCategory, 0, 2);
            tableLayoutPanel.Controls.Add(chkCreatesAggregates, 0, 3);
            tableLayoutPanel.Controls.Add(chkIsStateless, 0, 4);
            tableLayoutPanel.Controls.Add(chkHasDependencies, 0, 5);
            tableLayoutPanel.Location = new Point(10, 40);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 6;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.Size = new Size(480, 350);
            tableLayoutPanel.TabIndex = 1;
            // 
            // txtName
            // 
            txtName.Dock = DockStyle.Top;
            txtName.Label = "Factory Name:";
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
            // txtCategory
            // 
            txtCategory.Dock = DockStyle.Top;
            txtCategory.Label = "Category:";
            txtCategory.Location = new Point(3, 115);
            txtCategory.Name = "txtCategory";
            txtCategory.Size = new Size(474, 50);
            txtCategory.TabIndex = 2;
            // 
            // chkCreatesAggregates
            // 
            chkCreatesAggregates.Dock = DockStyle.Top;
            chkCreatesAggregates.Label = "Creates Aggregates:";
            chkCreatesAggregates.Location = new Point(3, 171);
            chkCreatesAggregates.Name = "chkCreatesAggregates";
            chkCreatesAggregates.Size = new Size(474, 30);
            chkCreatesAggregates.TabIndex = 3;
            // 
            // chkIsStateless
            // 
            chkIsStateless.Dock = DockStyle.Top;
            chkIsStateless.Label = "Is Stateless:";
            chkIsStateless.Location = new Point(3, 207);
            chkIsStateless.Name = "chkIsStateless";
            chkIsStateless.Size = new Size(474, 30);
            chkIsStateless.TabIndex = 4;
            // 
            // chkHasDependencies
            // 
            chkHasDependencies.Dock = DockStyle.Top;
            chkHasDependencies.Label = "Has Dependencies:";
            chkHasDependencies.Location = new Point(3, 243);
            chkHasDependencies.Name = "chkHasDependencies";
            chkHasDependencies.Size = new Size(474, 30);
            chkHasDependencies.TabIndex = 5;
            // 
            // DomainFactoryEditView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel);
            Controls.Add(lblTitle);
            Name = "DomainFactoryEditView";
            Padding = new Padding(10);
            Size = new Size(500, 400);
            tableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private TableLayoutPanel tableLayoutPanel;
        private SingleLineTextField txtName;
        private SingleLineTextField txtDescription;
        private SingleLineTextField txtCategory;
        private BooleanField chkCreatesAggregates;
        private BooleanField chkIsStateless;
        private BooleanField chkHasDependencies;
    }
}
