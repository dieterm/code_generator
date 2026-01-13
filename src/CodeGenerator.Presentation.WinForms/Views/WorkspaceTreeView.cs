using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Application.ViewModels;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.Events;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Shared.Views;
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

        /// <summary>
        /// Event raised when a detail view should be shown
        /// </summary>
        //public event EventHandler<DetailViewRequestedEventArgs>? DetailViewRequested;

        /// <summary>
        /// Event raised when an artifact is renamed
        /// </summary>
        //public event EventHandler<ArtifactRenamedEventArgs>? ArtifactRenamed;

        private void SetupTreeView()
        {
            // Enable label editing
            treeView.LabelEdit = true;

            // Event handlers
            treeView.AfterSelect += TreeView_AfterSelect;
            treeView.MouseClick += TreeView_MouseClick;
            treeView.MouseDoubleClick += TreeView_MouseDoubleClick;
            treeView.BeforeEdit += TreeView_BeforeEdit;
            // Label edit events for Syncfusion TreeViewAdv
            treeView.NodeEditorValidating += TreeView_NodeEditorValidating;
            treeView.NodeEditorValidated += TreeView_NodeEditorValidated;
        }



        private void BindViewModel(WorkspaceTreeViewModel? viewModel)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
                _viewModel.ContextMenuRequested -= ViewModel_ContextMenuRequested;
                //_viewModel.DetailViewRequested -= ViewModel_DetailViewRequested;
                _viewModel.BeginRenameRequested -= ViewModel_BeginRenameRequested;
                _viewModel.ArtifactRefreshRequested -= ViewModel_ArtifactRefreshRequested;
            }

            _viewModel = viewModel;

            if (_viewModel != null)
            {
                _viewModel.PropertyChanged += ViewModel_PropertyChanged;
                _viewModel.ContextMenuRequested += ViewModel_ContextMenuRequested;
                //_viewModel.DetailViewRequested += ViewModel_DetailViewRequested;
                _viewModel.BeginRenameRequested += ViewModel_BeginRenameRequested;
                _viewModel.ArtifactRefreshRequested += ViewModel_ArtifactRefreshRequested;
            }

            RefreshTree();
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(WorkspaceTreeViewModel.Workspace))
            {
                RefreshTree();
            }
        }

        private void ViewModel_ContextMenuRequested(object? sender, ContextMenuRequestedEventArgs e)
        {
            ShowContextMenu(e);
        }

        //private void ViewModel_DetailViewRequested(object? sender, DetailViewRequestedEventArgs e)
        //{
        //    DetailViewRequested?.Invoke(this, e);
        //}

        private void ViewModel_BeginRenameRequested(object? sender, IArtifact artifact)
        {
            // Find the node for this artifact and begin editing
            if (_nodeMap.TryGetValue(artifact.Id, out var node))
            {
                treeView.SelectedNode = node;
                treeView.BeginEdit(node);
            }
        }

        private void ViewModel_ArtifactRefreshRequested(object? sender, IArtifact artifact)
        {
            // Refresh the node text for this artifact
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

                    // Expand datasources container
                    var datasourcesNode = rootNode.Nodes.Cast<TreeNodeAdv>()
                        .FirstOrDefault(n => n.Tag is DatasourcesContainerArtifact);
                    datasourcesNode?.Expand();

                    // Select the root node
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
            // Subscribe to property changed events
            artifact.PropertyChanged += (object? s, PropertyChangedEventArgs e) =>
            {
                if (node.TreeView == null || node.TreeView.IsDisposed)
                {
                    return;
                }
                if (e.PropertyName == nameof(artifact.TreeNodeText))
                {
                    node.Text = artifact.TreeNodeText;
                }
                else if (e.PropertyName == nameof(artifact.TreeNodeIcon))
                {
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
                }
            };
            artifact.ChildAdded += (object? s, ChildAddedEventArgs e) =>
            {
                if (node.TreeView == null || node.TreeView.IsDisposed)
                {
                    return;
                }
                var childNode = CreateNode(e.ChildArtifact);
                node.Nodes.Add(childNode);
                node.Expand();
            };
            artifact.ChildRemoved += (object? s, ChildRemovedEventArgs e) =>
            {
                if (node.TreeView == null || node.TreeView.IsDisposed)
                {
                    return;
                }
                var childNode = _nodeMap[e.ChildArtifact.Id];
                node.Nodes.Remove(childNode);
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
            if (e.Node.Tag is IEditableTreeNode)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void TreeView_NodeEditorValidating(object? sender, TreeNodeAdvCancelableEditEventArgs e)
        {
            // Only allow editing for certain artifact types
            if (e.Node?.Tag is IArtifact artifact)
            {
                // Allow editing for WorkspaceArtifact, DatasourceArtifact, TableArtifact, etc.
                var canEdit = artifact is IEditableTreeNode;
  
                if (!canEdit)
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
            if (e.Node?.Tag is not IArtifact artifact)
            {
                return;
            }

            var newName = e.Label?.Trim();

            // If empty or null, use default name based on artifact type
            if (string.IsNullOrWhiteSpace(newName))
            {
                newName = GetDefaultName(artifact);
                // Update the node text manually
                e.Node.Text = newName;
            }

            // Update the artifact name
            var oldName = GetArtifactName(artifact);
            if (oldName != newName)
            {
                SetArtifactName(artifact, newName);

                // Notify that the artifact was renamed
                //ArtifactRenamed?.Invoke(this, new ArtifactRenamedEventArgs(artifact, oldName, newName));

                // Notify ViewModel
                _viewModel?.OnArtifactRenamed(artifact, oldName, newName);
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

        /// <summary>
        /// Start editing the label of the currently selected node
        /// </summary>
        public void BeginEditSelectedNode()
        {
            if (treeView.SelectedNode != null)
            {
                treeView.BeginEdit(treeView.SelectedNode);
            }
        }

        private void ShowContextMenu(ContextMenuRequestedEventArgs e)
        {
            var contextMenu = new ContextMenuStrip();

            foreach (var command in e.Commands)
            {
                if (command.IsSeparator)
                {
                    contextMenu.Items.Add(new ToolStripSeparator());
                }
                else if (command.SubCommands != null && command.SubCommands.Any())
                {
                    var menuItem = new ToolStripMenuItem(command.Text);
                    foreach (var subCommand in command.SubCommands)
                    {
                        var subItem = CreateMenuItem(subCommand, e.Artifact);
                        menuItem.DropDownItems.Add(subItem);
                    }
                    contextMenu.Items.Add(menuItem);
                }
                else
                {
                    var menuItem = CreateMenuItem(command, e.Artifact);
                    contextMenu.Items.Add(menuItem);
                }
            }

            if (contextMenu.Items.Count > 0)
            {
                contextMenu.Show(treeView, new System.Drawing.Point(e.X, e.Y));
            }
        }

        private ToolStripItem CreateMenuItem(WorkspaceCommand command, IArtifact artifact)
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

            if(command.SubCommands != null && command.SubCommands.Any())
            {
                foreach (var subCommand in command.SubCommands)
                {
                    var subItem = CreateMenuItem(subCommand, artifact);
                    menuItem.DropDownItems.Add(subItem);
                }
            }

            return menuItem;
        }

        /// <summary>
        /// Refresh a specific node
        /// </summary>
        public void RefreshNode(IArtifact artifact)
        {
            if (_nodeMap.TryGetValue(artifact.Id, out var node))
            {
                node.Text = artifact.TreeNodeText;
            }
        }

        /// <summary>
        /// Add a new child node
        /// </summary>
        public void AddChildNode(IArtifact parent, IArtifact child)
        {
            if (_nodeMap.TryGetValue(parent.Id, out var parentNode))
            {
                var childNode = CreateNode(child);
                parentNode.Nodes.Add(childNode);
                parentNode.Expand();
            }
        }

        /// <summary>
        /// Remove a node
        /// </summary>
        public void RemoveNode(IArtifact artifact)
        {
            if (_nodeMap.TryGetValue(artifact.Id, out var node))
            {
                node.Parent?.Nodes.Remove(node);
                _nodeMap.Remove(artifact.Id);
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

        private void ctxArtifactMenu_Opening(object sender, CancelEventArgs e)
        {
            var commands = _viewModel.GetContextMenuCommands(_viewModel.SelectedArtifact);
            ctxArtifactMenu.Items.Clear();
            foreach (var command in commands)
            {
                ctxArtifactMenu.Items.Add(CreateMenuItem(command, _viewModel.SelectedArtifact));
                /*if (command.IsSeparator)
                {
                    ctxArtifactMenu.Items.Add(new ToolStripSeparator());
                }
                else if (command.SubCommands != null && command.SubCommands.Any())
                {
                    var menuItem = new ToolStripMenuItem(command.Text);
                    foreach (var subCommand in command.SubCommands)
                    {
                        var subItem = CreateMenuItem(subCommand, _viewModel.SelectedArtifact);
                        menuItem.DropDownItems.Add(subItem);
                    }
                    ctxArtifactMenu.Items.Add(menuItem);
                }
                else
                {
                    var menuItem = CreateMenuItem(command, _viewModel.SelectedArtifact);
                    ctxArtifactMenu.Items.Add(menuItem);
                }*/

            }
            if (ctxArtifactMenu.Items.Count == 0)
            {
                e.Cancel = true;
            }
        }
    }
}
