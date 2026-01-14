using CodeGenerator.Application.ViewModels;
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
    public partial class TemplateTreeView : UserControl
    {
        private TemplateTreeViewModel? _viewModel;
        private readonly Dictionary<string, TreeNodeAdv> _nodeMap = new();

        public TemplateTreeView()
        {
            InitializeComponent();
            SetupTreeView();
        }

        /// <summary>
        /// The ViewModel
        /// </summary>
        public TemplateTreeViewModel? ViewModel
        {
            get => _viewModel;
            set => BindViewModel(value);
        }

        private void SetupTreeView()
        {
            treeView.AfterSelect += TreeView_AfterSelect;
            treeView.MouseClick += TreeView_MouseClick;
            treeView.MouseDoubleClick += TreeView_MouseDoubleClick;
        }

        private void BindViewModel(TemplateTreeViewModel? viewModel)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }

            _viewModel = viewModel;

            if (_viewModel != null)
            {
                _viewModel.PropertyChanged += ViewModel_PropertyChanged;
                txtTemplateFolder.Text = _viewModel.TemplateFolder;
                RefreshTree();
            }
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TemplateTreeViewModel.RootArtifact))
            {
                RefreshTree();
            }
            else if (e.PropertyName == nameof(TemplateTreeViewModel.TemplateFolder))
            {
                txtTemplateFolder.Text = _viewModel?.TemplateFolder ?? string.Empty;
            }
        }

        private void RefreshTree()
        {
            treeView.BeginUpdate();
            try
            {
                treeView.Nodes.Clear();
                _nodeMap.Clear();

                if (_viewModel?.RootArtifact != null)
                {
                    var rootNode = CreateNode(_viewModel.RootArtifact);
                    treeView.Nodes.Add(rootNode);
                    rootNode.Expand();
                }
            }
            finally
            {
                treeView.EndUpdate();
            }
        }

        private TreeNodeAdv CreateNode(IArtifact artifact)
        {
            var node = new TreeNodeAdv(artifact.TreeNodeText)
            {
                Tag = artifact
            };

            // Set icon
            try
            {
                var icon = artifact.TreeNodeIcon?.GetIcon();
                if (icon != null)
                {
                    node.LeftImageIndices = new int[] { GetOrAddImageIndex(artifact.TreeNodeIcon.IconKey, icon) };
                }
            }
            catch
            {
                // Ignore icon errors
            }

            _nodeMap[artifact.Id] = node;

            // Add children
            foreach (var child in artifact.Children)
            {
                var childNode = CreateNode(child);
                node.Nodes.Add(childNode);
            }

            return node;
        }

        private int GetOrAddImageIndex(string key, System.Drawing.Image icon)
        {
            if (treeView.LeftImageList == null)
            {
                treeView.LeftImageList = new ImageList();
            }

            if (treeView.LeftImageList.Images.ContainsKey(key))
            {
                return treeView.LeftImageList.Images.IndexOfKey(key);
            }

            treeView.LeftImageList.Images.Add(key, icon);
            return treeView.LeftImageList.Images.Count - 1;
        }

        private void TreeView_AfterSelect(object? sender, EventArgs e)
        {
            if (treeView.SelectedNode?.Tag is IArtifact artifact && _viewModel != null)
            {
                _viewModel.SelectedArtifact = artifact;
            }
        }

        private void TreeView_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var node = treeView.GetNodeAtPoint(e.Location);
                if (node?.Tag is IArtifact artifact && _viewModel != null)
                {
                    treeView.SelectedNode = node;
                    _viewModel.RequestContextMenu(artifact, e.X, e.Y);
                }
            }
        }

        private void TreeView_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            var node = treeView.GetNodeAtPoint(e.Location);
            if (node?.Tag is TemplateArtifact templateArtifact && _viewModel != null)
            {
                _viewModel.OnTemplateSelected(templateArtifact);
            }
        }
    }
}
