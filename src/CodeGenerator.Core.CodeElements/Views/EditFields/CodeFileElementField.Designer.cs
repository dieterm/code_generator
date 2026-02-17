namespace CodeGenerator.Core.CodeElements.Views.EditFields
{
    partial class CodeFileElementField
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblErrorMessage = new Label();
            lblLabel = new Label();
            tableLayoutPanel1 = new TableLayoutPanel();
            btnLoadCodeFileElement = new Button();
            btnSaveCodeFileElement = new Button();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // lblErrorMessage
            // 
            lblErrorMessage.AutoSize = true;
            lblErrorMessage.ForeColor = Color.Red;
            lblErrorMessage.Location = new Point(115, 28);
            lblErrorMessage.Name = "lblErrorMessage";
            lblErrorMessage.Size = new Size(81, 15);
            lblErrorMessage.TabIndex = 4;
            lblErrorMessage.Text = "Error message";
            // 
            // lblLabel
            // 
            lblLabel.AutoSize = true;
            lblLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblLabel.Location = new Point(3, 0);
            lblLabel.Name = "lblLabel";
            lblLabel.Size = new Size(68, 15);
            lblLabel.TabIndex = 3;
            lblLabel.Text = "Field Label:";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(btnLoadCodeFileElement, 0, 0);
            tableLayoutPanel1.Controls.Add(btnSaveCodeFileElement, 1, 0);
            tableLayoutPanel1.Location = new Point(115, 1);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(210, 30);
            tableLayoutPanel1.TabIndex = 5;
            // 
            // btnLoadCodeFileElement
            // 
            btnLoadCodeFileElement.Dock = DockStyle.Fill;
            btnLoadCodeFileElement.Location = new Point(3, 3);
            btnLoadCodeFileElement.Name = "btnLoadCodeFileElement";
            btnLoadCodeFileElement.Size = new Size(99, 24);
            btnLoadCodeFileElement.TabIndex = 0;
            btnLoadCodeFileElement.Text = "Load";
            btnLoadCodeFileElement.UseVisualStyleBackColor = true;
            // 
            // btnSaveCodeFileElement
            // 
            btnSaveCodeFileElement.Dock = DockStyle.Fill;
            btnSaveCodeFileElement.Location = new Point(108, 3);
            btnSaveCodeFileElement.Name = "btnSaveCodeFileElement";
            btnSaveCodeFileElement.Size = new Size(99, 24);
            btnSaveCodeFileElement.TabIndex = 1;
            btnSaveCodeFileElement.Text = "Save";
            btnSaveCodeFileElement.UseVisualStyleBackColor = true;
            // 
            // CodeFileElementField
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel1);
            Controls.Add(lblErrorMessage);
            Controls.Add(lblLabel);
            Name = "CodeFileElementField";
            Size = new Size(328, 48);
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblErrorMessage;
        private Label lblLabel;
        private TableLayoutPanel tableLayoutPanel1;
        private Button btnLoadCodeFileElement;
        private Button btnSaveCodeFileElement;
    }
}
