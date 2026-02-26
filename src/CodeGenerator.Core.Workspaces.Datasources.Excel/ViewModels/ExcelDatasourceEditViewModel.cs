using CodeGenerator.Core.Workspaces.Datasources.Excel.Artifacts;
using CodeGenerator.Core.Workspaces.Datasources.Excel.Services;
using CodeGenerator.Shared;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CodeGenerator.Core.Workspaces.Datasources.Excel.ViewModels;

/// <summary>
/// ViewModel for editing Excel datasource properties
/// </summary>
public class ExcelDatasourceEditViewModel : ViewModelBase
{
    private readonly ExcelSchemaReader _schemaReader;
    private ExcelDatasourceArtifact? _datasource;
    private bool _isLoading;
    private CancellationTokenSource? _loadingCts;

    public ExcelDatasourceEditViewModel()
    {
        _schemaReader = new ExcelSchemaReader();

        // Initialize field view models
        NameField = new SingleLineTextFieldModel { Label = "Datasource Name", Name = "Name" };
        FilePathField = new FileFieldModel 
        { 
            Label = "Excel File", 
            Name = "FilePath",
            Filter = "Excel Files (*.xlsx;*.xls)|*.xlsx;*.xls|All Files (*.*)|*.*",
            DefaultExtension = ".xlsx",
            SelectionMode = FileSelectionMode.Open
        };
        FirstRowIsHeaderField = new BooleanFieldModel { Label = "First Row is Header", Name = "FirstRowIsHeader" };

        ObjectImportField = new DatasourceObjectImportFieldModel
        {
            GroupBoxText = "Available Sheets",
            LoadButtonText = "Load Sheets",
            AddSelectedButtonText = "Add Selected",
            AddAllButtonText = "Add All",
            AddAllButtonVisible = true,
            Columns = new List<DatasourceObjectColumnDefinition>
            {
                new() { HeaderText = "Sheet Name", Width = 180 },
                new() { HeaderText = "Columns", Width = 80 },
                new() { HeaderText = "Rows", Width = 80 }
            },
            LoadCommand = new AsyncRelayCommand(async () => await LoadSheetsAsync()),
            AddSelectedCommand = new AsyncRelayCommand(async () => await AddSelectedSheetAsync()),
            AddAllCommand = new AsyncRelayCommand(async () => await AddAllSheetsAsync())
        };

        // Subscribe to field changes
        NameField.PropertyChanged += OnFieldChanged;
        FilePathField.PropertyChanged += OnFieldChanged;
        FirstRowIsHeaderField.PropertyChanged += OnFieldChanged;
    }

    /// <summary>
    /// The datasource being edited
    /// </summary>
    public ExcelDatasourceArtifact? Datasource
    {
        get => _datasource;
        set
        {
            if (_datasource != null)
            {
                _datasource.PropertyChanged -= OnDatasourcePropertyChanged;
            }
            if (SetProperty(ref _datasource, value))
            {
                if (_datasource != null)
                {
                    _datasource.PropertyChanged += OnDatasourcePropertyChanged;
                }
                LoadFromDatasource();
            }
        }
    }

