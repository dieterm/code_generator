namespace CodeGenerator.Presentation.WinForms.Views
{
    partial class GenerationResultTreeView
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
            Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.TreeNodeAdvStyleInfo treeNodeAdvStyleInfo2 = new Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.TreeNodeAdvStyleInfo();
            Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.TreeNodeAdvSubItemStyleInfo treeNodeAdvSubItemStyleInfo1 = new Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.TreeNodeAdvSubItemStyleInfo();
            Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.TreeColumnAdvStyleInfo treeColumnAdvStyleInfo1 = new Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.TreeColumnAdvStyleInfo();
            artifactTreeView = new Syncfusion.Windows.Forms.Tools.TreeViewAdv();
            imageList = new ImageList(components);
            splitContainer1 = new SplitContainer();
            artifactDetailsTreeView = new Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.MultiColumnTreeView();
            ((System.ComponentModel.ISupportInitialize)artifactTreeView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)artifactDetailsTreeView).BeginInit();
            SuspendLayout();
            // 
            // artifactTreeView
            // 
            artifactTreeView.AccelerateScrolling = Syncfusion.Windows.Forms.AccelerateScrollingBehavior.Immediate;
            treeNodeAdvStyleInfo1.CheckBoxTickThickness = 1;
            treeNodeAdvStyleInfo1.CheckColor = Color.FromArgb(109, 109, 109);
            treeNodeAdvStyleInfo1.EnsureDefaultOptionedChild = true;
            treeNodeAdvStyleInfo1.IntermediateCheckColor = Color.FromArgb(109, 109, 109);
            treeNodeAdvStyleInfo1.OptionButtonColor = Color.FromArgb(109, 109, 109);
            treeNodeAdvStyleInfo1.SelectedOptionButtonColor = Color.FromArgb(210, 210, 210);
            artifactTreeView.BaseStylePairs.AddRange(new Syncfusion.Windows.Forms.Tools.StyleNamePair[] { new Syncfusion.Windows.Forms.Tools.StyleNamePair("Standard", treeNodeAdvStyleInfo1) });
            artifactTreeView.Dock = DockStyle.Fill;
            // 
            // 
            // 
            artifactTreeView.HelpTextControl.BaseThemeName = null;
            artifactTreeView.HelpTextControl.Location = new Point(0, 0);
            artifactTreeView.HelpTextControl.Name = "";
            artifactTreeView.HelpTextControl.Size = new Size(392, 112);
            artifactTreeView.HelpTextControl.TabIndex = 0;
            artifactTreeView.HelpTextControl.Visible = true;
            artifactTreeView.InactiveSelectedNodeForeColor = SystemColors.ControlText;
            artifactTreeView.LeftImageList = imageList;
            artifactTreeView.Location = new Point(0, 0);
            artifactTreeView.MetroColor = Color.FromArgb(22, 165, 220);
            artifactTreeView.Name = "artifactTreeView";
            artifactTreeView.SelectedNodeForeColor = SystemColors.HighlightText;
            artifactTreeView.Size = new Size(360, 151);
            artifactTreeView.TabIndex = 0;
            artifactTreeView.Text = "treeViewAdv1";
            artifactTreeView.ThemeStyle.TreeNodeAdvStyle.CheckBoxTickThickness = 0;
            artifactTreeView.ThemeStyle.TreeNodeAdvStyle.EnsureDefaultOptionedChild = true;
            // 
            // 
            // 
            artifactTreeView.ToolTipControl.BaseThemeName = null;
            artifactTreeView.ToolTipControl.Location = new Point(0, 0);
            artifactTreeView.ToolTipControl.Name = "";
            artifactTreeView.ToolTipControl.Size = new Size(392, 112);
            artifactTreeView.ToolTipControl.TabIndex = 0;
            artifactTreeView.ToolTipControl.Visible = true;
            artifactTreeView.NodeMouseClick += artifactTreeView_NodeMouseClick;
            // 
            // imageList
            // 
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.ImageSize = new Size(16, 16);
            imageList.TransparentColor = Color.Transparent;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(artifactTreeView);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(artifactDetailsTreeView);
            splitContainer1.Size = new Size(360, 302);
            splitContainer1.SplitterDistance = 151;
            splitContainer1.TabIndex = 1;
            // 
            // artifactDetailsTreeView
            // 
            artifactDetailsTreeView.AutoAdjustMultiLineHeight = true;
            artifactDetailsTreeView.AutoGenerateColumns = true;
            treeNodeAdvStyleInfo2.CheckBoxTickThickness = 1;
            treeNodeAdvStyleInfo2.CheckColor = Color.FromArgb(109, 109, 109);
            treeNodeAdvStyleInfo2.DisabledCheckColor = Color.FromArgb(210, 210, 210);
            treeNodeAdvStyleInfo2.DisabledOptionButtonColor = Color.FromArgb(243, 243, 243);
            treeNodeAdvStyleInfo2.HoverCheckColor = Color.FromArgb(68, 68, 68);
            treeNodeAdvStyleInfo2.HoverOptionButtonColor = Color.FromArgb(222, 236, 249);
            treeNodeAdvStyleInfo2.IntermediateCheckColor = Color.FromArgb(109, 109, 109);
            treeNodeAdvStyleInfo2.IntermediateDisabledCheckColor = Color.FromArgb(210, 210, 210);
            treeNodeAdvStyleInfo2.IntermediateHoverCheckColor = Color.FromArgb(68, 68, 68);
            treeNodeAdvStyleInfo2.OptionButtonColor = Color.FromArgb(255, 255, 255);
            treeNodeAdvStyleInfo2.SelectedOptionButtonColor = Color.FromArgb(109, 109, 109);
            artifactDetailsTreeView.BaseStylePairs.AddRange(new Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.StyleNamePair[] { new Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.StyleNamePair("Standard", treeNodeAdvStyleInfo2), new Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.StyleNamePair("Standard - SubItem", treeNodeAdvSubItemStyleInfo1), new Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.StyleNamePair("Standard - Column", treeColumnAdvStyleInfo1) });
            artifactDetailsTreeView.ColumnsHeaderBackground = new Syncfusion.Drawing.BrushInfo(SystemColors.Control);
            artifactDetailsTreeView.Dock = DockStyle.Fill;
            artifactDetailsTreeView.Filter = null;
            artifactDetailsTreeView.FilterLevel = Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.FilterLevel.All;
            // 
            // 
            // 
            artifactDetailsTreeView.HelpTextControl.BaseThemeName = null;
            artifactDetailsTreeView.HelpTextControl.BorderStyle = BorderStyle.FixedSingle;
            artifactDetailsTreeView.HelpTextControl.Location = new Point(0, 0);
            artifactDetailsTreeView.HelpTextControl.Name = "m_helpText";
            artifactDetailsTreeView.HelpTextControl.Size = new Size(54, 17);
            artifactDetailsTreeView.HelpTextControl.TabIndex = 0;
            artifactDetailsTreeView.HelpTextControl.Text = "help text";
            artifactDetailsTreeView.Location = new Point(0, 0);
            artifactDetailsTreeView.Name = "artifactDetailsTreeView";
            artifactDetailsTreeView.NodeHoverColor = Color.FromArgb(51, 153, 255);
            artifactDetailsTreeView.Size = new Size(360, 147);
            artifactDetailsTreeView.TabIndex = 0;
            artifactDetailsTreeView.Text = "multiColumnTreeView1";
            artifactDetailsTreeView.ThemeStyle.TreeNodeAdvStyle.CheckBoxTickThickness = 0;
            // 
            // 
            // 
            artifactDetailsTreeView.ToolTipControl.BackColor = SystemColors.Info;
            artifactDetailsTreeView.ToolTipControl.BaseThemeName = null;
            artifactDetailsTreeView.ToolTipControl.BorderStyle = BorderStyle.FixedSingle;
            artifactDetailsTreeView.ToolTipControl.Location = new Point(0, 0);
            artifactDetailsTreeView.ToolTipControl.Name = "m_toolTip";
            artifactDetailsTreeView.ToolTipControl.Size = new Size(47, 17);
            artifactDetailsTreeView.ToolTipControl.TabIndex = 1;
            artifactDetailsTreeView.ToolTipControl.Text = "toolTip";
            // 
            // GenerationResultTreeView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(splitContainer1);
            Name = "GenerationResultTreeView";
            Size = new Size(360, 302);
            ((System.ComponentModel.ISupportInitialize)artifactTreeView).EndInit();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)artifactDetailsTreeView).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Syncfusion.Windows.Forms.Tools.TreeViewAdv artifactTreeView;
        private ImageList imageList;
        private SplitContainer splitContainer1;
        private Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.MultiColumnTreeView artifactDetailsTreeView;
    }
}
