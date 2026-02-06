using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Presentation.WinForms.Views.Domains
{
    partial class EntityEditViewEditView
    {
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        private void InitializeComponent()
        {
            lblTitle = new Label();
            txtName = new SingleLineTextField();
            cmbEntityState = new ComboboxField();
            tableLayoutPanel = new TableLayoutPanel();
            lblPreviewTitle = new Label();
            previewPanel = new Panel();
            tableLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblTitle.Location = new Point(10, 10);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(158, 21);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Edit View Properties";
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel.AutoSize = true;
            tableLayoutPanel.ColumnCount = 1;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.Controls.Add(txtName, 0, 0);
            tableLayoutPanel.Controls.Add(cmbEntityState, 0, 1);
            tableLayoutPanel.Location = new Point(10, 40);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 2;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.Size = new Size(380, 120);
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
            // cmbEntityState
            // 
            cmbEntityState.Dock = DockStyle.Top;
            cmbEntityState.Label = "Entity State:";
            cmbEntityState.Location = new Point(3, 59);
            cmbEntityState.Name = "cmbEntityState";
            cmbEntityState.Size = new Size(374, 50);
            cmbEntityState.TabIndex = 1;
            // 
            // lblPreviewTitle
            // 
            lblPreviewTitle.AutoSize = true;
            lblPreviewTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            lblPreviewTitle.Location = new Point(10, 170);
            lblPreviewTitle.Name = "lblPreviewTitle";
            lblPreviewTitle.Size = new Size(96, 19);
            lblPreviewTitle.TabIndex = 2;
            lblPreviewTitle.Text = "Field Preview";
            // 
            // previewPanel
            // 
            previewPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            previewPanel.AutoScroll = true;
            previewPanel.BackColor = SystemColors.ControlLightLight;
            previewPanel.BorderStyle = BorderStyle.FixedSingle;
            previewPanel.Location = new Point(10, 195);
            previewPanel.Name = "previewPanel";
            previewPanel.Padding = new Padding(5);
            previewPanel.Size = new Size(380, 295);
            previewPanel.TabIndex = 3;
            // 
            // EntityEditViewEditView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(previewPanel);
            Controls.Add(lblPreviewTitle);
            Controls.Add(tableLayoutPanel);
            Controls.Add(lblTitle);
            Name = "EntityEditViewEditView";
            Padding = new Padding(10);
            Size = new Size(400, 500);
            tableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private TableLayoutPanel tableLayoutPanel;
        private SingleLineTextField txtName;
        private ComboboxField cmbEntityState;
        private Label lblPreviewTitle;
        private Panel previewPanel;
    }
}
