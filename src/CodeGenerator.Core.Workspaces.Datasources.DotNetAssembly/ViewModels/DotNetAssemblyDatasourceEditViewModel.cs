using CodeGenerator.Core.Workspaces.Datasources.DotNetAssembly.Artifacts;
using CodeGenerator.Core.Workspaces.Datasources.DotNetAssembly.Services;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Core.Workspaces.Datasources.DotNetAssembly.ViewModels;

/// <summary>
/// ViewModel for editing .NET Assembly datasource properties
/// </summary>
public class DotNetAssemblyDatasourceEditViewModel : ViewModelBase
{
    private readonly AssemblySchemaReader _schemaReader;
    private DotNetAssemblyDatasourceArtifact? _datasource;
    private bool _isLoading;
    private bool _isLoadingFile;
    private string? _statusMessage;
    private string? _errorMessage;
    private AssemblyInfoViewModel? _assemblyInfo;
    private string _searchFilter = string.Empty;
    private List<AssemblyTypeInfoViewModel> _filteredTypes = new();

    public DotNetAssemblyDatasourceEditViewModel()
    {
        _schemaReader = new AssemblySchemaReader();

        // Initialize field view models
        NameField = new SingleLineTextFieldModel { Label = "Datasource Name", Name = "Name" };
        FilePathField = new FileFieldModel
        {
            Label = "Assembly File",
            Name = "FilePath",
            Filter = ".NET Assemblies (*.dll;*.exe)|*.dll;*.exe|DLL Files (*.dll)|*.dll|EXE Files (*.exe)|*.exe|All Files (*.*)|*.*",
            DefaultExtension = ".dll",
            SelectionMode = FileSelectionMode.Open
        };

        // Subscribe to field changes
        NameField.PropertyChanged += OnFieldChanged;
        FilePathField.PropertyChanged += OnFieldChanged;
    }

    /// <summary>
    /// The datasource being edited
    /// </summary>
    public DotNetAssemblyDatasourceArtifact? Datasource
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
        if (e.PropertyName == nameof(DotNetAssemblyDatasourceArtifact.Name) && !_isLoading)
        {
            NameField.Value = _datasource?.Name;
        }
    }

    // Field ViewModels
    public SingleLineTextFieldModel NameField { get; }
    public FileFieldModel FilePathField { get; }

    /// <summary>
    /// Assembly info after loading
    /// </summary>
    public AssemblyInfoViewModel? AssemblyInfo
    {
        get => _assemblyInfo;
        private set
        {
            if (SetProperty(ref _assemblyInfo, value))
            {
                ApplySearchFilter();
            }
        }
    }

    /// <summary>
    /// Filtered types based on search filter
    /// </summary>
    public List<AssemblyTypeInfoViewModel> FilteredTypes
    {
        get => _filteredTypes;
        private set => SetProperty(ref _filteredTypes, value);
    }

    /// <summary>
    /// Search filter for types
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
        if (_assemblyInfo == null)
        {
            FilteredTypes = new List<AssemblyTypeInfoViewModel>();
            return;
        }

        if (string.IsNullOrWhiteSpace(_searchFilter))
        {
            FilteredTypes = _assemblyInfo.Types.ToList();
        }
        else
        {
            var filter = _searchFilter.Trim();
            FilteredTypes = _assemblyInfo.Types
                .Where(t =>
                    t.Name.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                    (t.Namespace?.Contains(filter, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    t.FullName.Contains(filter, StringComparison.OrdinalIgnoreCase))
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
    /// Event raised when a type should be added to the workspace
    /// </summary>
    public event EventHandler<AddTypeEventArgs>? AddTypeRequested;

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

            AssemblyInfo = null;
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

        _datasource.Name = NameField.Value?.ToString() ?? ".NET Assembly Datasource";
        _datasource.FilePath = FilePathField.Value?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Load assembly info from the file
    /// </summary>
    public async Task LoadAssemblyInfoAsync(CancellationToken cancellationToken = default)
    {
        if (_datasource == null) return;

        var filePath = FilePathField.Value?.ToString();
        if (string.IsNullOrEmpty(filePath))
        {
            ErrorMessage = "Please select an assembly file first.";
            return;
        }

        if (!File.Exists(filePath))
        {
            ErrorMessage = "The specified file does not exist.";
            return;
        }

        IsLoadingFile = true;
        ErrorMessage = null;
        StatusMessage = "Analyzing assembly...";
        AssemblyInfo = null;

        try
        {
            var info = await _schemaReader.GetAssemblyInfoAsync(filePath, cancellationToken);
            AssemblyInfo = AssemblyInfoViewModel.FromAssemblyInfo(info);

            StatusMessage = $"Found {info.TypeCount} types in {info.NamespaceCount} namespaces";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error: {ex.Message}";
            StatusMessage = "Error analyzing assembly";
        }
        finally
        {
            IsLoadingFile = false;
        }
    }

    /// <summary>
    /// Import a specific type as a table
    /// </summary>
    public async Task ImportTypeAsync(string typeName, CancellationToken cancellationToken = default)
    {
        if (_datasource == null) return;

        var filePath = FilePathField.Value?.ToString();
        if (string.IsNullOrEmpty(filePath))
        {
            ErrorMessage = "Please select an assembly file first.";
            return;
        }

        if (!File.Exists(filePath))
        {
            ErrorMessage = "The specified file does not exist.";
            return;
        }

        try
        {
            var table = await _schemaReader.ImportTypeAsync(
                filePath,
                typeName,
                _datasource.Name,
                cancellationToken);

            AddTypeRequested?.Invoke(this, new AddTypeEventArgs(table));
            StatusMessage = $"Imported type: {table.Name}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error importing type: {ex.Message}";
        }
    }

    /// <summary>
    /// Import all types (or selected types) as tables
    /// </summary>
    public async Task ImportAllTypesAsync(IEnumerable<string>? typeNames = null, CancellationToken cancellationToken = default)
    {
        if (_datasource == null) return;

        var filePath = FilePathField.Value?.ToString();
        if (string.IsNullOrEmpty(filePath))
        {
            ErrorMessage = "Please select an assembly file first.";
            return;
        }

        try
        {
            var tables = await _schemaReader.ImportTypesAsync(
                filePath,
                _datasource.Name,
                typeNames,
                cancellationToken);

            foreach (var table in tables)
            {
                AddTypeRequested?.Invoke(this, new AddTypeEventArgs(table));
            }

            StatusMessage = $"Imported {tables.Count} types";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error importing types: {ex.Message}";
        }
    }
}