    private void OnDatasourcePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ExcelDatasourceArtifact.Name) && !_isLoading)
        {
            NameField.Value = _datasource?.Name;
        }
    }

    // Field ViewModels
    public SingleLineTextFieldModel NameField { get; }
    public FileFieldModel FilePathField { get; }
    public BooleanFieldModel FirstRowIsHeaderField { get; }
    public DatasourceObjectImportFieldModel ObjectImportField { get; }

    /// <summary>
    /// Event raised when a sheet should be added to the workspace
    /// </summary>
    public event EventHandler<AddSheetEventArgs>? AddSheetRequested;

    /// <summary>
    /// Event raised when a property value changes
    /// </summary>
    public event EventHandler<PropertyValueChangedEventArgs>? ValueChanged;

    private void LoadFromDatasource()
    {
        if (_datasource == null) return;

        _isLoading = true;
        try
        {
            NameField.Value = _datasource.Name;
            FilePathField.Value = _datasource.FilePath;
            FirstRowIsHeaderField.Value = _datasource.FirstRowIsHeader;

            ObjectImportField.Items.Clear();
            ObjectImportField.StatusText = null;
            ObjectImportField.ErrorText = null;
        }
        finally
        {
            _isLoading = false;
        }
    }

    private void OnFieldChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_isLoading || _datasource == null) return;

        if (e.PropertyName == nameof(FieldViewModelBase.Value) && sender is FieldViewModelBase field)
        {
            SaveToDatasource();
            ValueChanged?.Invoke(this, new PropertyValueChangedEventArgs(field.Name, field.Value));
        }
    }

    private void SaveToDatasource()
    {
        if (_datasource == null) return;

        _datasource.Name = NameField.Value?.ToString() ?? "Excel Datasource";
        _datasource.FilePath = FilePathField.Value?.ToString() ?? string.Empty;
        _datasource.FirstRowIsHeader = FirstRowIsHeaderField.Value is bool firstRowIsHeader && firstRowIsHeader;
    }

    public async Task LoadSheetsAsync(CancellationToken cancellationToken = default)
    {
        if (_datasource == null) return;

        var filePath = FilePathField.Value?.ToString();
        if (string.IsNullOrEmpty(filePath))
        {
            ObjectImportField.ErrorText = "Please select an Excel file first.";
            return;
        }

        if (!File.Exists(filePath))
        {
            ObjectImportField.ErrorText = "The specified file does not exist.";
            return;
        }

        _loadingCts?.Cancel();
        _loadingCts = new CancellationTokenSource();
        var token = CancellationTokenSource.CreateLinkedTokenSource(_loadingCts.Token, cancellationToken).Token;

        ObjectImportField.IsLoading = true;
        ObjectImportField.ErrorText = null;
        ObjectImportField.StatusText = "Loading sheets...";
        ObjectImportField.Items.Clear();

        try
        {
            var sheets = await _schemaReader.GetSheetsAsync(filePath, token);

            foreach (var sheet in sheets)
            {
                ObjectImportField.Items.Add(new DatasourceObjectItemViewModel
                {
                    Text = sheet.Name,
                    SubItems = new List<string> { sheet.ColumnCount.ToString(), sheet.RowCount.ToString() },
                    ImageKey = "table",
                    Tag = sheet
                });
            }

            ObjectImportField.StatusText = $"Found {sheets.Count} sheets";
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            ObjectImportField.ErrorText = $"Error: {ex.Message}";
            ObjectImportField.StatusText = "Error loading sheets";
        }
        finally
        {
            ObjectImportField.IsLoading = false;
        }
    }

    public async Task AddSelectedSheetAsync(CancellationToken cancellationToken = default)
    {
        if (_datasource == null || ObjectImportField.SelectedItem == null) return;

        var sheetInfo = ObjectImportField.SelectedItem.Tag as SheetInfo;
        var sheetName = sheetInfo?.Name ?? ObjectImportField.SelectedItem.Text;

        var filePath = FilePathField.Value?.ToString();
        if (string.IsNullOrEmpty(filePath))
        {
            ObjectImportField.ErrorText = "Please select an Excel file first.";
            return;
        }

        try
        {
            var table = await _schemaReader.ImportSheetAsync(
                filePath,
                sheetName,
                _datasource.Name,
                _datasource.FirstRowIsHeader,
                cancellationToken);

            AddSheetRequested?.Invoke(this, new AddSheetEventArgs(table));
        }
        catch (Exception ex)
        {
            ObjectImportField.ErrorText = $"Error importing sheet: {ex.Message}";
        }
    }

    public async Task AddAllSheetsAsync(CancellationToken cancellationToken = default)
    {
        if (_datasource == null) return;

        var filePath = FilePathField.Value?.ToString();
        if (string.IsNullOrEmpty(filePath))
        {
            ObjectImportField.ErrorText = "Please select an Excel file first.";
            return;
        }

        try
        {
            foreach (var item in ObjectImportField.Items)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var sheetInfo = item.Tag as SheetInfo;
                var sheetName = sheetInfo?.Name ?? item.Text;

                var table = await _schemaReader.ImportSheetAsync(
                    filePath,
                    sheetName,
                    _datasource.Name,
                    _datasource.FirstRowIsHeader,
                    cancellationToken);

                AddSheetRequested?.Invoke(this, new AddSheetEventArgs(table));
            }

            ObjectImportField.StatusText = $"Added {ObjectImportField.Items.Count} sheets";
        }
        catch (Exception ex)
        {
            ObjectImportField.ErrorText = $"Error importing sheets: {ex.Message}";
        }
    }
}
