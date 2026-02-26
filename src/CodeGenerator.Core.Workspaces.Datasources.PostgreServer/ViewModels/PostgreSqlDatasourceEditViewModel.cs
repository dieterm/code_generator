using CodeGenerator.Core.Workspaces.Datasources.PostgreSql.Artifacts;
using CodeGenerator.Core.Workspaces.Datasources.PostgreSql.Services;
using CodeGenerator.Shared;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CodeGenerator.Core.Workspaces.Datasources.PostgreSql.ViewModels;

/// <summary>
/// ViewModel for editing PostgreSQL datasource properties
/// </summary>
public class PostgreSqlDatasourceEditViewModel : ViewModelBase
{
    private readonly PostgreSqlSchemaReader _schemaReader;
    private PostgreSqlDatasourceArtifact? _datasource;
    private bool _isLoading;
    private CancellationTokenSource? _loadingCts;

    public PostgreSqlDatasourceEditViewModel()
    {
        _schemaReader = ServiceProviderHolder.GetRequiredService<PostgreSqlSchemaReader>();

        // Initialize field view models
        NameField = new SingleLineTextFieldModel { Label = "Datasource Name", Name = "Name" };
        ServerField = new SingleLineTextFieldModel { Label = "Server", Name = "Server" };
        PortField = new IntegerFieldModel { Label = "Port", Name = "Port", Minimum=0, Maximum = int.MaxValue };
        DatabaseField = new SingleLineTextFieldModel { Label = "Database", Name = "Database" };
        UsernameField = new SingleLineTextFieldModel { Label = "Username", Name = "Username" };
        PasswordField = new SingleLineTextFieldModel { Label = "Password", Name = "Password" };
        SslModeField = new ComboboxFieldModel 
        { 
            Label = "SSL Mode", 
            Name = "SslMode",
            Items = new List<ComboboxItem>
            {
                new() { DisplayName = "Disable", Value = "Disable" },
                new() { DisplayName = "Allow", Value = "Allow" },
                new() { DisplayName = "Prefer", Value = "Prefer" },
                new() { DisplayName = "Require", Value = "Require" },
                new() { DisplayName = "Verify CA", Value = "VerifyCA" },
                new() { DisplayName = "Verify Full", Value = "VerifyFull" }
            }
        };

        ObjectImportField = new DatasourceObjectImportFieldModel
        {
            GroupBoxText = "Available Tables and Views",
            LoadButtonText = "Load Tables/Views",
            AddSelectedButtonText = "Add Selected",
            AddAllButtonVisible = false,
            Columns = new List<DatasourceObjectColumnDefinition>
            {
                new() { HeaderText = "Name", Width = 180 },
                new() { HeaderText = "Schema", Width = 100 },
                new() { HeaderText = "Type", Width = 80 }
            },
            LoadCommand = new AsyncRelayCommand(async () => await LoadDatabaseObjectsAsync()),
            AddSelectedCommand = new AsyncRelayCommand(async () => await AddSelectedObjectAsync())
        };

        // Subscribe to field changes
        NameField.PropertyChanged += OnFieldChanged;
        ServerField.PropertyChanged += OnFieldChanged;
        PortField.PropertyChanged += OnFieldChanged;
        DatabaseField.PropertyChanged += OnFieldChanged;
        UsernameField.PropertyChanged += OnFieldChanged;
        PasswordField.PropertyChanged += OnFieldChanged;
        SslModeField.PropertyChanged += OnFieldChanged;
    }

