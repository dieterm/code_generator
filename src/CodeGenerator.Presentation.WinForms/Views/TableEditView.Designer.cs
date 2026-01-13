using CodeGenerator.UserControls.Views;

namespace CodeGenerator.Presentation.WinForms.Views
{
    partial class TableEditView
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
            grpGeneral = new GroupBox();
            tableLayoutGeneral = new TableLayoutPanel();
            txtName = new SingleLineTextField();
            txtSchema = new SingleLineTextField();
            grpGeneral.SuspendLayout();
            tableLayoutGeneral.SuspendLayout();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblTitle.Location = new Point(10, 10);
            lblTitle.Name = "lblTitle";
            lblTitle.Padding = new Padding(0, 0, 0, 10);
            lblTitle.Size = new Size(136, 31);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Table Details";
            // 
            // grpGeneral
            // 
            grpGeneral.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpGeneral.Controls.Add(tableLayoutGeneral);
            grpGeneral.Location = new Point(10, 44);
            grpGeneral.Name = "grpGeneral";
            grpGeneral.Size = new Size(380, 120);
            grpGeneral.TabIndex = 1;
            grpGeneral.TabStop = false;
            grpGeneral.Text = "General";
            // 
            // tableLayoutGeneral
            // 
            tableLayoutGeneral.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutGeneral.AutoSize = true;
            tableLayoutGeneral.ColumnCount = 1;
            tableLayoutGeneral.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutGeneral.Controls.Add(txtName, 0, 0);
            tableLayoutGeneral.Controls.Add(txtSchema, 0, 1);
            tableLayoutGeneral.Location = new Point(6, 22);
            tableLayoutGeneral.Name = "tableLayoutGeneral";
            tableLayoutGeneral.RowCount = 2;
            tableLayoutGeneral.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutGeneral.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutGeneral.Size = new Size(368, 86);
            tableLayoutGeneral.TabIndex = 0;
            // 
            // txtName
            // 
            txtName.Dock = DockStyle.Top;
            txtName.Label = "Table Name:";
            txtName.Location = new Point(3, 3);
            txtName.Name = "txtName";
            txtName.Size = new Size(362, 36);
            txtName.TabIndex = 0;
            // 
            // txtSchema
            // 
            txtSchema.Dock = DockStyle.Top;
            txtSchema.Label = "Schema:";
            txtSchema.Location = new Point(3, 45);
            txtSchema.Name = "txtSchema";
            txtSchema.Size = new Size(362, 36);
            txtSchema.TabIndex = 1;
            // 
            // TableEditView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            Controls.Add(grpGeneral);
            Controls.Add(lblTitle);
            Name = "TableEditView";
            Padding = new Padding(10);
            Size = new Size(400, 300);
            grpGeneral.ResumeLayout(false);
            grpGeneral.PerformLayout();
            tableLayoutGeneral.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private GroupBox grpGeneral;
        private TableLayoutPanel tableLayoutGeneral;
        private SingleLineTextField txtName;
        private SingleLineTextField txtSchema;
    }
}
