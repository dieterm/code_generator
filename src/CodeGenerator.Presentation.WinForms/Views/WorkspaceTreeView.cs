using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.ViewModels.Base;
using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.Events;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Shared.Views.TreeNode;
using Syncfusion.Windows.Forms.Tools;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views
{
    /// <summary>
    /// TreeView control for displaying and managing workspaces
    /// </summary>
    public partial class WorkspaceTreeView : UserControl
    {
        private WorkspaceTreeViewModel? _viewModel;
        private readonly Dictionary<string, TreeNodeAdv> _nodeMap = new();

        public WorkspaceTreeView()
        {
            InitializeComponent();
            SetupTreeView();
        }

        /// <summary>
        /// The ViewModel
        /// </summary>
        public WorkspaceTreeViewModel? ViewModel
        {
            get => _viewModel;
            set => BindViewModel(value);
        }

        private void SetupTreeView()
        {
            treeView.LabelEdit = true;
            treeView.AfterSelect += TreeView_AfterSelect;
            treeView.MouseClick += TreeView_MouseClick;
            treeView.MouseDoubleClick += TreeView_MouseDoubleClick;
            treeView.BeforeEdit += TreeView_BeforeEdit;
            treeView.NodeEditorValidating += TreeView_NodeEditorValidating;
            treeView.NodeEditorValidated += TreeView_NodeEditorValidated;
        }

        private void BindViewModel(WorkspaceTreeViewModel? viewModel)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
                _viewModel.ContextMenuRequested -= ViewModel_ContextMenuRequested;
                _viewModel.BeginRenameRequested -= ViewModel_BeginRenameRequested;
                _viewModel.ArtifactRefreshRequested -= ViewModel_ArtifactRefreshRequested;
            }

            _viewModel = viewModel;

            if (_viewModel != null)
            {
                _viewModel.PropertyChanged += ViewModel_PropertyChanged;
                _viewModel.ContextMenuRequested += ViewModel_ContextMenuRequested;
                _viewModel.BeginRenameRequested += ViewModel_BeginRenameRequested;
                _viewModel.ArtifactRefreshRequested += ViewModel_ArtifactRefreshRequested;
                RefreshTree();
            }
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(WorkspaceTreeViewModel.Workspace) ||
                e.PropertyName == nameof(WorkspaceTreeViewModel.RootArtifact))
            {
                RefreshTree();
            }
        }

        private void ViewModel_ContextMenuRequested(object? sender, ArtifactContextMenuEventArgs e)
        {
            ShowContextMenu(e);
        }

        private void ViewModel_BeginRenameRequested(object? sender, IArtifact artifact)
        {
            if (_nodeMap.TryGetValue(artifact.Id, out var node))
            {
                treeView.SelectedNode = node;
                treeView.BeginEdit(node);
            }
        }

        private void ViewModel_ArtifactRefreshRequested(object? sender, IArtifact artifact)
        {
            RefreshNode(artifact);
        }

        private void RefreshTree()
        {
            treeView.BeginUpdate();
            try
            {
                treeView.Nodes.Clear();
                _nodeMap.Clear();

                if (_viewModel?.Workspace != null)
                {
                    var rootNode = CreateNode(_viewModel.Workspace);
                    treeView.Nodes.Add(rootNode);
                    rootNode.Expand();

                    var datasourcesNode = rootNode.Nodes.Cast<TreeNodeAdv>()
                        .FirstOrDefault(n => n.Tag is DatasourcesContainerArtifact);
                    datasourcesNode?.Expand();

                    treeView.SelectedNode = rootNode;
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

            artifact.PropertyChanged += (s, e) =>
            {
                if (node.TreeView == null || node.TreeView.IsDisposed) return;

                if (e.PropertyName == nameof(artifact.TreeNodeText))
                {
                    node.Text = artifact.TreeNodeText;
                }
                else if (e.PropertyName == nameof(artifact.TreeNodeIcon))
                {
                    UpdateNodeIcon(node, artifact);
                }
            };

            artifact.ChildAdded += (s, e) =>
            {
                if (node.TreeView == null || node.TreeView.IsDisposed) return;
                var childNode = CreateNode(e.ChildArtifact);
                node.Nodes.Add(childNode);
                node.Expand();
            };

            artifact.ChildRemoved += (s, e) =>
            {
                if (node.TreeView == null || node.TreeView.IsDisposed) return;
                if (_nodeMap.TryGetValue(e.ChildArtifact.Id, out var childNode))
                {
                    node.Nodes.Remove(childNode);
                    _nodeMap.Remove(e.ChildArtifact.Id);
                }
            };

            UpdateNodeIcon(node, artifact);
            _nodeMap[artifact.Id] = node;

            foreach (var child in artifact.Children)
            {
                var childNode = CreateNode(child);
                node.Nodes.Add(childNode);
            }

            return node;
        }

        private void UpdateNodeIcon(TreeNodeAdv node, IArtifact artifact)
        {
            try
            {
                var icon = artifact.TreeNodeIcon?.GetIcon();
                if (icon != null)
                {
                    node.LeftImageIndices = new int[] { GetOrAddImageIndex(artifact.TreeNodeIcon.IconKey, icon) };
                }
            }
            catch { }
        }

        private int GetOrAddImageIndex(string key, Image icon)
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
                _viewModel.SelectArtifactCommand.Execute(artifact);
                //_viewModel.SelectedArtifact = artifact;
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

        private async void TreeView_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            var node = treeView.GetNodeAtPoint(e.Location);
            if (node?.Tag is IArtifact artifact && _viewModel != null)
            {
                await _viewModel.HandleDoubleClickAsync(artifact);
            }
        }

        private void TreeView_BeforeEdit(object sender, TreeNodeAdvBeforeEditEventArgs e)
        {
            if (e.Node.Tag is IArtifact artifact)
            {
                e.Cancel = !(artifact is IEditableTreeNode);
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void TreeView_NodeEditorValidating(object? sender, TreeNodeAdvCancelableEditEventArgs e)
        {
            if (e.Node?.Tag is IArtifact artifact)
            {
                if (!(artifact is IEditableTreeNode))
                {
                    e.Cancel = true;
                    e.ContinueEditing = false;
                }
            }
            else
            {
                e.Cancel = true;
                e.ContinueEditing = false;
            }
        }

        private void TreeView_NodeEditorValidated(object? sender, TreeNodeAdvEditEventArgs e)
        {
            if (e.Node?.Tag is not IArtifact artifact || _viewModel == null) return;

            var newName = e.Label?.Trim();

            if (string.IsNullOrWhiteSpace(newName))
            {
                newName = GetDefaultName(artifact);
                e.Node.Text = newName;
            }

            var oldName = GetArtifactName(artifact);
            if (oldName != newName)
            {
                SetArtifactName(artifact, newName);
                //_viewModel.OnArtifactRenamed(artifact, oldName, newName);
            }
        }

        private string GetArtifactName(IArtifact artifact)
        {
            return artifact switch
            {
                WorkspaceArtifact wa => wa.Name,
                DatasourceArtifact da => da.Name,
                TableArtifact ta => ta.Name,
                ViewArtifact va => va.Name,
                ColumnArtifact ca => ca.Name,
                IndexArtifact ia => ia.Name,
                _ => artifact.TreeNodeText
            };
        }

        private void SetArtifactName(IArtifact artifact, string newName)
        {
            switch (artifact)
            {
                case WorkspaceArtifact wa:
                    wa.Name = newName;
                    break;
                case DatasourceArtifact da:
                    da.Name = newName;
                    break;
                case TableArtifact ta:
                    ta.Name = newName;
                    break;
                case ViewArtifact va:
                    va.Name = newName;
                    break;
                case ColumnArtifact ca:
                    ca.Name = newName;
                    break;
                case IndexArtifact ia:
                    ia.Name = newName;
                    break;
            }
        }

        private string GetDefaultName(IArtifact artifact)
        {
            return artifact switch
            {
                WorkspaceArtifact => "Workspace",
                DatasourceArtifact => "New Datasource",
                TableArtifact => "NewTable",
                ViewArtifact => "NewView",
                ColumnArtifact => "NewColumn",
                IndexArtifact => "NewIndex",
                _ => "Unnamed"
            };
        }

        private void ShowContextMenu(ArtifactContextMenuEventArgs e)
        {
            var contextMenu = new ContextMenuStrip();

            foreach (var command in e.Commands)
            {
                contextMenu.Items.Add(CreateMenuItem(command, e.Artifact));
            }

            if (contextMenu.Items.Count > 0)
            {
                contextMenu.Show(treeView, new Point(e.X, e.Y));
            }
        }

        private ToolStripItem CreateMenuItem(ArtifactTreeNodeCommand command, IArtifact artifact)
        {
            if (command.IsSeparator)
            {
                return new ToolStripSeparator();
            }

            var menuItem = new ToolStripMenuItem(command.Text);

            if (command.CanExecute != null)
            {
                menuItem.Enabled = command.CanExecute(artifact);
            }

            if (command.Execute != null)
            {
                menuItem.Click += async (s, args) => await command.Execute(artifact);
            }

            if (command.SubCommands != null && command.SubCommands.Any())
            {
                foreach (var subCommand in command.SubCommands)
                {
                    menuItem.DropDownItems.Add(CreateMenuItem(subCommand, artifact));
                }
            }

            return menuItem;
        }

        public void RefreshNode(IArtifact artifact)
        {
            if (_nodeMap.TryGetValue(artifact.Id, out var node))
            {
                node.Text = artifact.TreeNodeText;
            }
        }

        public void BeginEditSelectedNode()
        {
            if (treeView.SelectedNode != null)
            {
                treeView.BeginEdit(treeView.SelectedNode);
            }
        }

        private void ctxArtifactMenu_Opening(object sender, CancelEventArgs e)
        {
            if (_viewModel?.SelectedArtifact == null)
            {
                e.Cancel = true;
                return;
            }

            var commands = _viewModel.GetContextMenuCommands(_viewModel.SelectedArtifact);
            ctxArtifactMenu.Items.Clear();

            foreach (var command in commands)
            {
                ctxArtifactMenu.Items.Add(CreateMenuItem(command, _viewModel.SelectedArtifact));
            }

            if (ctxArtifactMenu.Items.Count == 0)
            {
                e.Cancel = true;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _viewModel?.Cleanup();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
