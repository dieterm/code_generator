using CodeGenerator.Core.Workspaces.Datasources.DotNetAssembly.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views;

/// <summary>
/// View for editing .NET Assembly datasource properties
/// </summary>
public partial class DotNetAssemblyDatasourceEditView : UserControl, IView<DotNetAssemblyDatasourceEditViewModel>
{
    private DotNetAssemblyDatasourceEditViewModel? _viewModel;
    private CancellationTokenSource? _loadingCts;

    public DotNetAssemblyDatasourceEditView()
    {
        InitializeComponent();

        btnAnalyze.Click += BtnAnalyze_Click;
        btnImportSelected.Click += BtnImportSelected_Click;
        btnImportAll.Click += BtnImportAll_Click;
        txtSearch.TextChanged += TxtSearch_TextChanged;
        treeTypes.AfterSelect += TreeTypes_AfterSelect;
        treeTypes.NodeMouseDoubleClick += TreeTypes_NodeMouseDoubleClick;
    }

    public void BindViewModel(DotNetAssemblyDatasourceEditViewModel viewModel)
    {
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        _viewModel = viewModel;

        if (_viewModel == null) return;

        // Bind fields
        txtName.BindViewModel(_viewModel.NameField);
        fileField.BindViewModel(_viewModel.FilePathField);

        // Subscribe to events
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;

        UpdateUI();
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (InvokeRequired)
        {
            Invoke(() => ViewModel_PropertyChanged(sender, e));
            return;
        }

        if (e.PropertyName == nameof(DotNetAssemblyDatasourceEditViewModel.IsLoadingFile))
        {
            UpdateUI();
        }
        else if (e.PropertyName == nameof(DotNetAssemblyDatasourceEditViewModel.StatusMessage))
        {
            lblStatus.Text = _viewModel?.StatusMessage ?? string.Empty;
        }
        else if (e.PropertyName == nameof(DotNetAssemblyDatasourceEditViewModel.ErrorMessage))
        {
            lblError.Text = _viewModel?.ErrorMessage ?? string.Empty;
            lblError.Visible = !string.IsNullOrEmpty(_viewModel?.ErrorMessage);
        }
        else if (e.PropertyName == nameof(DotNetAssemblyDatasourceEditViewModel.FilteredTypes))
        {
            RefreshTypeTree();
        }
        else if (e.PropertyName == nameof(DotNetAssemblyDatasourceEditViewModel.AssemblyInfo))
        {
            RefreshAssemblyInfo();
        }
    }

    private void RefreshAssemblyInfo()
    {
        if (_viewModel?.AssemblyInfo == null)
        {
            lblAssemblyInfo.Text = string.Empty;
            btnImportAll.Enabled = false;
            return;
        }

        var info = _viewModel.AssemblyInfo;
        lblAssemblyInfo.Text = info.DisplayInfo;
        btnImportAll.Enabled = info.TypeCount > 0;

        RefreshTypeTree();
    }

    private void RefreshTypeTree()
    {
        treeTypes.BeginUpdate();
        try
        {
            treeTypes.Nodes.Clear();

            if (_viewModel?.FilteredTypes == null || !_viewModel.FilteredTypes.Any())
            {
                btnImportSelected.Enabled = false;
                return;
            }

            // Group types by namespace
            var groupedTypes = _viewModel.FilteredTypes
                .GroupBy(t => t.Namespace ?? "(No Namespace)")
                .OrderBy(g => g.Key);

            foreach (var group in groupedTypes)
            {
                var namespaceNode = new TreeNode(group.Key)
                {
                    ImageKey = "folder",
                    SelectedImageKey = "folder"
                };

                foreach (var typeInfo in group.OrderBy(t => t.Name))
                {
                    var typeNode = new TreeNode(GetTypeDisplayText(typeInfo))
                    {
                        Tag = typeInfo,
                        ImageKey = typeInfo.IconKey,
                        SelectedImageKey = typeInfo.IconKey
                    };

                    // Add property nodes
                    foreach (var prop in typeInfo.Properties)
                    {
                        var propNode = new TreeNode($"{prop.Name}: {prop.TypeDisplay}")
                        {
                            Tag = prop,
                            ImageKey = "property",
                            SelectedImageKey = "property"
                        };
                        typeNode.Nodes.Add(propNode);
                    }

                    namespaceNode.Nodes.Add(typeNode);
                }

                treeTypes.Nodes.Add(namespaceNode);
            }

            // Expand all namespace nodes
            foreach (TreeNode node in treeTypes.Nodes)
            {
                node.Expand();
            }
        }
        finally
        {
            treeTypes.EndUpdate();
        }
    }

