using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Core.Workspaces.Datasources.Directory.Artifacts;
using CodeGenerator.Core.Workspaces.Datasources.Directory.Services;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Core.Workspaces.Datasources.Directory.ViewModels;

/// <summary>
/// ViewModel for editing Directory datasource properties
/// </summary>
public class DirectoryDatasourceEditViewModel : ViewModelBase
{
    private readonly DirectorySchemaReader _schemaReader;
    private DirectoryDatasourceArtifact? _datasource;
    private bool _isLoading;
    private bool _isScanning;
    private string? _statusMessage;
    private string? _errorMessage;
    private DirectorySummary? _directorySummary;

    public DirectoryDatasourceEditViewModel()
    {
        _schemaReader = new DirectorySchemaReader();

        // Initialize field view models
        NameField = new SingleLineTextFieldModel { Label = "Datasource Name", Name = "Name" };
        DirectoryPathField = new FolderFieldModel
        {
            Label = "Directory",
            Name = "DirectoryPath",
            Description = "Select a folder to scan"
        };
        SearchPatternField = new SingleLineTextFieldModel
        {
            Label = "Search Pattern",
            Name = "SearchPattern",
            Tooltip = "Filter pattern for files (e.g., *.cs, *.txt, *.*)"
        };
        IncludeSubdirectoriesField = new CheckboxFieldModel
        {
            Label = "Include Subdirectories",
            Name = "IncludeSubdirectories"
        };

        // Subscribe to field changes
        NameField.PropertyChanged += OnFieldChanged;
        DirectoryPathField.PropertyChanged += OnFieldChanged;
        SearchPatternField.PropertyChanged += OnFieldChanged;
        IncludeSubdirectoriesField.PropertyChanged += OnFieldChanged;
    }

    /// <summary>
    /// The datasource being edited
    /// </summary>
    public DirectoryDatasourceArtifact? Datasource
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
        if (e.PropertyName == nameof(DirectoryDatasourceArtifact.Name) && !_isLoading)
        {
            NameField.Value = _datasource?.Name;
        }
    }

    // Field ViewModels
    public SingleLineTextFieldModel NameField { get; }
    public FolderFieldModel DirectoryPathField { get; }
    public SingleLineTextFieldModel SearchPatternField { get; }
    public CheckboxFieldModel IncludeSubdirectoriesField { get; }

    /// <summary>
    /// Directory summary after scanning
    /// </summary>
    public DirectorySummary? DirectorySummary
    {
        get => _directorySummary;
        private set => SetProperty(ref _directorySummary, value);
    }

    /// <summary>
    /// Is the view model currently scanning directory
    /// </summary>
    public bool IsScanning
    {
        get => _isScanning;
        private set => SetProperty(ref _isScanning, value);
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
            DirectoryPathField.Value = _datasource.DirectoryPath;
            SearchPatternField.Value = _datasource.SearchPattern;
            IncludeSubdirectoriesField.Value = _datasource.IncludeSubdirectories;

            DirectorySummary = null;
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

        _datasource.Name = NameField.Value?.ToString() ?? "Directory Datasource";
        _datasource.DirectoryPath = DirectoryPathField.Value?.ToString() ?? string.Empty;
        _datasource.SearchPattern = SearchPatternField.Value?.ToString() ?? "*.*";
        _datasource.IncludeSubdirectories = IncludeSubdirectoriesField.Value is bool b && b;
    }

    /// <summary>
    /// Scan the directory and get summary information
    /// </summary>
    public async Task ScanDirectoryAsync(CancellationToken cancellationToken = default)
    {
        if (_datasource == null) return;

        var directoryPath = DirectoryPathField.Value?.ToString();
        if (string.IsNullOrEmpty(directoryPath))
        {
            ErrorMessage = "Please select a directory first.";
            return;
        }

        if (!System.IO.Directory.Exists(directoryPath))
        {
            ErrorMessage = "The specified directory does not exist.";
            return;
        }

        IsScanning = true;
        ErrorMessage = null;
        StatusMessage = "Scanning directory...";
        DirectorySummary = null;

        try
        {
            await Task.Run(() =>
            {
                var searchPattern = SearchPatternField.Value?.ToString() ?? "*.*";
                DirectorySummary = _schemaReader.GetDirectorySummary(directoryPath, searchPattern);
            }, cancellationToken);

            if (DirectorySummary != null)
            {
                if (!string.IsNullOrEmpty(DirectorySummary.AccessError))
                {
                    StatusMessage = $"Scanned with warnings: {DirectorySummary.AccessError}";
                }
                else
                {
                    StatusMessage = $"Found {DirectorySummary.TotalFileCount} files in {DirectorySummary.TotalDirectoryCount + 1} directories ({DirectorySummary.TotalSizeFormatted})";
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error: {ex.Message}";
            StatusMessage = "Error scanning directory";
        }
        finally
        {
            IsScanning = false;
        }
    }

    /// <summary>
    /// Import the directory structure as a table
    /// </summary>
    public async Task ImportTableAsync(CancellationToken cancellationToken = default)
    {
        if (_datasource == null) return;

        var directoryPath = DirectoryPathField.Value?.ToString();
        if (string.IsNullOrEmpty(directoryPath))
        {
            ErrorMessage = "Please select a directory first.";
            return;
        }

        if (!System.IO.Directory.Exists(directoryPath))
        {
            ErrorMessage = "The specified directory does not exist.";
            return;
        }

        try
        {
            var searchPattern = SearchPatternField.Value?.ToString() ?? "*.*";
            var table = await _schemaReader.ImportDirectoryAsync(
                directoryPath,
                _datasource.Name,
                searchPattern,
                cancellationToken);

            AddTableRequested?.Invoke(this, new AddTableEventArgs(table));
            StatusMessage = $"Imported table: {table.Name}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error importing directory: {ex.Message}";
        }
    }
}

/// <summary>
/// Event args for property value changes
/// </summary>
public class PropertyValueChangedEventArgs : EventArgs
{
    public string PropertyName { get; }
    public object? Value { get; }

    public PropertyValueChangedEventArgs(string propertyName, object? value)
    {
        PropertyName = propertyName;
        Value = value;
    }
}

/// <summary>
/// Event args for adding a table
/// </summary>
public class AddTableEventArgs : EventArgs
{
    public TableArtifact Table { get; }

    public AddTableEventArgs(TableArtifact table)
    {
        Table = table;
    }
}
