using CodeGenerator.Core.Workspaces.Datasources.SqlServer.Artifacts;
using CodeGenerator.Core.Workspaces.Datasources.SqlServer.Services;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CodeGenerator.Core.Workspaces.Datasources.SqlServer.ViewModels;

/// <summary>
/// ViewModel for editing SQL Server datasource properties
/// </summary>
public class SqlServerDatasourceEditViewModel : ViewModelBase
{
    private readonly SqlServerSchemaReader _schemaReader;
    private SqlServerDatasourceArtifact? _datasource;
    private bool _isLoading;
    private bool _isConnecting;
    private string? _connectionStatus;
    private string? _errorMessage;

    public SqlServerDatasourceEditViewModel()
    {
        _schemaReader = new SqlServerSchemaReader();

        // Initialize field view models
        NameField = new SingleLineTextFieldModel { Label = "Datasource Name", Name = "Name" };
        ServerField = new SingleLineTextFieldModel { Label = "Server", Name = "Server" };
        DatabaseField = new SingleLineTextFieldModel { Label = "Database", Name = "Database" };
        IntegratedSecurityField = new BooleanFieldModel { Label = "Windows Authentication", Name = "IntegratedSecurity" };
        UsernameField = new SingleLineTextFieldModel { Label = "Username", Name = "Username" };
        PasswordField = new SingleLineTextFieldModel { Label = "Password", Name = "Password" };
        TrustServerCertificateField = new BooleanFieldModel { Label = "Trust Server Certificate", Name = "TrustServerCertificate" };

        // Subscribe to field changes
        NameField.PropertyChanged += OnFieldChanged;
        ServerField.PropertyChanged += OnFieldChanged;
        DatabaseField.PropertyChanged += OnFieldChanged;
        IntegratedSecurityField.PropertyChanged += OnFieldChanged;
        UsernameField.PropertyChanged += OnFieldChanged;
        PasswordField.PropertyChanged += OnFieldChanged;
        TrustServerCertificateField.PropertyChanged += OnFieldChanged;

        AvailableObjects = new ObservableCollection<DatabaseObjectViewModel>();
    }

    /// <summary>
    /// The datasource being edited
    /// </summary>
    public SqlServerDatasourceArtifact? Datasource
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
        if (e.PropertyName == nameof(SqlServerDatasourceArtifact.Name) && !_isLoading)
        {
            NameField.Value = _datasource?.Name;
        }
    }

    // Field ViewModels
    public SingleLineTextFieldModel NameField { get; }
    public SingleLineTextFieldModel ServerField { get; }
    public SingleLineTextFieldModel DatabaseField { get; }
    public BooleanFieldModel IntegratedSecurityField { get; }
    public SingleLineTextFieldModel UsernameField { get; }
    public SingleLineTextFieldModel PasswordField { get; }
    public BooleanFieldModel TrustServerCertificateField { get; }

    /// <summary>
    /// Available tables and views from the database
    /// </summary>
    public ObservableCollection<DatabaseObjectViewModel> AvailableObjects { get; }

    /// <summary>
    /// Currently selected object in the list
    /// </summary>
    private DatabaseObjectViewModel? _selectedObject;
    public DatabaseObjectViewModel? SelectedObject
    {
        get => _selectedObject;
        set => SetProperty(ref _selectedObject, value);
    }

    /// <summary>
    /// Is the view model currently connecting
    /// </summary>
    public bool IsConnecting
    {
        get => _isConnecting;
        private set => SetProperty(ref _isConnecting, value);
    }

    /// <summary>
    /// Connection status message
    /// </summary>
    public string? ConnectionStatus
    {
        get => _connectionStatus;
        private set => SetProperty(ref _connectionStatus, value);
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
            DatabaseField.Value = _datasource.Database;
            IntegratedSecurityField.Value = _datasource.IntegratedSecurity;
            UsernameField.Value = _datasource.Username;
            PasswordField.Value = _datasource.Password;
            TrustServerCertificateField.Value = _datasource.TrustServerCertificate;

            AvailableObjects.Clear();
            ConnectionStatus = null;
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

        _datasource.Name = NameField.Value?.ToString() ?? "SQL Server Datasource";
        _datasource.Server = ServerField.Value?.ToString() ?? "localhost";
        _datasource.Database = DatabaseField.Value?.ToString() ?? string.Empty;
        _datasource.IntegratedSecurity = IntegratedSecurityField.Value is bool integrated && integrated;
        _datasource.Username = UsernameField.Value?.ToString() ?? string.Empty;
        _datasource.Password = PasswordField.Value?.ToString() ?? string.Empty;
        _datasource.TrustServerCertificate = TrustServerCertificateField.Value is bool trustCert && trustCert;
    }

    /// <summary>
    /// Test connection and load available tables/views
    /// </summary>
    public async Task LoadDatabaseObjectsAsync(CancellationToken cancellationToken = default)
    {
        if (_datasource == null) return;

        IsConnecting = true;
        ErrorMessage = null;
        ConnectionStatus = "Connecting...";
        AvailableObjects.Clear();

        try
        {
            // First test connection
            var isValid = await _datasource.ValidateAsync(cancellationToken);
            if (!isValid)
            {
                ErrorMessage = "Could not connect to the database. Please check your connection settings.";
                ConnectionStatus = "Connection failed";
                return;
            }

            ConnectionStatus = "Loading tables and views...";

            // Load tables and views
            var objects = await _schemaReader.GetTablesAndViewsAsync(
                _datasource.ConnectionString,
                cancellationToken);

            foreach (var obj in objects)
            {
                AvailableObjects.Add(new DatabaseObjectViewModel
                {
                    Name = obj.Name,
                    Schema = obj.Schema,
                    ObjectType = obj.ObjectType,
                    DisplayName = obj.DisplayName
                });
            }

            ConnectionStatus = $"Found {objects.Count} tables and views";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error: {ex.Message}";
            ConnectionStatus = "Error loading schema";
        }
        finally
        {
            IsConnecting = false;
        }
    }

    /// <summary>
    /// Add the selected object to the workspace
    /// </summary>
    public async Task AddSelectedObjectAsync(CancellationToken cancellationToken = default)
    {
        if (_datasource == null || SelectedObject == null) return;

        try
        {
            if (SelectedObject.ObjectType == DatabaseObjectType.Table)
            {
                var table = await _schemaReader.ImportTableAsync(
                    _datasource.ConnectionString,
                    SelectedObject.Name,
                    SelectedObject.Schema,
                    _datasource.Name,
                    cancellationToken);

                AddObjectRequested?.Invoke(this, new AddDatabaseObjectEventArgs(table));
            }
            else
            {
                var view = await _schemaReader.ImportViewAsync(
                    _datasource.ConnectionString,
                    SelectedObject.Name,
                    SelectedObject.Schema,
                    _datasource.Name,
                    cancellationToken);

                AddObjectRequested?.Invoke(this, new AddDatabaseObjectEventArgs(view));
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error importing object: {ex.Message}";
        }
    }
}
