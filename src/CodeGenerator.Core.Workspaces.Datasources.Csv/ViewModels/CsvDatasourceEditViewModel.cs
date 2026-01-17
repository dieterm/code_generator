using CodeGenerator.Core.Workspaces.Datasources.Csv.Artifacts;
using CodeGenerator.Core.Workspaces.Datasources.Csv.Services;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Core.Workspaces.Datasources.Csv.ViewModels;

/// <summary>
/// ViewModel for editing CSV datasource properties
/// </summary>
public class CsvDatasourceEditViewModel : ViewModelBase
{
    private readonly CsvSchemaReader _schemaReader;
    private CsvDatasourceArtifact? _datasource;
    private bool _isLoading;
    private bool _isLoadingFile;
    private string? _statusMessage;
    private string? _errorMessage;
    private CsvFileInfoViewModel? _fileInfo;

    public CsvDatasourceEditViewModel()
    {
        _schemaReader = new CsvSchemaReader();

        // Initialize field view models
        NameField = new SingleLineTextFieldModel { Label = "Datasource Name", Name = "Name" };
        FilePathField = new FileFieldModel 
        { 
            Label = "CSV File", 
            Name = "FilePath",
            Filter = "CSV Files (*.csv)|*.csv|Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
            DefaultExtension = ".csv",
            SelectionMode = FileSelectionMode.Open
        };
        FirstRowIsHeaderField = new BooleanFieldModel { Label = "First Row is Header", Name = "FirstRowIsHeader" };
        FieldDelimiterField = new SingleLineTextFieldModel 
        { 
            Label = "Field Delimiter", 
            Name = "FieldDelimiter",
            Tooltip = "Character(s) that separate fields (e.g., \",\" or \"|\" or \"\\t\" for tab)"
        };
        RowTerminatorField = new SingleLineTextFieldModel 
        { 
            Label = "Row Terminator", 
            Name = "RowTerminator",
            Tooltip = "Character(s) that terminate rows (e.g., \"\\n\" or \"\\r\\n\")"
        };

        // Subscribe to field changes
        NameField.PropertyChanged += OnFieldChanged;
        FilePathField.PropertyChanged += OnFieldChanged;
        FirstRowIsHeaderField.PropertyChanged += OnFieldChanged;
        FieldDelimiterField.PropertyChanged += OnFieldChanged;
        RowTerminatorField.PropertyChanged += OnFieldChanged;
    }

    /// <summary>
    /// The datasource being edited
    /// </summary>
    public CsvDatasourceArtifact? Datasource
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
        if (e.PropertyName == nameof(CsvDatasourceArtifact.Name) && !_isLoading)
        {
            NameField.Value = _datasource?.Name;
        }
    }

    // Field ViewModels
    public SingleLineTextFieldModel NameField { get; }
    public FileFieldModel FilePathField { get; }
    public BooleanFieldModel FirstRowIsHeaderField { get; }
    public SingleLineTextFieldModel FieldDelimiterField { get; }
    public SingleLineTextFieldModel RowTerminatorField { get; }

    /// <summary>
    /// File info after loading
    /// </summary>
    public CsvFileInfoViewModel? FileInfo
    {
        get => _fileInfo;
        private set => SetProperty(ref _fileInfo, value);
    }

    /// <summary>
    /// Is the view model currently loading file info
    /// </summary>
    public bool IsLoadingFile
    {
        get => _isLoadingFile;
        private set => SetProperty(ref _isLoadingFile, value);
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
    /// Event raised when a table should be added to the workspace
    /// </summary>
    public event EventHandler<AddTableEventArgs>? AddTableRequested;

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
            FieldDelimiterField.Value = _datasource.FieldDelimiter;
            RowTerminatorField.Value = _datasource.RowTerminator;

            FileInfo = null;
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

        _datasource.Name = NameField.Value?.ToString() ?? "CSV Datasource";
        _datasource.FilePath = FilePathField.Value?.ToString() ?? string.Empty;
        _datasource.FirstRowIsHeader = FirstRowIsHeaderField.Value is bool firstRowIsHeader && firstRowIsHeader;
        _datasource.FieldDelimiter = FieldDelimiterField.Value?.ToString() ?? ",";
        _datasource.RowTerminator = RowTerminatorField.Value?.ToString() ?? "\\n";
    }

    /// <summary>
    /// Load file info from the CSV file
    /// </summary>
    public async Task LoadFileInfoAsync(CancellationToken cancellationToken = default)
    {
        if (_datasource == null) return;

        var filePath = FilePathField.Value?.ToString();
        if (string.IsNullOrEmpty(filePath))
        {
            ErrorMessage = "Please select a CSV file first.";
            return;
        }

        if (!File.Exists(filePath))
        {
            ErrorMessage = "The specified file does not exist.";
            return;
        }

        IsLoadingFile = true;
        ErrorMessage = null;
        StatusMessage = "Analyzing file...";
        FileInfo = null;

        try
        {
            var fieldDelimiter = FieldDelimiterField.Value?.ToString() ?? ",";
            var rowTerminator = RowTerminatorField.Value?.ToString() ?? "\\n";
            var firstRowIsHeader = FirstRowIsHeaderField.Value is bool header && header;

            var info = await _schemaReader.GetFileInfoAsync(
                filePath, 
                fieldDelimiter, 
                rowTerminator, 
                firstRowIsHeader, 
                cancellationToken);

            FileInfo = CsvFileInfoViewModel.FromCsvFileInfo(info);
            StatusMessage = $"Table: {info.TableName} ({info.ColumnCount} columns, {info.RowCount} rows)";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error: {ex.Message}";
            StatusMessage = "Error analyzing file";
        }
        finally
        {
            IsLoadingFile = false;
        }
    }

    /// <summary>
    /// Import the CSV file as a table
    /// </summary>
    public async Task ImportTableAsync(CancellationToken cancellationToken = default)
    {
        if (_datasource == null) return;

        var filePath = FilePathField.Value?.ToString();
        if (string.IsNullOrEmpty(filePath))
        {
            ErrorMessage = "Please select a CSV file first.";
            return;
        }

        if (!File.Exists(filePath))
        {
            ErrorMessage = "The specified file does not exist.";
            return;
        }

        try
        {
            var fieldDelimiter = FieldDelimiterField.Value?.ToString() ?? ",";
            var rowTerminator = RowTerminatorField.Value?.ToString() ?? "\\n";
            var firstRowIsHeader = FirstRowIsHeaderField.Value is bool header && header;

            var table = await _schemaReader.ImportCsvAsync(
                filePath,
                _datasource.Name,
                fieldDelimiter,
                rowTerminator,
                firstRowIsHeader,
                cancellationToken);

            AddTableRequested?.Invoke(this, new AddTableEventArgs(table));
            StatusMessage = $"Imported table: {table.Name}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error importing file: {ex.Message}";
        }
    }
}
