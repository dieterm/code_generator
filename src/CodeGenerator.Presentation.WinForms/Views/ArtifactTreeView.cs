using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.ViewModels.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.Events;
using CodeGenerator.Core.Artifacts.Views;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.Shared.Views.TreeNode;
using DocumentFormat.OpenXml.Office2010.CustomUI;
using Syncfusion.Windows.Forms.Tools;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views;

/// <summary>
/// Generic TreeView control for displaying artifacts with support for
/// context menus, icons, label editing, and dynamic updates
/// </summary>
public partial class ArtifactTreeView : UserControl
{
    private IArtifactTreeViewModel? _viewModel;
    private readonly Dictionary<string, TreeNodeAdv> _nodeMap = new();

    public ArtifactTreeView()
    {
        InitializeComponent();
        SetupTreeView();
    }

    /// <summary>
    /// The ViewModel
    /// </summary>
    public IArtifactTreeViewModel? ViewModel
    {
        get => _viewModel;
        set => BindViewModel(value);
    }

    private void SetupTreeView()
    {
        TreeView.AfterSelect += TreeView_AfterSelect;
        TreeView.MouseClick += TreeView_MouseClick;
        TreeView.MouseDoubleClick += TreeView_MouseDoubleClick;
        TreeView.BeforeEdit += TreeView_BeforeEdit;
        TreeView.NodeEditorValidating += TreeView_NodeEditorValidating;
        TreeView.NodeEditorValidated += TreeView_NodeEditorValidated;
        ContextMenu.Opening += ContextMenu_Opening;
        TreeView.ContextMenuStrip = ContextMenu;
    }

    public virtual void BindViewModel(IArtifactTreeViewModel? viewModel)
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

