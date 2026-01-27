using CodeGenerator.Core.Workspaces.Datasources.OpenApi.Artifacts;
using CodeGenerator.Core.Workspaces.Datasources.OpenApi.Services;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Core.Workspaces.Datasources.OpenApi.ViewModels;

/// <summary>
/// ViewModel for editing OpenAPI datasource properties
/// </summary>
public class OpenApiDatasourceEditViewModel : ViewModelBase
{
    private readonly OpenApiSchemaReader _schemaReader;
    private OpenApiDatasourceArtifact? _datasource;
    private bool _isLoading;
    private bool _isLoadingFile;
    private string? _statusMessage;
    private string? _errorMessage;
    private OpenApiFileInfoViewModel? _fileInfo;
    private string _searchFilter = string.Empty;
    private List<OpenApiSchemaInfoViewModel> _filteredSchemas = new();

    public OpenApiDatasourceEditViewModel()
    {
        _schemaReader = new OpenApiSchemaReader();

        // Initialize field view models
        NameField = new SingleLineTextFieldModel { Label = "Datasource Name", Name = "Name" };
        FilePathField = new FileFieldModel
        {
            Label = "OpenAPI File",
            Name = "FilePath",
            Filter = "OpenAPI Files (*.yaml;*.yml;*.json)|*.yaml;*.yml;*.json|YAML Files (*.yaml;*.yml)|*.yaml;*.yml|JSON Files (*.json)|*.json|All Files (*.*)|*.*",
            DefaultExtension = ".yaml",
            SelectionMode = FileSelectionMode.Open
        };

        // Subscribe to field changes
        NameField.PropertyChanged += OnFieldChanged;
        FilePathField.PropertyChanged += OnFieldChanged;
    }

    /// <summary>
    /// The datasource being edited
    /// </summary>
    public OpenApiDatasourceArtifact? Datasource
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
        if (e.PropertyName == nameof(OpenApiDatasourceArtifact.Name) && !_isLoading)
        {
            NameField.Value = _datasource?.Name;
        }
    }

    // Field ViewModels
    public SingleLineTextFieldModel NameField { get; }
    public FileFieldModel FilePathField { get; }

    /// <summary>
    /// File info after loading
    /// </summary>
    public OpenApiFileInfoViewModel? FileInfo
    {
        get => _fileInfo;
        private set
        {
            if (SetProperty(ref _fileInfo, value))
            {
                ApplySearchFilter();
            }
        }
    }

    /// <summary>
    /// Filtered schemas based on search filter
    /// </summary>
    public List<OpenApiSchemaInfoViewModel> FilteredSchemas
    {
        get => _filteredSchemas;
        private set => SetProperty(ref _filteredSchemas, value);
    }

    /// <summary>
    /// Search filter for schemas
    /// </summary>
    public string SearchFilter
    {
        get => _searchFilter;
        set
        {
            if (SetProperty(ref _searchFilter, value))
            {
                ApplySearchFilter();
            }
        }
    }

    private void ApplySearchFilter()
    {
        if (_fileInfo == null)
        {
            FilteredSchemas = new List<OpenApiSchemaInfoViewModel>();
            return;
        }

        if (string.IsNullOrWhiteSpace(_searchFilter))
        {
            FilteredSchemas = _fileInfo.Schemas.ToList();
        }
        else
        {
            var filter = _searchFilter.Trim();
            FilteredSchemas = _fileInfo.Schemas
                .Where(s =>
                    s.Name.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                    (s.Description?.Contains(filter, StringComparison.OrdinalIgnoreCase) ?? false))
                .ToList();
        }
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
    /// Event raised when a schema should be added to the workspace
    /// </summary>
    public event EventHandler<AddSchemaEventArgs>? AddSchemaRequested;

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

            FileInfo = null;
            StatusMessage = null;
            ErrorMessage = null;
            SearchFilter = string.Empty;
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

        _datasource.Name = NameField.Value?.ToString() ?? "OpenAPI Datasource";
        _datasource.FilePath = FilePathField.Value?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Load file info from the OpenAPI file
    /// </summary>
    public async Task LoadFileInfoAsync(CancellationToken cancellationToken = default)
    {
        if (_datasource == null) return;

        var filePath = FilePathField.Value?.ToString();
        if (string.IsNullOrEmpty(filePath))
        {
            ErrorMessage = "Please select an OpenAPI file first.";
            return;
        }

        if (!File.Exists(filePath))
        {
            ErrorMessage = "The specified file does not exist.";
            return;
        }

        IsLoadingFile = true;
        ErrorMessage = null;
        StatusMessage = "Analyzing OpenAPI specification...";
        FileInfo = null;

        try
        {
            var info = await _schemaReader.GetFileInfoAsync(filePath, cancellationToken);
            FileInfo = OpenApiFileInfoViewModel.FromFileInfo(info);

            StatusMessage = $"Found {info.SchemaCount} schemas";
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
    /// Import a specific schema as a table
    /// </summary>
    public async Task ImportSchemaAsync(string schemaName, CancellationToken cancellationToken = default)
    {
        if (_datasource == null) return;

        var filePath = FilePathField.Value?.ToString();
        if (string.IsNullOrEmpty(filePath))
        {
            ErrorMessage = "Please select an OpenAPI file first.";
            return;
        }

        if (!File.Exists(filePath))
        {
            ErrorMessage = "The specified file does not exist.";
            return;
        }

        try
        {
            var table = await _schemaReader.ImportSchemaAsync(
                filePath,
                schemaName,
                _datasource.Name,
                cancellationToken);

            AddSchemaRequested?.Invoke(this, new AddSchemaEventArgs(table));
            StatusMessage = $"Imported schema: {table.Name}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error importing schema: {ex.Message}";
        }
    }

    /// <summary>
    /// Import all schemas (or filtered schemas) as tables
    /// </summary>
    public async Task ImportAllSchemasAsync(IEnumerable<string>? schemaNames = null, CancellationToken cancellationToken = default)
    {
        if (_datasource == null) return;

        var filePath = FilePathField.Value?.ToString();
        if (string.IsNullOrEmpty(filePath))
        {
            ErrorMessage = "Please select an OpenAPI file first.";
            return;
        }

        try
        {
            var tables = await _schemaReader.ImportSchemasAsync(
                filePath,
                _datasource.Name,
                schemaNames,
                cancellationToken);

            foreach (var table in tables)
            {
                AddSchemaRequested?.Invoke(this, new AddSchemaEventArgs(table));
            }

            StatusMessage = $"Imported {tables.Count} schemas";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error importing schemas: {ex.Message}";
        }
    }
}
