using CodeGenerator.Application.ViewModels.Generation;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;

namespace CodeGenerator.Presentation.WinForms.Views.Generation
{
    /// <summary>
    /// View for displaying artifact property details in a Syncfusion MultiColumnTreeView.
    /// </summary>
    public partial class GenerationDetailsView : UserControl, IView<GenerationDetailsViewModel>
    {
        private GenerationDetailsViewModel? _viewModel;
        private Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.MultiColumnTreeView artifactDetailsTreeView;

        public GenerationDetailsView()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            artifactDetailsTreeView = new Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.MultiColumnTreeView();
            ((ISupportInitialize)artifactDetailsTreeView).BeginInit();
            SuspendLayout();

            artifactDetailsTreeView.AutoAdjustMultiLineHeight = true;
            artifactDetailsTreeView.AutoGenerateColumns = true;
            artifactDetailsTreeView.Dock = DockStyle.Fill;
            artifactDetailsTreeView.Location = new Point(0, 0);
            artifactDetailsTreeView.Name = "artifactDetailsTreeView";
            artifactDetailsTreeView.TabIndex = 0;

            var firstColumnIdx = artifactDetailsTreeView.Columns.Add(
                new Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.TreeColumnAdv("Property"));
            artifactDetailsTreeView.Columns[firstColumnIdx].Width = 200;

            var secondColumnIdx = artifactDetailsTreeView.Columns.Add(
                new Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.TreeColumnAdv("Value"));
            artifactDetailsTreeView.Columns[secondColumnIdx].Width = 200;

            Controls.Add(artifactDetailsTreeView);
            Name = "GenerationDetailsView";

            ((ISupportInitialize)artifactDetailsTreeView).EndInit();
            ResumeLayout(false);
        }

        public void BindViewModel(GenerationDetailsViewModel viewModel)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }

            _viewModel = viewModel;

            if (_viewModel != null)
            {
                _viewModel.PropertyChanged += ViewModel_PropertyChanged;
                RefreshDetailsTreeView();
            }
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((GenerationDetailsViewModel)(object)viewModel);
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GenerationDetailsViewModel.SelectedArtifact))
            {
                if (InvokeRequired)
                {
                    Invoke(RefreshDetailsTreeView);
                    return;
                }
                RefreshDetailsTreeView();
            }
        }

        private void RefreshDetailsTreeView()
        {
            artifactDetailsTreeView.Nodes.Clear();

            var artifact = _viewModel?.SelectedArtifact;
            if (artifact == null) return;

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
                                    Text = key?.ToString(),
                                    Tag = entry
                                };
                                var subitem1 = new Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.TreeNodeAdvSubItem();
                                subitem1.Text = entry?.ToString() ?? "null";
                                itemNode.SubItems.Add(subitem1);
                                propertyNode.Nodes.Add(itemNode);
                            }
                        }
                        else if (value is ArtifactDecoratorCollection decoratorCollection)
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
                        else if (value is IArtifact artifactValue)
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
                    Debug.WriteLine($"{nameof(GenerationDetailsView)}: Error retrieving property '{property.Name}': {ex.Message}");
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_viewModel != null)
                {
                    _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
                }
            }
            base.Dispose(disposing);
        }
    }
}
