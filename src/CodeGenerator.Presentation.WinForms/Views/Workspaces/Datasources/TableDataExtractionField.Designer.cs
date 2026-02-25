namespace CodeGenerator.Presentation.WinForms.Views
{
    partial class TableDataExtractionField
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
            btnLoadData = new Button();
            pnlProperties = new Panel();
            SuspendLayout();
            // 
            // btnLoadData
            // 
            btnLoadData.Dock = DockStyle.Top;
            btnLoadData.Location = new Point(0, 0);
            btnLoadData.Name = "btnLoadData";
            btnLoadData.Size = new Size(380, 23);
            btnLoadData.TabIndex = 0;
            btnLoadData.Text = "Load Data";
            btnLoadData.UseVisualStyleBackColor = true;
            // 
            // pnlProperties
            // 
            pnlProperties.AutoScroll = true;
            pnlProperties.Dock = DockStyle.Fill;
            pnlProperties.Location = new Point(0, 23);
            pnlProperties.Name = "pnlProperties";
            pnlProperties.Size = new Size(380, 177);
            pnlProperties.TabIndex = 1;
            // 
            // TableDataExtractionField
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pnlProperties);
            Controls.Add(btnLoadData);
            Name = "TableDataExtractionField";
            Size = new Size(380, 200);
            ResumeLayout(false);
        }

        #endregion

        private Button btnLoadData;
        private Panel pnlProperties;
    }
}