    /// <summary>
    /// The datasource being edited
    /// </summary>
    public PostgreSqlDatasourceArtifact? Datasource
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
        if (e.PropertyName == nameof(PostgreSqlDatasourceArtifact.Name) && !_isLoading)
        {
            NameField.Value = _datasource?.Name;
        }
    }

    // Field ViewModels
    public SingleLineTextFieldModel NameField { get; }
    public SingleLineTextFieldModel ServerField { get; }
    public IntegerFieldModel PortField { get; }
    public SingleLineTextFieldModel DatabaseField { get; }
    public SingleLineTextFieldModel UsernameField { get; }
    public SingleLineTextFieldModel PasswordField { get; }
    public ComboboxFieldModel SslModeField { get; }
    public DatasourceObjectImportFieldModel ObjectImportField { get; }

    /// <summary>
    /// Event raised when a table/view should be added to the workspace
    /// </summary>
    public event EventHandler<AddDatabaseObjectEventArgs>? AddObjectRequested;

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
            ServerField.Value = _datasource.Server;
            PortField.Value = _datasource.Port;
            DatabaseField.Value = _datasource.Database;
            UsernameField.Value = _datasource.Username;
            PasswordField.Value = _datasource.Password;
            SslModeField.Value = _datasource.SslMode;

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

        _datasource.Name = NameField.Value?.ToString() ?? "PostgreSQL Datasource";
        _datasource.Server = ServerField.Value?.ToString() ?? "localhost";
        _datasource.Port = PortField.Value is int port ? port : 5432;
        _datasource.Database = DatabaseField.Value?.ToString() ?? string.Empty;
        _datasource.Username = UsernameField.Value?.ToString() ?? "postgres";
        _datasource.Password = PasswordField.Value?.ToString() ?? string.Empty;
        _datasource.SslMode = SslModeField.Value?.ToString() ?? "Prefer";
    }

    /// <summary>
    /// Test connection and load available tables/views
    /// </summary>
    public async Task LoadDatabaseObjectsAsync(CancellationToken cancellationToken = default)
    {
        if (_datasource == null) return;

        _loadingCts?.Cancel();
        _loadingCts = new CancellationTokenSource();
        var token = CancellationTokenSource.CreateLinkedTokenSource(_loadingCts.Token, cancellationToken).Token;

        ObjectImportField.IsLoading = true;
        ObjectImportField.ErrorText = null;
        ObjectImportField.StatusText = "Connecting...";
        ObjectImportField.Items.Clear();

        try
        {
            var isValid = await _datasource.ValidateAsync(token);
            if (!isValid)
            {
                ObjectImportField.ErrorText = "Could not connect to the database. Please check your connection settings.";
                ObjectImportField.StatusText = "Connection failed";
                return;
            }

            ObjectImportField.StatusText = "Loading tables and views...";

            var objects = await _schemaReader.GetTablesAndViewsAsync(
                _datasource.ConnectionString,
                token);

            foreach (var obj in objects)
            {
                ObjectImportField.Items.Add(new DatasourceObjectItemViewModel
                {
                    Text = obj.Name,
                    SubItems = new List<string> { obj.Schema, obj.ObjectType.ToString() },
                    ImageKey = obj.ObjectType == DatabaseObjectType.Table ? "table" : "eye",
                    Tag = obj
                });
            }

            ObjectImportField.StatusText = $"Found {objects.Count} tables and views";
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            ObjectImportField.ErrorText = $"Error: {ex.Message}";
            ObjectImportField.StatusText = "Error loading schema";
        }
        finally
        {
            ObjectImportField.IsLoading = false;
        }
    }

    /// <summary>
    /// Add the selected object to the workspace
    /// </summary>
    public async Task AddSelectedObjectAsync(CancellationToken cancellationToken = default)
    {
        if (_datasource == null || ObjectImportField.SelectedItem == null) return;

        var dbObject = ObjectImportField.SelectedItem.Tag as DatabaseObjectInfo;
        if (dbObject == null) return;

        try
        {
            if (dbObject.ObjectType == DatabaseObjectType.Table)
            {
                var table = await _schemaReader.ImportTableAsync(
                    _datasource.ConnectionString,
                    dbObject.Name,
                    dbObject.Schema,
                    _datasource.Name,
                    cancellationToken);

                AddObjectRequested?.Invoke(this, new AddDatabaseObjectEventArgs(table));
            }
            else
            {
                var view = await _schemaReader.ImportViewAsync(
                    _datasource.ConnectionString,
                    dbObject.Name,
                    dbObject.Schema,
                    _datasource.Name,
                    cancellationToken);

                AddObjectRequested?.Invoke(this, new AddDatabaseObjectEventArgs(view));
            }
        }
        catch (Exception ex)
        {
            ObjectImportField.ErrorText = $"Error importing object: {ex.Message}";
        }
    }
}
