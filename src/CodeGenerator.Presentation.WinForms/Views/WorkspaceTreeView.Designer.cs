namespace CodeGenerator.Presentation.WinForms.Views
{
    partial class WorkspaceTreeView
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
            components = new System.ComponentModel.Container();
            Syncfusion.Windows.Forms.Tools.TreeNodeAdvStyleInfo treeNodeAdvStyleInfo1 = new Syncfusion.Windows.Forms.Tools.TreeNodeAdvStyleInfo();
            treeView = new Syncfusion.Windows.Forms.Tools.TreeViewAdv();
            ctxArtifactMenu = new ContextMenuStrip(components);
            ((System.ComponentModel.ISupportInitialize)treeView).BeginInit();
            SuspendLayout();
            // 
            // treeView
            // 
            treeNodeAdvStyleInfo1.CheckBoxTickThickness = 1;
            treeNodeAdvStyleInfo1.CheckColor = Color.FromArgb(109, 109, 109);
            treeNodeAdvStyleInfo1.EnsureDefaultOptionedChild = true;
            treeNodeAdvStyleInfo1.IntermediateCheckColor = Color.FromArgb(109, 109, 109);
            treeNodeAdvStyleInfo1.OptionButtonColor = Color.FromArgb(109, 109, 109);
            treeNodeAdvStyleInfo1.SelectedOptionButtonColor = Color.FromArgb(210, 210, 210);
            treeNodeAdvStyleInfo1.ShowPlusMinus = true;
            treeView.BaseStylePairs.AddRange(new Syncfusion.Windows.Forms.Tools.StyleNamePair[] { new Syncfusion.Windows.Forms.Tools.StyleNamePair("Standard", treeNodeAdvStyleInfo1) });
            treeView.ContextMenuStrip = ctxArtifactMenu;
            treeView.Dock = DockStyle.Fill;
            // 
            // 
            // 
            treeView.HelpTextControl.BaseThemeName = null;
            treeView.HelpTextControl.Location = new Point(0, 0);
            treeView.HelpTextControl.Name = "";
            treeView.HelpTextControl.TabIndex = 0;
            treeView.HotTracking = true;
            treeView.InactiveSelectedNodeForeColor = SystemColors.ControlText;
            treeView.LabelEdit = true;
            treeView.Location = new Point(0, 0);
            treeView.MetroColor = Color.FromArgb(22, 165, 220);
            treeView.Name = "treeView";
            treeView.SelectedNodeForeColor = SystemColors.HighlightText;
            treeView.Size = new Size(300, 400);
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
            treeView.ToolTipControl.TabIndex = 0;
            // 
            // ctxArtifactMenu
            // 
            ctxArtifactMenu.Name = "ctxArtifactMenu";
            ctxArtifactMenu.Size = new Size(181, 26);
            ctxArtifactMenu.Opening += ctxArtifactMenu_Opening;
            // 
            // WorkspaceTreeView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(treeView);
            Name = "WorkspaceTreeView";
            Size = new Size(300, 400);
            ((System.ComponentModel.ISupportInitialize)treeView).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Syncfusion.Windows.Forms.Tools.TreeViewAdv treeView;
        private ContextMenuStrip ctxArtifactMenu;
    }
}
