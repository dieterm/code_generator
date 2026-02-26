using CodeGenerator.Core.Workspaces.Datasources.Yaml.Artifacts;
using CodeGenerator.Core.Workspaces.Datasources.Yaml.Services;
using CodeGenerator.Shared;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Core.Workspaces.Datasources.Yaml.ViewModels;

/// <summary>
/// ViewModel for editing YAML datasource properties
/// </summary>
public class YamlDatasourceEditViewModel : ViewModelBase
{
    private readonly YamlSchemaReader _schemaReader;
    private YamlDatasourceArtifact? _datasource;
    private bool _isLoading;
    private CancellationTokenSource? _loadingCts;

    public YamlDatasourceEditViewModel()
    {
        _schemaReader = new YamlSchemaReader();

        // Initialize field view models
        NameField = new SingleLineTextFieldModel { Label = "Datasource Name", Name = "Name" };
        FilePathField = new FileFieldModel 
        { 
            Label = "YAML File", 
            Name = "FilePath",
            Filter = "YAML Files (*.yaml;*.yml)|*.yaml;*.yml|All Files (*.*)|*.*",
            DefaultExtension = ".yaml",
            SelectionMode = FileSelectionMode.Open
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
                new() { HeaderText = "Property Name", Width = 160 },
                new() { HeaderText = "Data Type", Width = 100 },
                new() { HeaderText = "Nullable", Width = 60 }
            },
            LoadCommand = new AsyncRelayCommand(async () => await LoadFileInfoAsync()),
            AddSelectedCommand = new AsyncRelayCommand(async () => await ImportTableAsync())
        };

        // Subscribe to field changes
        NameField.PropertyChanged += OnFieldChanged;
        FilePathField.PropertyChanged += OnFieldChanged;
    }

    /// <summary>
    /// The datasource being edited
    /// </summary>
    public YamlDatasourceArtifact? Datasource
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
        if (e.PropertyName == nameof(YamlDatasourceArtifact.Name) && !_isLoading)
        {
            NameField.Value = _datasource?.Name;
        }
    }

    // Field ViewModels
    public SingleLineTextFieldModel NameField { get; }
    public FileFieldModel FilePathField { get; }
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

        _datasource.Name = NameField.Value?.ToString() ?? "YAML Datasource";
        _datasource.FilePath = FilePathField.Value?.ToString() ?? string.Empty;
    }

    public async Task LoadFileInfoAsync(CancellationToken cancellationToken = default)
    {
        if (_datasource == null) return;

        var filePath = FilePathField.Value?.ToString();
        if (string.IsNullOrEmpty(filePath))
        {
            ObjectImportField.ErrorText = "Please select a YAML file first.";
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
            var info = await _schemaReader.GetFileInfoAsync(filePath, token);
            var fileInfoVm = YamlFileInfoViewModel.FromYamlFileInfo(info);

            if (info.IsSequence)
            {
                ObjectImportField.InfoText = $"Sequence with {info.ItemCount} items, {info.PropertyCount} properties detected";
            }
            else
            {
                ObjectImportField.InfoText = $"Mapping with {info.PropertyCount} properties detected";
            }

            foreach (var prop in fileInfoVm.Properties)
            {
                ObjectImportField.Items.Add(new DatasourceObjectItemViewModel
                {
                    Text = prop.Name,
                    SubItems = new List<string> { prop.TypeDisplay, prop.IsNullable ? "Yes" : "No" },
                    Tag = prop
                });
            }

            ObjectImportField.StatusText = $"Found {fileInfoVm.Properties.Count} properties";
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

    public async Task ImportTableAsync(CancellationToken cancellationToken = default)
    {
        if (_datasource == null) return;

        var filePath = FilePathField.Value?.ToString();
        if (string.IsNullOrEmpty(filePath))
        {
            ObjectImportField.ErrorText = "Please select a YAML file first.";
            return;
        }

        if (!File.Exists(filePath))
        {
            ObjectImportField.ErrorText = "The specified file does not exist.";
            return;
        }

        try
        {
            var table = await _schemaReader.ImportYamlAsync(
                filePath,
                _datasource.Name,
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
