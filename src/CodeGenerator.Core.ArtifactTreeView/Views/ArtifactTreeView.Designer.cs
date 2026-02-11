using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;

namespace CodeGenerator.Presentation.WinForms.Views
{
    partial class ArtifactTreeView : UserControl
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
            TreeView = new Syncfusion.Windows.Forms.Tools.TreeViewAdv();
            ContextMenu = new ContextMenuStrip(components);
            ((System.ComponentModel.ISupportInitialize)TreeView).BeginInit();
            SuspendLayout();
            // 
            // TreeView
            // 
            TreeView.Dock = DockStyle.Fill;
            TreeView.LabelEdit = true;
            TreeView.FullRowSelect = true;
            TreeView.HideSelection = false;
            TreeView.ShowLines = true;
            TreeView.ShowPlusMinus = true;
            TreeView.ShowRootLines = true;
            TreeView.Location = new Point(0, 0);
            TreeView.Name = "TreeView";
            TreeView.Size = new Size(300, 400);
            TreeView.TabIndex = 0;
            TreeView.Text = "treeView";
            // 
            // ContextMenu
            // 
            ContextMenu.Name = "ContextMenu";
            ContextMenu.Size = new Size(61, 4);
            // 
            // ArtifactTreeView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(TreeView);
            Name = "ArtifactTreeView";
            Size = new Size(300, 400);
            ((System.ComponentModel.ISupportInitialize)TreeView).EndInit();
            ResumeLayout(false);
        }

        #endregion

        protected Syncfusion.Windows.Forms.Tools.TreeViewAdv TreeView;
        protected ContextMenuStrip ContextMenu;
    }
}
