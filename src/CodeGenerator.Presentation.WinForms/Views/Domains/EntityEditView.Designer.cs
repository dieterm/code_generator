using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Presentation.WinForms.Views.Domains
{
    partial class EntityEditView
    {
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        private void InitializeComponent()
        {
            lblTitle = new Label();
            txtName = new SingleLineTextField();
            txtDescription = new SingleLineTextField();
            chkIsAggregateRoot = new BooleanField();
            cbxDefaultState = new ComboboxField();
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
            lblTitle.Size = new Size(138, 21);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Entity Properties";
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel.AutoSize = true;
            tableLayoutPanel.ColumnCount = 1;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.Controls.Add(txtName, 0, 0);
            tableLayoutPanel.Controls.Add(txtDescription, 0, 1);
            tableLayoutPanel.Controls.Add(chkIsAggregateRoot, 0, 2);
            tableLayoutPanel.Controls.Add(cbxDefaultState, 0, 3);
            tableLayoutPanel.Location = new Point(10, 40);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 4;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.Size = new Size(380, 220);
            tableLayoutPanel.TabIndex = 1;
            // 
            // txtName
            // 
            txtName.Dock = DockStyle.Top;
            txtName.Label = "Entity Name:";
            txtName.Location = new Point(3, 3);
            txtName.Name = "txtName";
            txtName.Size = new Size(374, 50);
            txtName.TabIndex = 0;
            // 
            // txtDescription
            // 
            txtDescription.Dock = DockStyle.Top;
            txtDescription.Label = "Description:";
            txtDescription.Location = new Point(3, 59);
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(374, 50);
            txtDescription.TabIndex = 1;
            // 
            // chkIsAggregateRoot
            // 
            chkIsAggregateRoot.Dock = DockStyle.Top;
            chkIsAggregateRoot.Label = "Is Aggregate Root:";
            chkIsAggregateRoot.Location = new Point(3, 115);
            chkIsAggregateRoot.Name = "chkIsAggregateRoot";
            chkIsAggregateRoot.Size = new Size(374, 30);
            chkIsAggregateRoot.TabIndex = 2;
            // 
            // cbxDefaultState
            // 
            cbxDefaultState.Dock = DockStyle.Top;
            cbxDefaultState.Label = "Default State:";
            cbxDefaultState.Location = new Point(3, 151);
            cbxDefaultState.Name = "cbxDefaultState";
            cbxDefaultState.Size = new Size(374, 50);
            cbxDefaultState.TabIndex = 3;
            // 
            // EntityEditView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel);
            Controls.Add(lblTitle);
            Name = "EntityEditView";
            Padding = new Padding(10);
            Size = new Size(400, 280);
            tableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private TableLayoutPanel tableLayoutPanel;
        private SingleLineTextField txtName;
        private SingleLineTextField txtDescription;
        private BooleanField chkIsAggregateRoot;
        private ComboboxField cbxDefaultState;
    }
}
