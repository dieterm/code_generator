using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Controllers.Template;
using CodeGenerator.Application.ViewModels.Base;
using CodeGenerator.Application.ViewModels.Template;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Templates;
using Syncfusion.Windows.Forms.Tools;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views
{
    /// <summary>
    /// TreeView control for displaying and browsing templates
    /// </summary>
    public partial class TemplateTreeView : ArtifactTreeView
    {
        //private TemplateTreeViewModel? _viewModel;
        //private readonly Dictionary<string, TreeNodeAdv> _nodeMap = new();

        public TemplateTreeView()
            : base()
        {
            //InitializeComponent();
            //SetupTreeView();
        }

        /// <summary>
        /// The ViewModel
        /// </summary>
        //public TemplateTreeViewModel? ViewModel
        //{
        //    get => _viewModel;
        //    set => BindViewModel(value);
        //}

        //private void SetupTreeView()
        //{
        //    //TreeView.AfterSelect += TreeView_AfterSelect;
        //    TreeView.MouseClick += TreeView_MouseClick;
        //    TreeView.MouseDoubleClick += TreeView_MouseDoubleClick;
        //}

        //private void BindViewModel(TemplateTreeViewModel? viewModel)
        //{
        //    if (_viewModel != null)
        //    {
        //        _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        //        _viewModel.ContextMenuRequested -= ViewModel_ContextMenuRequested;
        //    }

        //    _viewModel = viewModel;

        //    if (_viewModel != null)
        //    {
        //        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        //        _viewModel.ContextMenuRequested += ViewModel_ContextMenuRequested;
        //        //txtTemplateFolder.Text = _viewModel.TemplateFolder;
        //        RefreshTree();
        //    }
        //}

        //private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == nameof(TemplateTreeViewModel.RootArtifact))
        //    {
        //        RefreshTree();
        //    }
        //    //else if (e.PropertyName == nameof(TemplateTreeViewModel.TemplateFolder))
        //    //{
        //    //    //txtTemplateFolder.Text = _viewModel?.TemplateFolder ?? string.Empty;
        //    //}
        //}

        //private void ViewModel_ContextMenuRequested(object? sender, ArtifactContextMenuEventArgs e)
        //{
        //    ShowContextMenu(e);
        //}

        //private void RefreshTree()
        //{
        //    TreeView.BeginUpdate();
        //    try
        //    {
        //        TreeView.Nodes.Clear();
        //        _nodeMap.Clear();

        //        if (_viewModel?.RootArtifact != null)
        //        {
        //            var rootNode = CreateNode(_viewModel.RootArtifact);
        //            TreeView.Nodes.Add(rootNode);
        //            rootNode.Expand();
        //        }
        //    }
        //    finally
        //    {
        //        TreeView.EndUpdate();
        //    }
        //}

        //private TreeNodeAdv CreateNode(IArtifact artifact)
        //{
        //    var node = new TreeNodeAdv(artifact.TreeNodeText)
        //    {
        //        Tag = artifact
        //    };

        //    // Set icon
        //    try
        //    {
        //        var icon = artifact.TreeNodeIcon?.GetIcon();
        //        if (icon != null)
        //        {
        //            node.LeftImageIndices = new int[] { GetOrAddImageIndex(artifact.TreeNodeIcon.IconKey, icon) };
        //        }
        //    }
        //    catch
        //    {
        //        // Ignore icon errors
        //    }

        //    _nodeMap[artifact.Id] = node;

        //    // Add children
        //    foreach (var child in artifact.Children)
        //    {
        //        var childNode = CreateNode(child);
        //        node.Nodes.Add(childNode);
        //    }

        //    return node;
        //}

        private int GetOrAddImageIndex(string key, System.Drawing.Image icon)
        {
            if (TreeView.LeftImageList == null)
            {
                TreeView.LeftImageList = new ImageList();
            }

            if (TreeView.LeftImageList.Images.ContainsKey(key))
            {
                return TreeView.LeftImageList.Images.IndexOfKey(key);
            }

            TreeView.LeftImageList.Images.Add(key, icon);
            return TreeView.LeftImageList.Images.Count - 1;
        }

        //private void TreeView_AfterSelect(object? sender, EventArgs e)
        //{
        //    if (TreeView.SelectedNode?.Tag is IArtifact artifact && _viewModel != null)
        //    {
        //        _viewModel.SelectedArtifact = artifact;
        //    }
        //}

        //private void TreeView_MouseClick(object? sender, MouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Right)
        //    {
        //        var node = TreeView.GetNodeAtPoint(e.Location);
        //        if (node?.Tag is IArtifact artifact && _viewModel != null)
        //        {
        //            TreeView.SelectedNode = node;
        //            _viewModel.RequestContextMenu(artifact, e.X, e.Y);
        //        }
        //    }
        //}

        //private async void TreeView_MouseDoubleClick(object? sender, MouseEventArgs e)
        //{
        //    var node = TreeView.GetNodeAtPoint(e.Location);
        //    if (node?.Tag is IArtifact artifact && _viewModel != null)
        //    {
        //        await _viewModel.HandleDoubleClickAsync(artifact);
        //    }
        //}

        //private void ShowContextMenu(ArtifactContextMenuEventArgs e)
        //{
        //    var contextMenu = new ContextMenuStrip();

        //    foreach (var command in e.Commands)
        //    {
        //        contextMenu.Items.Add(CreateMenuItem(command, e.Artifact));
        //    }

        //    if (contextMenu.Items.Count > 0)
        //    {
        //        contextMenu.Show(TreeView, new Point(e.X, e.Y));
        //    }
        //}

        //private ToolStripItem CreateMenuItem(ArtifactTreeNodeCommand command, IArtifact artifact)
        //{
        //    if (command.IsSeparator)
        //    {
        //        return new ToolStripSeparator();
        //    }

        //    var menuItem = new ToolStripMenuItem(command.Text);

        //    if (command.CanExecute != null)
        //    {
        //        menuItem.Enabled = command.CanExecute(artifact);
        //    }

        //    if (command.Execute != null)
        //    {
        //        menuItem.Click += async (s, args) => await command.Execute(artifact);
        //    }

        //    if (command.SubCommands != null && command.SubCommands.Any())
        //    {
        //        foreach (var subCommand in command.SubCommands)
        //        {
        //            menuItem.DropDownItems.Add(CreateMenuItem(subCommand, artifact));
        //        }
        //    }

        //    return menuItem;
        //}
    }
}
