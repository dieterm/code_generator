using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Core.Workspaces.Datasources.Directory.Artifacts;
using CodeGenerator.Core.Workspaces.Datasources.Directory.Services;
using CodeGenerator.Shared;
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
    private CancellationTokenSource? _loadingCts;

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

        ObjectImportField = new DatasourceObjectImportFieldModel
        {
            GroupBoxText = "Directory Summary",
            LoadButtonText = "Scan Directory",
            AddSelectedButtonText = "Import Table",
            AddAllButtonVisible = false,
            InfoLabelVisible = true,
            Columns = new List<DatasourceObjectColumnDefinition>
            {
                new() { HeaderText = "Property", Width = 140 },
                new() { HeaderText = "Value", Width = 200 }
            },
            LoadCommand = new AsyncRelayCommand(async () => await ScanDirectoryAsync()),
            AddSelectedCommand = new AsyncRelayCommand(async () => await ImportTableAsync())
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
            DirectoryPathField.Value = _datasource.DirectoryPath;
            SearchPatternField.Value = _datasource.SearchPattern;
            IncludeSubdirectoriesField.Value = _datasource.IncludeSubdirectories;

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

        _datasource.Name = NameField.Value?.ToString() ?? "Directory Datasource";
        _datasource.DirectoryPath = DirectoryPathField.Value?.ToString() ?? string.Empty;
        _datasource.SearchPattern = SearchPatternField.Value?.ToString() ?? "*.*";
        _datasource.IncludeSubdirectories = IncludeSubdirectoriesField.Value is bool b && b;
    }

    public async Task ScanDirectoryAsync(CancellationToken cancellationToken = default)
    {
        if (_datasource == null) return;

        var directoryPath = DirectoryPathField.Value?.ToString();
        if (string.IsNullOrEmpty(directoryPath))
        {
            ObjectImportField.ErrorText = "Please select a directory first.";
            return;
        }

        if (!System.IO.Directory.Exists(directoryPath))
        {
            ObjectImportField.ErrorText = "The specified directory does not exist.";
            return;
        }

        _loadingCts?.Cancel();
        _loadingCts = new CancellationTokenSource();
        var token = CancellationTokenSource.CreateLinkedTokenSource(_loadingCts.Token, cancellationToken).Token;

        ObjectImportField.IsLoading = true;
        ObjectImportField.ErrorText = null;
        ObjectImportField.StatusText = "Scanning directory...";
        ObjectImportField.Items.Clear();

        try
        {
            DirectorySummary? summary = null;
            await Task.Run(() =>
            {
                var searchPattern = SearchPatternField.Value?.ToString() ?? "*.*";
                summary = _schemaReader.GetDirectorySummary(directoryPath, searchPattern);
            }, token);

            if (summary != null)
            {
                ObjectImportField.InfoText = !string.IsNullOrEmpty(summary.AccessError)
                    ? $"Scanned with warnings: {summary.AccessError}"
                    : null;

                ObjectImportField.Items.Add(new DatasourceObjectItemViewModel { Text = "Total Files", SubItems = new List<string> { summary.TotalFileCount.ToString() } });
                ObjectImportField.Items.Add(new DatasourceObjectItemViewModel { Text = "Total Directories", SubItems = new List<string> { (summary.TotalDirectoryCount + 1).ToString() } });
                ObjectImportField.Items.Add(new DatasourceObjectItemViewModel { Text = "Total Size", SubItems = new List<string> { summary.TotalSizeFormatted } });

                ObjectImportField.StatusText = $"Found {summary.TotalFileCount} files in {summary.TotalDirectoryCount + 1} directories ({summary.TotalSizeFormatted})";
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            ObjectImportField.ErrorText = $"Error: {ex.Message}";
            ObjectImportField.StatusText = "Error scanning directory";
        }
        finally
        {
            ObjectImportField.IsLoading = false;
        }
    }

    public async Task ImportTableAsync(CancellationToken cancellationToken = default)
    {
        if (_datasource == null) return;

        var directoryPath = DirectoryPathField.Value?.ToString();
        if (string.IsNullOrEmpty(directoryPath))
        {
            ObjectImportField.ErrorText = "Please select a directory first.";
            return;
        }

        if (!System.IO.Directory.Exists(directoryPath))
        {
            ObjectImportField.ErrorText = "The specified directory does not exist.";
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
            ObjectImportField.StatusText = $"Imported table: {table.Name}";
        }
        catch (Exception ex)
        {
            ObjectImportField.ErrorText = $"Error importing directory: {ex.Message}";
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
