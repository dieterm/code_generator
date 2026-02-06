using CodeGenerator.Core.Workspaces.Datasources.Json.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views;

/// <summary>
/// View for editing JSON datasource properties
/// </summary>
public partial class JsonDatasourceEditView : UserControl, IView<JsonDatasourceEditViewModel>
{
    private JsonDatasourceEditViewModel? _viewModel;
    private CancellationTokenSource? _loadingCts;

    public JsonDatasourceEditView()
    {
        InitializeComponent();

        btnAnalyze.Click += BtnAnalyze_Click;
        btnImport.Click += BtnImport_Click;
    }

    public void BindViewModel(JsonDatasourceEditViewModel viewModel)
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

        if (e.PropertyName == nameof(JsonDatasourceEditViewModel.IsLoadingFile))
        {
            UpdateUI();
        }
        else if (e.PropertyName == nameof(JsonDatasourceEditViewModel.StatusMessage))
        {
            lblStatus.Text = _viewModel?.StatusMessage ?? string.Empty;
        }
        else if (e.PropertyName == nameof(JsonDatasourceEditViewModel.ErrorMessage))
        {
            lblError.Text = _viewModel?.ErrorMessage ?? string.Empty;
            lblError.Visible = !string.IsNullOrEmpty(_viewModel?.ErrorMessage);
        }
        else if (e.PropertyName == nameof(JsonDatasourceEditViewModel.FileInfo))
        {
            RefreshFileInfo();
        }
    }

    private void RefreshFileInfo()
    {
        lstProperties.Items.Clear();

        if (_viewModel?.FileInfo == null)
        {
            lblFileInfo.Text = string.Empty;
            btnImport.Enabled = false;
            return;
        }

        var fileInfo = _viewModel.FileInfo;
        lblFileInfo.Text = fileInfo.DisplayName;

        foreach (var prop in fileInfo.Properties)
        {
            var item = new ListViewItem(prop.Name);
            item.SubItems.Add(prop.TypeDisplay);
            item.SubItems.Add(prop.IsNullable ? "Yes" : "No");
            lstProperties.Items.Add(item);
        }

        btnImport.Enabled = fileInfo.PropertyCount > 0;
    }

    private void UpdateUI()
    {
        var isLoading = _viewModel?.IsLoadingFile ?? false;

        btnAnalyze.Enabled = !isLoading;
        btnAnalyze.Text = isLoading ? "Analyzing..." : "Analyze File";

        grpFile.Enabled = !isLoading;
        btnImport.Enabled = !isLoading && _viewModel?.FileInfo != null && _viewModel.FileInfo.PropertyCount > 0;
    }

    private async void BtnAnalyze_Click(object? sender, EventArgs e)
    {
        if (_viewModel == null) return;

        _loadingCts?.Cancel();
        _loadingCts = new CancellationTokenSource();

        try
        {
            await _viewModel.LoadFileInfoAsync(_loadingCts.Token);
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
            MessageBox.Show($"Error importing JSON: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
    {
        BindViewModel((JsonDatasourceEditViewModel)(object)viewModel);
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
