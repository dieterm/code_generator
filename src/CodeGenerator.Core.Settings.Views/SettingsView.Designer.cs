namespace CodeGenerator.Core.Settings.Views
{
    partial class SettingsView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsView));
            splitContainer1 = new SplitContainer();
            tvSettingSection = new TreeView();
            btnCancel = new Button();
            btnOk = new Button();
            pnlSearch = new Panel();
            textBoxExt1 = new Syncfusion.Windows.Forms.Tools.TextBoxExt();
            btnSearch = new Button();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.SuspendLayout();
            pnlSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)textBoxExt1).BeginInit();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(tvSettingSection);
            splitContainer1.Panel1.Controls.Add(pnlSearch);
            splitContainer1.Size = new Size(794, 410);
            splitContainer1.SplitterDistance = 264;
            splitContainer1.TabIndex = 0;
            // 
            // tvSettingSection
            // 
            tvSettingSection.Dock = DockStyle.Fill;
            tvSettingSection.Location = new Point(0, 29);
            tvSettingSection.Name = "tvSettingSection";
            tvSettingSection.Size = new Size(264, 381);
            tvSettingSection.TabIndex = 0;
            tvSettingSection.AfterSelect += tvSettingSection_AfterSelect;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(673, 416);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(121, 34);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnOk.DialogResult = DialogResult.OK;
            btnOk.Location = new Point(546, 416);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(121, 34);
            btnOk.TabIndex = 2;
            btnOk.Text = "Ok";
            btnOk.UseVisualStyleBackColor = true;
            // 
            // pnlSearch
            // 
            pnlSearch.Controls.Add(btnSearch);
            pnlSearch.Controls.Add(textBoxExt1);
            pnlSearch.Dock = DockStyle.Top;
            pnlSearch.Location = new Point(0, 0);
            pnlSearch.Name = "pnlSearch";
            pnlSearch.Size = new Size(264, 29);
            pnlSearch.TabIndex = 1;
            // 
            // textBoxExt1
            // 
            textBoxExt1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxExt1.BeforeTouchSize = new Size(226, 23);
            textBoxExt1.Location = new Point(3, 3);
            textBoxExt1.Name = "textBoxExt1";
            textBoxExt1.PlaceholderText = "Search setting section or item";
            textBoxExt1.Size = new Size(226, 23);
            textBoxExt1.TabIndex = 0;
            // 
            // btnSearch
            // 
            btnSearch.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSearch.Image = (Image)resources.GetObject("btnSearch.Image");
            btnSearch.Location = new Point(235, 1);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(26, 26);
            btnSearch.TabIndex = 1;
            btnSearch.UseVisualStyleBackColor = true;
            // 
            // SettingsView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(btnOk);
            Controls.Add(btnCancel);
            Controls.Add(splitContainer1);
            Name = "SettingsView";
            Size = new Size(794, 450);
            splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            pnlSearch.ResumeLayout(false);
            pnlSearch.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)textBoxExt1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private TreeView tvSettingSection;
        private Button btnCancel;
        private Button btnOk;
        private Panel pnlSearch;
        private Button btnSearch;
        private Syncfusion.Windows.Forms.Tools.TextBoxExt textBoxExt1;
    }
}
