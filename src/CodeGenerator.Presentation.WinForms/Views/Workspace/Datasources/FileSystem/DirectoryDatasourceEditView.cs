using CodeGenerator.Core.Workspaces.Datasources.Directory.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views;

/// <summary>
/// View for editing Directory datasource properties
/// </summary>
public partial class DirectoryDatasourceEditView : UserControl, IView<DirectoryDatasourceEditViewModel>
{
    private DirectoryDatasourceEditViewModel? _viewModel;
    private CancellationTokenSource? _scanningCts;

    public DirectoryDatasourceEditView()
    {
        InitializeComponent();

        btnScan.Click += BtnScan_Click;
        btnImport.Click += BtnImport_Click;
    }

    public void BindViewModel(DirectoryDatasourceEditViewModel viewModel)
    {
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        _viewModel = viewModel;

        if (_viewModel == null) return;

        // Bind fields
        txtName.BindViewModel(_viewModel.NameField);
        folderField.BindViewModel(_viewModel.DirectoryPathField);
        txtSearchPattern.BindViewModel(_viewModel.SearchPatternField);
        chkIncludeSubdirectories.BindViewModel(_viewModel.IncludeSubdirectoriesField);

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

        if (e.PropertyName == nameof(DirectoryDatasourceEditViewModel.IsScanning))
        {
            UpdateUI();
        }
        else if (e.PropertyName == nameof(DirectoryDatasourceEditViewModel.StatusMessage))
        {
            lblStatus.Text = _viewModel?.StatusMessage ?? string.Empty;
        }
        else if (e.PropertyName == nameof(DirectoryDatasourceEditViewModel.ErrorMessage))
        {
            lblError.Text = _viewModel?.ErrorMessage ?? string.Empty;
            lblError.Visible = !string.IsNullOrEmpty(_viewModel?.ErrorMessage);
        }
        else if (e.PropertyName == nameof(DirectoryDatasourceEditViewModel.DirectorySummary))
        {
            RefreshDirectorySummary();
        }
    }

    private void RefreshDirectorySummary()
    {
        lstSummary.Items.Clear();

        if (_viewModel?.DirectorySummary == null)
        {
            lblDirectoryInfo.Text = string.Empty;
            btnImport.Enabled = false;
            return;
        }

        var summary = _viewModel.DirectorySummary;
        lblDirectoryInfo.Text = summary.DirectoryName;

        // Add summary items
        AddSummaryItem("Directory Name", summary.DirectoryName);
        AddSummaryItem("Files (in root)", summary.FileCount.ToString());
        AddSummaryItem("Total Files", summary.TotalFileCount.ToString());
        AddSummaryItem("Subdirectories", summary.DirectoryCount.ToString());
        AddSummaryItem("Total Directories", summary.TotalDirectoryCount.ToString());
        AddSummaryItem("Total Size", summary.TotalSizeFormatted);
        AddSummaryItem("Created", summary.CreationDate.ToString("g"));
        AddSummaryItem("Modified", summary.ModifiedDate.ToString("g"));

        if (!string.IsNullOrEmpty(summary.AccessError))
        {
            AddSummaryItem("Warning", summary.AccessError);
        }

        btnImport.Enabled = summary.Exists;
    }

    private void AddSummaryItem(string name, string value)
    {
        var item = new ListViewItem(name);
        item.SubItems.Add(value);
        lstSummary.Items.Add(item);
    }

    private void UpdateUI()
    {
        var isScanning = _viewModel?.IsScanning ?? false;

        btnScan.Enabled = !isScanning;
        btnScan.Text = isScanning ? "Scanning..." : "Scan Directory";

        grpDirectory.Enabled = !isScanning;
        btnImport.Enabled = !isScanning && _viewModel?.DirectorySummary != null && _viewModel.DirectorySummary.Exists;
    }

    private async void BtnScan_Click(object? sender, EventArgs e)
    {
        if (_viewModel == null) return;

        _scanningCts?.Cancel();
        _scanningCts = new CancellationTokenSource();

        try
        {
            await _viewModel.ScanDirectoryAsync(_scanningCts.Token);
        }
        catch (OperationCanceledException)
        {
            // Cancelled, ignore
        }
    }

    private async void BtnImport_Click(object? sender, EventArgs e)
    {
        if (_viewModel == null) return;

        try
        {
            await _viewModel.ImportTableAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error importing directory: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
    {
        BindViewModel((DirectoryDatasourceEditViewModel)(object)viewModel);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _scanningCts?.Cancel();
            _scanningCts?.Dispose();

            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }

            components?.Dispose();
        }
        base.Dispose(disposing);
    }
}
