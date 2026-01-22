namespace CodeGenerator.Presentation.WinForms.Views
{
    partial class DomainSchemaTreeView
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
            components = new System.ComponentModel.Container();
            Syncfusion.Windows.Forms.Tools.TreeNodeAdvStyleInfo treeNodeAdvStyleInfo1 = new Syncfusion.Windows.Forms.Tools.TreeNodeAdvStyleInfo();
            treeView = new Syncfusion.Windows.Forms.Tools.TreeViewAdv();
            imageList1 = new ImageList(components);
            ((System.ComponentModel.ISupportInitialize)treeView).BeginInit();
            SuspendLayout();
            // 
            // treeView
            // 
            treeView.AccelerateScrolling = Syncfusion.Windows.Forms.AccelerateScrollingBehavior.Immediate;
            treeNodeAdvStyleInfo1.CheckBoxTickThickness = 1;
            treeNodeAdvStyleInfo1.CheckColor = Color.FromArgb(109, 109, 109);
            treeNodeAdvStyleInfo1.EnsureDefaultOptionedChild = true;
            treeNodeAdvStyleInfo1.IntermediateCheckColor = Color.FromArgb(109, 109, 109);
            treeNodeAdvStyleInfo1.OptionButtonColor = Color.FromArgb(109, 109, 109);
            treeNodeAdvStyleInfo1.SelectedOptionButtonColor = Color.FromArgb(210, 210, 210);
            treeView.BaseStylePairs.AddRange(new Syncfusion.Windows.Forms.Tools.StyleNamePair[] { new Syncfusion.Windows.Forms.Tools.StyleNamePair("Standard", treeNodeAdvStyleInfo1) });
            treeView.Dock = DockStyle.Fill;
            // 
            // 
            // 
            treeView.HelpTextControl.BaseThemeName = null;
            treeView.HelpTextControl.Location = new Point(0, 0);
            treeView.HelpTextControl.Name = "";
            treeView.HelpTextControl.Size = new Size(392, 112);
            treeView.HelpTextControl.TabIndex = 0;
            treeView.HelpTextControl.Visible = true;
            treeView.InactiveSelectedNodeForeColor = SystemColors.ControlText;
            treeView.Location = new Point(0, 0);
            treeView.MetroColor = Color.FromArgb(22, 165, 220);
            treeView.Name = "treeView";
            treeView.SelectedNodeForeColor = SystemColors.HighlightText;
            treeView.Size = new Size(428, 411);
            treeView.TabIndex = 0;
            treeView.Text = "treeViewAdv1";
            treeView.ThemeStyle.TreeNodeAdvStyle.CheckBoxTickThickness = 0;
            treeView.ThemeStyle.TreeNodeAdvStyle.EnsureDefaultOptionedChild = true;
            // 
            // 
            // 
            treeView.ToolTipControl.BaseThemeName = null;
            treeView.ToolTipControl.Location = new Point(0, 0);
            treeView.ToolTipControl.Name = "";
            treeView.ToolTipControl.Size = new Size(392, 112);
            treeView.ToolTipControl.TabIndex = 0;
            treeView.ToolTipControl.Visible = true;
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth32Bit;
            imageList1.ImageSize = new Size(16, 16);
            imageList1.TransparentColor = Color.Transparent;
            // 
            // DomainSchemaTreeView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(treeView);
            Name = "DomainSchemaTreeView";
            Size = new Size(428, 411);
            ((System.ComponentModel.ISupportInitialize)treeView).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Syncfusion.Windows.Forms.Tools.TreeViewAdv treeView;
        private ImageList imageList1;
    }
}
