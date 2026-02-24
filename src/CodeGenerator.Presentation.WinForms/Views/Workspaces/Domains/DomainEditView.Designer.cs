using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Presentation.WinForms.Views
{
    partial class DomainEditView
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
            txtName = new SingleLineTextField();
            txtDescription = new SingleLineTextField();
            txtNamespace = new ParameterizedStringField();
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
            lblTitle.Size = new Size(147, 21);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Domain Properties";
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel.AutoSize = true;
            tableLayoutPanel.ColumnCount = 1;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.Controls.Add(txtName, 0, 0);
            tableLayoutPanel.Controls.Add(txtDescription, 0, 1);
            tableLayoutPanel.Controls.Add(txtNamespace, 0, 2);
            tableLayoutPanel.Location = new Point(10, 40);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 3;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.Size = new Size(380, 200);
            tableLayoutPanel.TabIndex = 1;
            // 
            // txtName
            // 
            txtName.Dock = DockStyle.Top;
            txtName.Label = "Domain Name:";
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
            // txtDefaultNamespace
            // 
            txtNamespace.Dock = DockStyle.Top;
            txtNamespace.Label = "Default Namespace:";
            txtNamespace.Location = new Point(3, 115);
            txtNamespace.Name = "txtDefaultNamespace";
            txtNamespace.Size = new Size(374, 80);
            txtNamespace.TabIndex = 2;
            // 
            // DomainEditView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel);
            Controls.Add(lblTitle);
            Name = "DomainEditView";
            Padding = new Padding(10);
            Size = new Size(400, 260);
            tableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private TableLayoutPanel tableLayoutPanel;
        private SingleLineTextField txtName;
        private SingleLineTextField txtDescription;
        private ParameterizedStringField txtNamespace;
    }
}
