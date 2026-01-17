using CodeGenerator.Core.Workspaces.Datasources.Json.Artifacts;
using CodeGenerator.Core.Workspaces.Datasources.Json.Services;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Core.Workspaces.Datasources.Json.ViewModels;

/// <summary>
/// ViewModel for editing JSON datasource properties
/// </summary>
public class JsonDatasourceEditViewModel : ViewModelBase
{
    private readonly JsonSchemaReader _schemaReader;
    private JsonDatasourceArtifact? _datasource;
    private bool _isLoading;
    private bool _isLoadingFile;
    private string? _statusMessage;
    private string? _errorMessage;
    private JsonFileInfoViewModel? _fileInfo;

    public JsonDatasourceEditViewModel()
    {
        _schemaReader = new JsonSchemaReader();

        // Initialize field view models
        NameField = new SingleLineTextFieldModel { Label = "Datasource Name", Name = "Name" };
        FilePathField = new FileFieldModel 
        { 
            Label = "JSON File", 
            Name = "FilePath",
            Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
            DefaultExtension = ".json",
            SelectionMode = FileSelectionMode.Open
        };

        // Subscribe to field changes
        NameField.PropertyChanged += OnFieldChanged;
        FilePathField.PropertyChanged += OnFieldChanged;
    }

    /// <summary>
    /// The datasource being edited
    /// </summary>
    public JsonDatasourceArtifact? Datasource
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
        if (e.PropertyName == nameof(JsonDatasourceArtifact.Name) && !_isLoading)
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
    public JsonFileInfoViewModel? FileInfo
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

        _datasource.Name = NameField.Value?.ToString() ?? "JSON Datasource";
        _datasource.FilePath = FilePathField.Value?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Load file info from the JSON file
    /// </summary>
    public async Task LoadFileInfoAsync(CancellationToken cancellationToken = default)
    {
        if (_datasource == null) return;

        var filePath = FilePathField.Value?.ToString();
        if (string.IsNullOrEmpty(filePath))
        {
            ErrorMessage = "Please select a JSON file first.";
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
            var info = await _schemaReader.GetFileInfoAsync(filePath, cancellationToken);
            FileInfo = JsonFileInfoViewModel.FromJsonFileInfo(info);
            
            if (info.IsArray)
            {
                StatusMessage = $"Array with {info.ItemCount} items, {info.PropertyCount} properties detected";
            }
            else
            {
                StatusMessage = $"Object with {info.PropertyCount} properties detected";
            }
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
    /// Import the JSON file as a table
    /// </summary>
    public async Task ImportTableAsync(CancellationToken cancellationToken = default)
    {
        if (_datasource == null) return;

        var filePath = FilePathField.Value?.ToString();
        if (string.IsNullOrEmpty(filePath))
        {
            ErrorMessage = "Please select a JSON file first.";
            return;
        }

        if (!File.Exists(filePath))
        {
            ErrorMessage = "The specified file does not exist.";
            return;
        }

        try
        {
            var table = await _schemaReader.ImportJsonAsync(
                filePath,
                _datasource.Name,
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
