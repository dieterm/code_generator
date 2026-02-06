using CodeGenerator.Application.ViewModels.Generation;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.CodeGeneration;
using CodeGenerator.Core.Artifacts.TreeNode;
using Syncfusion.Windows.Forms.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeGenerator.Presentation.WinForms.Views
{
    public partial class GenerationResultTreeView : UserControl
    {
        private bool isSelectingArtifact = false;
        public GenerationResultTreeViewModel? ViewModel { get; private set; }
        public GenerationResultTreeView()
        {
            InitializeComponent();

            Disposed += (s, e) =>
            {
                if (ViewModel != null)
                {
                    ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
                }
            };

            var firstColumnIdx = artifactDetailsTreeView.Columns.Add(new Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.TreeColumnAdv("Property"));
            artifactDetailsTreeView.Columns[firstColumnIdx].Width = 200;
            var secondColumnIdx = artifactDetailsTreeView.Columns.Add(new Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.TreeColumnAdv("Value"));
            artifactDetailsTreeView.Columns[secondColumnIdx].Width = 200;
        }

        public void BindViewModel(GenerationResultTreeViewModel viewModel)
        {
            if (ViewModel != null)
            {
                ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }
            ViewModel = viewModel;
            RefreshTreeView();
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GenerationResultTreeViewModel.GenerationResult))
            {
                RefreshTreeView();
            }
            else if (e.PropertyName == nameof(GenerationResultTreeViewModel.SelectedArtifact))
            {
               // if(isSelectingArtifact) return;
                
                artifactDetailsTreeView.Nodes.Clear();
                // Select the corresponding node in the tree view
                if (ViewModel?.SelectedArtifact != null)
                {
                    
                    var nodeToSelect = FindNodeByArtifact(artifactTreeView.Nodes, ViewModel.SelectedArtifact);
                    if (nodeToSelect != null)
                    {
                        artifactTreeView.SelectedNode = nodeToSelect;
                        artifactTreeView.EnsureVisible(nodeToSelect);
                    }

                    // Show details in the details view
                    RefreshDetailsTreeView();
                }
            }
        }

        private void RefreshDetailsTreeView()
        {
            artifactDetailsTreeView.Nodes.Clear();

            var artifact = ViewModel.SelectedArtifact;
           
            foreach (var property in artifact.GetType().GetProperties())
            {
                try
                {
                    object? value = null;

                    try
                    {
                        value = property.GetValue(artifact);
                    }
                    catch (Exception)
                    {
                        //throw;
                    }
                   
                    
                    var propertyNode = new Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.TreeNodeAdv
                    {
                        Text = property.Name,
                        Tag = value
                    };
                    if (value != null)
                    {
                        if (value is IEnumerable<IArtifact> artifactCollection)
                        {
                            foreach (var item in artifactCollection)
                            {
                                var itemNode = new Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.TreeNodeAdv
                                {
                                    Text = item.TreeNodeText,
                                    Tag = item
                                };
                                propertyNode.Nodes.Add(itemNode);
                            }
                        }
                        else if (value is IDictionary dict)
                        {
                            foreach (var key in dict.Keys)
                            {
                                var entry = dict[key];
                                var itemNode = new Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.TreeNodeAdv
                                {
                                    Text = key.ToString(),
                                    Tag = entry
                                };
                                var subitem1 = new Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.TreeNodeAdvSubItem();
                                subitem1.Text = entry?.ToString() ??"null";
                                itemNode.SubItems.Add(subitem1);
                                propertyNode.Nodes.Add(itemNode);
                            }
                        }
                        else if(value is ArtifactDecoratorCollection decoratorCollection)
                        {
                            foreach (var decorator in decoratorCollection)
                            {
                                var itemNode = new Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.TreeNodeAdv
                                {
                                    Text = decorator.Key,
                                    Tag = decorator
                                };
                                var subitem1 = new Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.TreeNodeAdvSubItem();
                                subitem1.Text = decorator.GetType().FullName ?? "null";
                                itemNode.SubItems.Add(subitem1);
                                propertyNode.Nodes.Add(itemNode);
                            }
                        }
                        else if(value is IArtifact artifactValue)
                        {
                            var subitem1 = new Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.TreeNodeAdvSubItem();
                            subitem1.Text = $"{artifactValue.TreeNodeText} ({value.GetType().Name})";
                            propertyNode.SubItems.Add(subitem1);
                        }
                        else if (value is ResourceManagerTreeNodeIcon iconValue)
                        {
                            var subitem1 = new Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.TreeNodeAdvSubItem();
                            subitem1.Text = $"{iconValue.IconKey} ({value.GetType().Name})";
                            propertyNode.SubItems.Add(subitem1);
                        }
                        else
                        {
                            var subitem1 = new Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.TreeNodeAdvSubItem();
                            subitem1.Text = value.ToString();
                            propertyNode.SubItems.Add(subitem1);
                        }
                    }
                    else
                    {
                        var subitem1 = new Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.TreeNodeAdvSubItem();
                        subitem1.Text = "null";
                        propertyNode.SubItems.Add(subitem1);
                        
                    }
                    artifactDetailsTreeView.Nodes.Add(propertyNode);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{nameof(GenerationResultTreeView)}: Error retrieving property '{property.Name}': {ex.Message}");
                }
            }
        }

        private TreeNodeAdv? FindNodeByArtifact(TreeNodeAdvCollection nodes, IArtifact selectedArtifact)
        {
            foreach (TreeNodeAdv node in nodes)
            {
                if (node.Tag == selectedArtifact)
                {
                    return node;
                }
                var foundInChild = FindNodeByArtifact(node.Nodes, selectedArtifact);
                if (foundInChild != null)
                {
                    return foundInChild;
                }
            }
            return null;
        }

        private void RefreshTreeView()
        {
            // Clear existing nodes and images
            artifactTreeView.Nodes.Clear();
            imageList.Images.Clear();

            if (ViewModel == null || ViewModel.GenerationResult == null || ViewModel.GenerationResult.RootArtifact == null) return;

            var rootArtifact = ViewModel.GenerationResult.RootArtifact;

            // First Initialize ImageList
            InitializeImageList(rootArtifact);

            // Then build TreeNodes
            var rootNode = CreateTreeNodeFromArtifact(rootArtifact);
            artifactTreeView.Nodes.Add(rootNode);
            rootNode.Nodes.Sort();
            rootNode.Expand();
            foreach (var node in artifactTreeView.Nodes.Cast<TreeNodeAdv>())
            {
                node.Expand();
            }
        }

        private TreeNodeAdv CreateTreeNodeFromArtifact(IArtifact rootArtifact)
        {
            var treeNode = new TreeNodeAdv
            {
                Text = rootArtifact.TreeNodeText,
                LeftImageIndices = new int[] { artifactTreeView.LeftImageList.Images.IndexOfKey(rootArtifact.TreeNodeIcon.IconKey) },
                Tag = rootArtifact
            };
            foreach (var child in rootArtifact.Children)
            {
                var childNode = CreateTreeNodeFromArtifact(child);
                treeNode.Nodes.Add(childNode);
            }
            return treeNode;
        }

        private void InitializeImageList(IArtifact rootArtifact)
        {
            var nodeIconKey = rootArtifact.TreeNodeIcon.IconKey;

            if (!imageList.Images.ContainsKey(nodeIconKey))
            {
                var icon = rootArtifact.TreeNodeIcon.GetIcon();
                if (icon == null)
                    throw new InvalidOperationException($"Icon with key '{nodeIconKey}' could not be resolved.");
                imageList.Images.Add(nodeIconKey, icon);
            }
            // Recursively add child artifacts' icons
            foreach (var child in rootArtifact.Children)
            {
                InitializeImageList(child);
            }
        }
        
        private void artifactTreeView_NodeMouseClick(object sender, TreeViewAdvMouseClickEventArgs e)
        {
            isSelectingArtifact = true;
            ViewModel.SelectArtifactCommand.Execute(e.Node.Tag as IArtifact);
            isSelectingArtifact = false;
        }
    }
}