    protected virtual void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IArtifactTreeViewModel.RootArtifact))
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
            TreeView.SelectedNode = node;
            TreeView.BeginEdit(node);
        }
    }

    private void ViewModel_ArtifactRefreshRequested(object? sender, IArtifact artifact)
    {
        RefreshNode(artifact);
    }

    /// <summary>
    /// Refresh the entire tree
    /// </summary>
    protected virtual void RefreshTree()
    {
        TreeView.BeginUpdate();
        try
        {
            TreeView.Nodes.Clear();
            _nodeMap.Clear();

            if (_viewModel?.RootArtifact != null)
            {
                var rootNode = CreateNode(_viewModel.RootArtifact);
                TreeView.Nodes.Add(rootNode);
                rootNode.Expand();

                // Optionally expand more nodes - override in derived classes
                OnTreeRefreshed(rootNode);
            }
        }
        finally
        {
            TreeView.EndUpdate();
        }
    }

    /// <summary>
    /// Called after tree refresh, can be overridden to expand specific nodes
    /// </summary>
    protected virtual void OnTreeRefreshed(TreeNodeAdv rootNode)
    {
        // Override in derived classes to expand specific nodes
    }

    /// <summary>
    /// Create a tree node for an artifact
    /// </summary>
    protected virtual TreeNodeAdv CreateNode(IArtifact artifact)
    {
        var node = new TreeNodeAdv(artifact.TreeNodeText)
        {
            Tag = artifact
        };

        if(artifact.TreeNodeTextColor != null)
        {
            node.TextColor = artifact.TreeNodeTextColor.Value;
        }

        // Subscribe to property changes
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

        // Subscribe to child changes
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

        // Set icon
        UpdateNodeIcon(node, artifact);

        _nodeMap[artifact.Id] = node;

        // Add children
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
        catch
        {
            // Ignore icon errors
        }
    }

    private int GetOrAddImageIndex(string key, Image icon)
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

    private void TreeView_AfterSelect(object? sender, EventArgs e)
    {
        if (TreeView.SelectedNode?.Tag is IArtifact artifact && _viewModel != null)
        {
            _viewModel.SelectArtifactCommand.Execute(artifact);
        }
    }

    private void TreeView_MouseClick(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            var node = TreeView.GetNodeAtPoint(e.Location);
            if (node?.Tag is IArtifact artifact && _viewModel != null)
            {
                TreeView.SelectedNode = node;
                _viewModel.RequestContextMenu(artifact, e.X, e.Y);
            }
        }
    }

    private async void TreeView_MouseDoubleClick(object? sender, MouseEventArgs e)
    {
        var node = TreeView.GetNodeAtPoint(e.Location);
        if (node?.Tag is IArtifact artifact && _viewModel != null)
        {
            await _viewModel.HandleDoubleClickAsync(artifact);
        }
    }

    private void TreeView_BeforeEdit(object sender, TreeNodeAdvBeforeEditEventArgs e)
    {
        if (e.Node.Tag is IEditableTreeNode artifact && _viewModel != null)
        {
            e.Cancel = !artifact.CanBeginEdit();
        }
        else
        {
            e.Cancel = true;
        }
    }

    private void TreeView_NodeEditorValidating(object? sender, TreeNodeAdvCancelableEditEventArgs e)
    {
        if (e.Node?.Tag is IEditableTreeNode artifact && _viewModel != null)
        {
            if (!artifact.Validating(e.Label?.Trim()))
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
        if (e.Node?.Tag is not IEditableTreeNode artifact || _viewModel == null) return;

        var newName = e.Label?.Trim();

        if (string.IsNullOrWhiteSpace(newName))
        {
            newName = artifact.TreeNodeText;
            e.Node.Text = newName;
        }

        var oldName = artifact.TreeNodeText;
        if (oldName != newName)
        {
            //SetArtifactName(artifact, newName);
            //_viewModel.OnArtifactRenamed(artifact, oldName, newName);
            artifact.EndEdit(oldName, newName);
        }
    }
    /// <summary>
    /// There are 2 places where the context menu is shown:
    /// 1. When right-clicking on a tree node
    /// 2. When the treeviewcontroller fires the ContextMenuRequested event
    /// This method handles the first case
    /// </summary>
    private void ContextMenu_Opening(object? sender, CancelEventArgs e)
    {
        if (_viewModel?.SelectedArtifact == null)
        {
            e.Cancel = true;
            return;
        }

        var commands = _viewModel.GetContextMenuCommands(_viewModel.SelectedArtifact);
        RefreshContextMenu(commands, _viewModel.SelectedArtifact);

        if (ContextMenu.Items.Count == 0)
        {
            e.Cancel = true;
        }
    }

    private void RefreshContextMenu(IEnumerable<ArtifactTreeNodeCommand> commands, IArtifact artifact)
    {
        // Clear previous items instead of creating a new ContextMenuStrip
        ContextMenu.Items.Clear();

        // group commands by GroupName
        // add seperator between each group
        // put "Default" group first, then "Edit", "Rename", "Manage", "Delete"
        var commandGroups = new List<ArtifactTreeNodeCommandGroup>();
        foreach (var command in commands)
        {
            if (!commandGroups.Any(c => c.Name == command.GroupName))
            {
                commandGroups.Add(new ArtifactTreeNodeCommandGroup(command.GroupName));
            }
            commandGroups.Single(c => c.Name == command.GroupName).Commands.Add(command);
        }

        foreach (var commandGroup in commandGroups.OrderBy(c => c.SortOrder))
        {
            if (ContextMenu.Items.Count > 0)
            {
                ContextMenu.Items.Add(new ToolStripSeparator());
            }

            foreach (var command in commandGroup.Commands)
            {
                ContextMenu.Items.Add(CreateMenuItem(command, artifact));
            }
        }
    }

    /// <summary>
    /// There are 2 places where the context menu is shown:
    /// 1. When right-clicking on a tree node
    /// 2. When the treeviewcontroller fires the ContextMenuRequested event
    /// This method handles the second case
    /// </summary>
    private void ShowContextMenu(ArtifactContextMenuEventArgs e)
    {
        RefreshContextMenu(e.Commands, e.Artifact);
       
        if (ContextMenu.Items.Count > 0)
        {
            ContextMenu.Show(TreeView, new Point(e.X, e.Y));
        }
    }

    private ToolStripItem CreateMenuItem(ArtifactTreeNodeCommand command, IArtifact artifact)
    {
        //if (command.IsSeparator)
        //{
        //    return new ToolStripSeparator();
        //}

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

    /// <summary>
    /// Start editing the label of the currently selected node
    /// </summary>
    public void BeginEditSelectedNode()
    {
        if (TreeView.SelectedNode != null)
        {
            TreeView.BeginEdit(TreeView.SelectedNode);
        }
    }

    /// <summary>
    /// Get a node by artifact ID
    /// </summary>
    protected TreeNodeAdv? GetNode(string artifactId)
    {
        return _nodeMap.TryGetValue(artifactId, out var node) ? node : null;
    }

    /// <summary>
    /// Select an artifact in the tree
    /// </summary>
    public void SelectArtifact(IArtifact artifact)
    {
        if (_nodeMap.TryGetValue(artifact.Id, out var node))
        {
            TreeView.SelectedNode = node;
        }
    }

    /// <summary>
    /// Get the name property value from an artifact - override in derived classes
    /// </summary>
    //public virtual string GetArtifactName(IArtifact artifact)
    //{
    //    return _viewModel?.GetArtifactName(artifact) ?? artifact.TreeNodeText;
    //}

    /// <summary>
    /// Set the name property value on an artifact - override in derived classes
    /// </summary>
    //public virtual void SetArtifactName(IArtifact artifact, string newName)
    //{
    //    _viewModel?.SetArtifactName(artifact, newName);
    //}

    /// <summary>
    /// Get a default name for an artifact type - override in derived classes
    /// </summary>
    //public virtual string GetDefaultName(IArtifact artifact)
    //{
    //    return _viewModel?.GetDefaultName(artifact) ?? "Unnamed";
    //}

    /// <summary>
    /// Check if an artifact can be renamed - override in derived classes
    /// </summary>
    //public virtual bool CanRename(IArtifact artifact)
    //{
    //    return _viewModel?.CanRename(artifact) ?? false;
    //}

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _viewModel?.Cleanup();
            components?.Dispose();
        }
        base.Dispose(disposing);
    }

    //public void BindViewModel(TViewModel viewModel)
    //{
    //    throw new NotImplementedException();
    //}

}
