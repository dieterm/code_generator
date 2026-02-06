using CodeGenerator.Core.Workspaces.Datasources.Excel.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views;

/// <summary>
/// View for editing Excel datasource properties
/// </summary>
public partial class ExcelDatasourceEditView : UserControl, IView<ExcelDatasourceEditViewModel>
{
    private ExcelDatasourceEditViewModel? _viewModel;
    private CancellationTokenSource? _loadingCts;

    public ExcelDatasourceEditView()
    {
        InitializeComponent();

        btnLoadSheets.Click += BtnLoadSheets_Click;
        btnAddSheet.Click += BtnAddSheet_Click;
        btnAddAllSheets.Click += BtnAddAllSheets_Click;
        lstSheets.SelectedIndexChanged += LstSheets_SelectedIndexChanged;
    }

    public void BindViewModel(ExcelDatasourceEditViewModel viewModel)
    {
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            _viewModel.AvailableSheets.CollectionChanged -= AvailableSheets_CollectionChanged;
        }

        _viewModel = viewModel;

        if (_viewModel == null) return;

        // Bind fields
        txtName.BindViewModel(_viewModel.NameField);
        fileField.BindViewModel(_viewModel.FilePathField);
        chkFirstRowIsHeader.BindViewModel(_viewModel.FirstRowIsHeaderField);

        // Subscribe to events
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        _viewModel.AvailableSheets.CollectionChanged += AvailableSheets_CollectionChanged;

        UpdateUI();
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (InvokeRequired)
        {
            Invoke(() => ViewModel_PropertyChanged(sender, e));
            return;
        }

        if (e.PropertyName == nameof(ExcelDatasourceEditViewModel.IsLoadingSheets))
        {
            UpdateUI();
        }
        else if (e.PropertyName == nameof(ExcelDatasourceEditViewModel.StatusMessage))
        {
            lblStatus.Text = _viewModel?.StatusMessage ?? string.Empty;
        }
        else if (e.PropertyName == nameof(ExcelDatasourceEditViewModel.ErrorMessage))
        {
            lblError.Text = _viewModel?.ErrorMessage ?? string.Empty;
            lblError.Visible = !string.IsNullOrEmpty(_viewModel?.ErrorMessage);
        }
        else if (e.PropertyName == nameof(ExcelDatasourceEditViewModel.SelectedSheet))
        {
            btnAddSheet.Enabled = _viewModel?.SelectedSheet != null;
        }
    }

    private void AvailableSheets_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (InvokeRequired)
        {
            Invoke(() => AvailableSheets_CollectionChanged(sender, e));
            return;
        }

        RefreshSheetsList();
    }

    private void RefreshSheetsList()
    {
        lstSheets.Items.Clear();

        if (_viewModel == null) return;

        foreach (var sheet in _viewModel.AvailableSheets)
        {
            var item = new ListViewItem(sheet.Name)
            {
                Tag = sheet,
                ImageKey = sheet.TypeIcon
            };
            item.SubItems.Add(sheet.ColumnCount.ToString());
            item.SubItems.Add(sheet.RowCount.ToString());
            lstSheets.Items.Add(item);
        }

        btnAddAllSheets.Enabled = lstSheets.Items.Count > 0;
    }

    private void UpdateUI()
    {
        var isLoading = _viewModel?.IsLoadingSheets ?? false;

        btnLoadSheets.Enabled = !isLoading;
        btnLoadSheets.Text = isLoading ? "Loading..." : "Load Sheets";

        grpFile.Enabled = !isLoading;
        btnAddSheet.Enabled = !isLoading && _viewModel?.SelectedSheet != null;
        btnAddAllSheets.Enabled = !isLoading && (_viewModel?.AvailableSheets.Count ?? 0) > 0;
    }

    private async void BtnLoadSheets_Click(object? sender, EventArgs e)
    {
        if (_viewModel == null) return;

        _loadingCts?.Cancel();
        _loadingCts = new CancellationTokenSource();

        try
        {
            await _viewModel.LoadSheetsAsync(_loadingCts.Token);
        }
        catch (OperationCanceledException)
        {
            // Cancelled, ignore
        }
    }

    private async void BtnAddSheet_Click(object? sender, EventArgs e)
    {
        if (_viewModel == null) return;

        try
        {
            await _viewModel.AddSelectedSheetAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error adding sheet: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void BtnAddAllSheets_Click(object? sender, EventArgs e)
    {
        if (_viewModel == null) return;

        try
        {
            await _viewModel.AddAllSheetsAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error adding sheets: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LstSheets_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_viewModel == null) return;

        _viewModel.SelectedSheet = lstSheets.SelectedItems.Count > 0
            ? lstSheets.SelectedItems[0].Tag as SheetViewModel
            : null;
    }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
    {
        BindViewModel((ExcelDatasourceEditViewModel)(object)viewModel);
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
                _viewModel.AvailableSheets.CollectionChanged -= AvailableSheets_CollectionChanged;
            }

            components?.Dispose();
        }
        base.Dispose(disposing);
    }
}
