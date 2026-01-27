using CodeGenerator.Core.Workspaces.Datasources.OpenApi.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views;

/// <summary>
/// View for editing OpenAPI datasource properties
/// </summary>
public partial class OpenApiDatasourceEditView : UserControl, IView<OpenApiDatasourceEditViewModel>
{
    private OpenApiDatasourceEditViewModel? _viewModel;
    private CancellationTokenSource? _loadingCts;

    public OpenApiDatasourceEditView()
    {
        InitializeComponent();

        btnAnalyze.Click += BtnAnalyze_Click;
        btnImportSelected.Click += BtnImportSelected_Click;
        btnImportAll.Click += BtnImportAll_Click;
        txtSearch.TextChanged += TxtSearch_TextChanged;
        lstSchemas.SelectedIndexChanged += LstSchemas_SelectedIndexChanged;
        lstSchemas.DoubleClick += LstSchemas_DoubleClick;
    }

    public void BindViewModel(OpenApiDatasourceEditViewModel viewModel)
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

        if (e.PropertyName == nameof(OpenApiDatasourceEditViewModel.IsLoadingFile))
        {
            UpdateUI();
        }
        else if (e.PropertyName == nameof(OpenApiDatasourceEditViewModel.StatusMessage))
        {
            lblStatus.Text = _viewModel?.StatusMessage ?? string.Empty;
        }
        else if (e.PropertyName == nameof(OpenApiDatasourceEditViewModel.ErrorMessage))
        {
            lblError.Text = _viewModel?.ErrorMessage ?? string.Empty;
            lblError.Visible = !string.IsNullOrEmpty(_viewModel?.ErrorMessage);
        }
        else if (e.PropertyName == nameof(OpenApiDatasourceEditViewModel.FilteredSchemas))
        {
            RefreshSchemaList();
        }
        else if (e.PropertyName == nameof(OpenApiDatasourceEditViewModel.FileInfo))
        {
            RefreshFileInfo();
        }
    }

    private void RefreshFileInfo()
    {
        if (_viewModel?.FileInfo == null)
        {
            lblFileInfo.Text = string.Empty;
            btnImportAll.Enabled = false;
            return;
        }

        var info = _viewModel.FileInfo;
        lblFileInfo.Text = info.DisplayName;
        btnImportAll.Enabled = info.SchemaCount > 0;

        RefreshSchemaList();
    }

    private void RefreshSchemaList()
    {
        lstSchemas.BeginUpdate();
        try
        {
            lstSchemas.Items.Clear();

            if (_viewModel?.FilteredSchemas == null || !_viewModel.FilteredSchemas.Any())
            {
                btnImportSelected.Enabled = false;
                lstProperties.Items.Clear();
                return;
            }

            foreach (var schema in _viewModel.FilteredSchemas.OrderBy(s => s.Name))
            {
                var item = new ListViewItem(schema.Name);
                item.SubItems.Add(schema.TypeDisplay);
                item.SubItems.Add(schema.PropertyCount.ToString());
                item.SubItems.Add(schema.Description ?? string.Empty);
                item.Tag = schema;
                lstSchemas.Items.Add(item);
            }
        }
        finally
        {
            lstSchemas.EndUpdate();
        }
    }

    private void RefreshPropertyList(OpenApiSchemaInfoViewModel schema)
    {
        lstProperties.BeginUpdate();
        try
        {
            lstProperties.Items.Clear();

            if (schema.IsEnum && schema.EnumValues != null)
            {
                foreach (var value in schema.EnumValues)
                {
                    var item = new ListViewItem(value);
                    item.SubItems.Add("enum value");
                    item.SubItems.Add("No");
                    lstProperties.Items.Add(item);
                }
            }
            else
            {
                foreach (var prop in schema.Properties)
                {
                    var item = new ListViewItem(prop.Name);
                    item.SubItems.Add(prop.TypeDisplay);
                    item.SubItems.Add(prop.IsRequired ? "Yes" : "No");
                    lstProperties.Items.Add(item);
                }
            }
        }
        finally
        {
            lstProperties.EndUpdate();
        }
    }

    private void UpdateUI()
    {
        var isLoading = _viewModel?.IsLoadingFile ?? false;

        btnAnalyze.Enabled = !isLoading;
        btnAnalyze.Text = isLoading ? "Analyzing..." : "Analyze File";

        grpFile.Enabled = !isLoading;
        btnImportAll.Enabled = !isLoading && _viewModel?.FileInfo != null && _viewModel.FileInfo.SchemaCount > 0;
        btnImportSelected.Enabled = !isLoading && lstSchemas.SelectedItems.Count > 0;
    }

    private void TxtSearch_TextChanged(object? sender, EventArgs e)
    {
        if (_viewModel != null)
        {
            _viewModel.SearchFilter = txtSearch.Text;
        }
    }

    private void LstSchemas_SelectedIndexChanged(object? sender, EventArgs e)
    {
        btnImportSelected.Enabled = lstSchemas.SelectedItems.Count > 0;

        if (lstSchemas.SelectedItems.Count > 0 && lstSchemas.SelectedItems[0].Tag is OpenApiSchemaInfoViewModel schema)
        {
            RefreshPropertyList(schema);
        }
        else
        {
            lstProperties.Items.Clear();
        }
    }

    private async void LstSchemas_DoubleClick(object? sender, EventArgs e)
    {
        if (lstSchemas.SelectedItems.Count > 0 && lstSchemas.SelectedItems[0].Tag is OpenApiSchemaInfoViewModel schema)
        {
            await ImportSchemaAsync(schema.Name);
        }
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

    private async void BtnImportSelected_Click(object? sender, EventArgs e)
    {
        if (_viewModel == null) return;

        if (lstSchemas.SelectedItems.Count > 0 && lstSchemas.SelectedItems[0].Tag is OpenApiSchemaInfoViewModel schema)
        {
            await ImportSchemaAsync(schema.Name);
        }
    }

    private async void BtnImportAll_Click(object? sender, EventArgs e)
    {
        if (_viewModel == null) return;

        try
        {
            // Import only the filtered schemas if there's a search filter
            var schemaNames = _viewModel.FilteredSchemas?.Select(s => s.Name).ToList();
            await _viewModel.ImportAllSchemasAsync(schemaNames);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error importing schemas: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task ImportSchemaAsync(string schemaName)
    {
        if (_viewModel == null) return;

        try
        {
            await _viewModel.ImportSchemaAsync(schemaName);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error importing schema: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
    {
        BindViewModel((OpenApiDatasourceEditViewModel)(object)viewModel);
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
