using CodeGenerator.Core.Workspaces.Datasources.Csv.Artifacts;
using CodeGenerator.Core.Workspaces.Datasources.Csv.Services;
using CodeGenerator.Shared;
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
    private CancellationTokenSource? _loadingCts;

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

        ObjectImportField = new DatasourceObjectImportFieldModel
        {
            GroupBoxText = "File Structure",
            LoadButtonText = "Analyze File",
            AddSelectedButtonText = "Import Table",
            AddAllButtonVisible = false,
            InfoLabelVisible = true,
            Columns = new List<DatasourceObjectColumnDefinition>
            {
                new() { HeaderText = "Column Name", Width = 200 },
                new() { HeaderText = "Inferred Type", Width = 120 }
            },
            LoadCommand = new AsyncRelayCommand(async () => await LoadFileInfoAsync()),
            AddSelectedCommand = new AsyncRelayCommand(async () => await ImportTableAsync())
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
    public DatasourceObjectImportFieldModel ObjectImportField { get; }

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

            ObjectImportField.Items.Clear();
            ObjectImportField.InfoText = null;
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
            ObjectImportField.ErrorText = "Please select a CSV file first.";
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
        ObjectImportField.StatusText = "Analyzing file...";
        ObjectImportField.Items.Clear();

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
                token);

            ObjectImportField.InfoText = $"Table: {info.TableName} ({info.ColumnCount} columns, {info.RowCount} rows)";

            foreach (var columnName in info.ColumnNames)
            {
                ObjectImportField.Items.Add(new DatasourceObjectItemViewModel
                {
                    Text = columnName,
                    SubItems = new List<string> { "(analyze to infer)" }
                });
            }

            ObjectImportField.StatusText = $"Found {info.ColumnCount} columns";
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            ObjectImportField.ErrorText = $"Error: {ex.Message}";
            ObjectImportField.StatusText = "Error analyzing file";
        }
        finally
        {
            ObjectImportField.IsLoading = false;
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
            ObjectImportField.ErrorText = "Please select a CSV file first.";
            return;
        }

        if (!File.Exists(filePath))
        {
            ObjectImportField.ErrorText = "The specified file does not exist.";
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
            ObjectImportField.StatusText = $"Imported table: {table.Name}";
        }
        catch (Exception ex)
        {
            ObjectImportField.ErrorText = $"Error importing file: {ex.Message}";
        }
    }
}
