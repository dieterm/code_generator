using CodeGenerator.Core.Workspaces.Datasources.Excel.Artifacts;
using CodeGenerator.Core.Workspaces.Datasources.Excel.Services;
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
    private bool _isLoadingSheets;
    private string? _statusMessage;
    private string? _errorMessage;

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

        // Subscribe to field changes
        NameField.PropertyChanged += OnFieldChanged;
        FilePathField.PropertyChanged += OnFieldChanged;
        FirstRowIsHeaderField.PropertyChanged += OnFieldChanged;

        AvailableSheets = new ObservableCollection<SheetViewModel>();
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

    /// <summary>
    /// Available sheets from the Excel file
    /// </summary>
    public ObservableCollection<SheetViewModel> AvailableSheets { get; }

    /// <summary>
    /// Currently selected sheet in the list
    /// </summary>
    private SheetViewModel? _selectedSheet;
    public SheetViewModel? SelectedSheet
    {
        get => _selectedSheet;
        set => SetProperty(ref _selectedSheet, value);
    }

    /// <summary>
    /// Is the view model currently loading sheets
    /// </summary>
    public bool IsLoadingSheets
    {
        get => _isLoadingSheets;
        private set => SetProperty(ref _isLoadingSheets, value);
    }

    /// <summary>
    /// Status message
    /// </summary>
    public string? StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    /// <summary>
    /// Error message (if any)
    /// </summary>
    public string? ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

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

            AvailableSheets.Clear();
            StatusMessage = null;
            ErrorMessage = null;
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

    /// <summary>
    /// Load sheets from the Excel file
    /// </summary>
    public async Task LoadSheetsAsync(CancellationToken cancellationToken = default)
    {
        if (_datasource == null) return;

        var filePath = FilePathField.Value?.ToString();
        if (string.IsNullOrEmpty(filePath))
        {
            ErrorMessage = "Please select an Excel file first.";
            return;
        }

        if (!File.Exists(filePath))
        {
            ErrorMessage = "The specified file does not exist.";
            return;
        }

        IsLoadingSheets = true;
        ErrorMessage = null;
        StatusMessage = "Loading sheets...";
        AvailableSheets.Clear();

        try
        {
            var sheets = await _schemaReader.GetSheetsAsync(filePath, cancellationToken);

            foreach (var sheet in sheets)
            {
                AvailableSheets.Add(SheetViewModel.FromSheetInfo(sheet));
            }

            StatusMessage = $"Found {sheets.Count} sheets";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error: {ex.Message}";
            StatusMessage = "Error loading sheets";
        }
        finally
        {
            IsLoadingSheets = false;
        }
    }

    /// <summary>
    /// Add the selected sheet to the workspace as a table
    /// </summary>
    public async Task AddSelectedSheetAsync(CancellationToken cancellationToken = default)
    {
        if (_datasource == null || SelectedSheet == null) return;

        var filePath = FilePathField.Value?.ToString();
        if (string.IsNullOrEmpty(filePath))
        {
            ErrorMessage = "Please select an Excel file first.";
            return;
        }

        try
        {
            var table = await _schemaReader.ImportSheetAsync(
                filePath,
                SelectedSheet.Name,
                _datasource.Name,
                _datasource.FirstRowIsHeader,
                cancellationToken);

            AddSheetRequested?.Invoke(this, new AddSheetEventArgs(table));
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error importing sheet: {ex.Message}";
        }
    }

    /// <summary>
    /// Add all sheets to the workspace as tables
    /// </summary>
    public async Task AddAllSheetsAsync(CancellationToken cancellationToken = default)
    {
        if (_datasource == null) return;

        var filePath = FilePathField.Value?.ToString();
        if (string.IsNullOrEmpty(filePath))
        {
            ErrorMessage = "Please select an Excel file first.";
            return;
        }

        try
        {
            foreach (var sheet in AvailableSheets)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var table = await _schemaReader.ImportSheetAsync(
                    filePath,
                    sheet.Name,
                    _datasource.Name,
                    _datasource.FirstRowIsHeader,
                    cancellationToken);

                AddSheetRequested?.Invoke(this, new AddSheetEventArgs(table));
            }

            StatusMessage = $"Added {AvailableSheets.Count} sheets";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error importing sheets: {ex.Message}";
        }
    }
}