    private string GetTypeDisplayText(AssemblyTypeInfoViewModel typeInfo)
    {
        var suffix = typeInfo.PropertyCount > 0 ? $" ({typeInfo.PropertyCount} properties)" : "";
        return $"{typeInfo.Name} [{typeInfo.TypeKind}]{suffix}";
    }

    private void UpdateUI()
    {
        var isLoading = _viewModel?.IsLoadingFile ?? false;

        btnAnalyze.Enabled = !isLoading;
        btnAnalyze.Text = isLoading ? "Analyzing..." : "Analyze Assembly";

        grpFile.Enabled = !isLoading;
        btnImportAll.Enabled = !isLoading && _viewModel?.AssemblyInfo != null && _viewModel.AssemblyInfo.TypeCount > 0;
        btnImportSelected.Enabled = !isLoading && treeTypes.SelectedNode?.Tag is AssemblyTypeInfoViewModel;
    }

    private void TxtSearch_TextChanged(object? sender, EventArgs e)
    {
        if (_viewModel != null)
        {
            _viewModel.SearchFilter = txtSearch.Text;
        }
    }

    private void TreeTypes_AfterSelect(object? sender, TreeViewEventArgs e)
    {
        btnImportSelected.Enabled = e.Node?.Tag is AssemblyTypeInfoViewModel;
    }

    private async void TreeTypes_NodeMouseDoubleClick(object? sender, TreeNodeMouseClickEventArgs e)
    {
        if (e.Node?.Tag is AssemblyTypeInfoViewModel typeInfo)
        {
            await ImportTypeAsync(typeInfo.FullName);
        }
    }

    private async void BtnAnalyze_Click(object? sender, EventArgs e)
    {
        if (_viewModel == null) return;

        _loadingCts?.Cancel();
        _loadingCts = new CancellationTokenSource();

        try
        {
            await _viewModel.LoadAssemblyInfoAsync(_loadingCts.Token);
        }
        catch (OperationCanceledException)
        {
            // Cancelled, ignore
        }
    }

    private async void BtnImportSelected_Click(object? sender, EventArgs e)
    {
        if (_viewModel == null) return;

        if (treeTypes.SelectedNode?.Tag is AssemblyTypeInfoViewModel typeInfo)
        {
            await ImportTypeAsync(typeInfo.FullName);
        }
    }

    private async void BtnImportAll_Click(object? sender, EventArgs e)
    {
        if (_viewModel == null) return;

        try
        {
            // Import only the filtered types if there's a search filter
            var typeNames = _viewModel.FilteredTypes?.Select(t => t.FullName).ToList();
            await _viewModel.ImportAllTypesAsync(typeNames);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error importing types: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task ImportTypeAsync(string typeName)
    {
        if (_viewModel == null) return;

        try
        {
            await _viewModel.ImportTypeAsync(typeName);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error importing type: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
    {
        BindViewModel((DotNetAssemblyDatasourceEditViewModel)(object)viewModel);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _loadingCts?.Cancel();
            _loadingCts?.Dispose();

            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }

            components?.Dispose();
        }
        base.Dispose(disposing);
    }
}
