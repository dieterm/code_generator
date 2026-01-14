namespace CodeGenerator.Presentation.WinForms.Views
{
    partial class TemplateTreeView
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
            this.components = new System.ComponentModel.Container();
            this.treeView = new Syncfusion.Windows.Forms.Tools.TreeViewAdv();
            this.lblTemplateFolder = new System.Windows.Forms.Label();
            this.txtTemplateFolder = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.treeView)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTemplateFolder
            // 
            this.lblTemplateFolder.AutoSize = true;
            this.lblTemplateFolder.Location = new System.Drawing.Point(3, 6);
            this.lblTemplateFolder.Name = "lblTemplateFolder";
            this.lblTemplateFolder.Size = new System.Drawing.Size(91, 15);
            this.lblTemplateFolder.TabIndex = 0;
            this.lblTemplateFolder.Text = "Template Folder:";
            // 
            // txtTemplateFolder
            // 
            this.txtTemplateFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTemplateFolder.Location = new System.Drawing.Point(100, 3);
            this.txtTemplateFolder.Name = "txtTemplateFolder";
            this.txtTemplateFolder.ReadOnly = true;
            this.txtTemplateFolder.Size = new System.Drawing.Size(197, 23);
            this.txtTemplateFolder.TabIndex = 1;
            // 
            // treeView
            // 
            this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView.Location = new System.Drawing.Point(3, 32);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(294, 315);
            this.treeView.TabIndex = 2;
            // 
            // TemplateTreeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeView);
            this.Controls.Add(this.txtTemplateFolder);
            this.Controls.Add(this.lblTemplateFolder);
            this.Name = "TemplateTreeView";
            this.Size = new System.Drawing.Size(300, 350);
            ((System.ComponentModel.ISupportInitialize)(this.treeView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Syncfusion.Windows.Forms.Tools.TreeViewAdv treeView;
        private System.Windows.Forms.Label lblTemplateFolder;
        private System.Windows.Forms.TextBox txtTemplateFolder;
    }
}
