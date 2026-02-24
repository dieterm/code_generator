using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Presentation.WinForms.Views.Domains
{
    partial class EntityRelationEditView
    {
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        private void InitializeComponent()
        {
            lblTitle = new Label();
            txtName = new SingleLineTextField();
            cmbTargetEntity = new ComboboxField();
            cmbSourceCardinality = new ComboboxField();
            cmbTargetCardinality = new ComboboxField();
            txtSourcePropertyName = new SingleLineTextField();
            txtTargetPropertyName = new SingleLineTextField();
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
            lblTitle.Size = new Size(148, 21);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Relation Properties";
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel.AutoSize = true;
            tableLayoutPanel.ColumnCount = 1;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.Controls.Add(txtName, 0, 0);
            tableLayoutPanel.Controls.Add(cmbTargetEntity, 0, 1);
            tableLayoutPanel.Controls.Add(cmbSourceCardinality, 0, 2);
            tableLayoutPanel.Controls.Add(cmbTargetCardinality, 0, 3);
            tableLayoutPanel.Controls.Add(txtSourcePropertyName, 0, 4);
            tableLayoutPanel.Controls.Add(txtTargetPropertyName, 0, 5);
            tableLayoutPanel.Location = new Point(10, 40);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 6;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel.Size = new Size(380, 340);
            tableLayoutPanel.TabIndex = 1;
            // 
            // txtName
            // 
            txtName.Dock = DockStyle.Top;
            txtName.Label = "Relation Name:";
            txtName.Location = new Point(3, 3);
            txtName.Name = "txtName";
            txtName.Size = new Size(374, 50);
            txtName.TabIndex = 0;
            // 
            // cmbTargetEntity
            // 
            cmbTargetEntity.Dock = DockStyle.Top;
            cmbTargetEntity.Label = "Target Entity:";
            cmbTargetEntity.Location = new Point(3, 59);
            cmbTargetEntity.Name = "cmbTargetEntity";
            cmbTargetEntity.Size = new Size(374, 50);
            cmbTargetEntity.TabIndex = 1;
            // 
            // cmbSourceCardinality
            // 
            cmbSourceCardinality.Dock = DockStyle.Top;
            cmbSourceCardinality.Label = "Source Cardinality:";
            cmbSourceCardinality.Location = new Point(3, 115);
            cmbSourceCardinality.Name = "cmbSourceCardinality";
            cmbSourceCardinality.Size = new Size(374, 50);
            cmbSourceCardinality.TabIndex = 2;
            // 
            // cmbTargetCardinality
            // 
            cmbTargetCardinality.Dock = DockStyle.Top;
            cmbTargetCardinality.Label = "Target Cardinality:";
            cmbTargetCardinality.Location = new Point(3, 171);
            cmbTargetCardinality.Name = "cmbTargetCardinality";
            cmbTargetCardinality.Size = new Size(374, 50);
            cmbTargetCardinality.TabIndex = 3;
            // 
            // txtSourcePropertyName
            // 
            txtSourcePropertyName.Dock = DockStyle.Top;
            txtSourcePropertyName.Label = "Source Property Name:";
            txtSourcePropertyName.Location = new Point(3, 227);
            txtSourcePropertyName.Name = "txtSourcePropertyName";
            txtSourcePropertyName.Size = new Size(374, 50);
            txtSourcePropertyName.TabIndex = 4;
            // 
            // txtTargetPropertyName
            // 
            txtTargetPropertyName.Dock = DockStyle.Top;
            txtTargetPropertyName.Label = "Target Property Name:";
            txtTargetPropertyName.Location = new Point(3, 283);
            txtTargetPropertyName.Name = "txtTargetPropertyName";
            txtTargetPropertyName.Size = new Size(374, 50);
            txtTargetPropertyName.TabIndex = 5;
            // 
            // EntityRelationEditView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel);
            Controls.Add(lblTitle);
            Name = "EntityRelationEditView";
            Padding = new Padding(10);
            Size = new Size(400, 400);
            tableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private TableLayoutPanel tableLayoutPanel;
        private SingleLineTextField txtName;
        private ComboboxField cmbTargetEntity;
        private ComboboxField cmbSourceCardinality;
        private ComboboxField cmbTargetCardinality;
        private SingleLineTextField txtSourcePropertyName;
        private SingleLineTextField txtTargetPropertyName;
    }
}
