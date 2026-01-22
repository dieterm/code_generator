using CodeGenerator.Core.Workspaces.Datasources.Mysql.Artifacts;
using CodeGenerator.Core.Workspaces.Datasources.Mysql.Services;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CodeGenerator.Core.Workspaces.Datasources.Mysql.ViewModels
{
    /// <summary>
    /// ViewModel for editing MySQL datasource properties
    /// </summary>
    public class MysqlDatasourceEditViewModel : ViewModelBase
    {
        private readonly MysqlSchemaReader _schemaReader;
        private MysqlDatasourceArtifact? _datasource;
        private bool _isLoading;
        private bool _isConnecting;
        private string? _connectionStatus;
        private string? _errorMessage;

        public MysqlDatasourceEditViewModel()
        {
            _schemaReader = new MysqlSchemaReader();

            // Initialize field view models
            NameField = new SingleLineTextFieldModel { Label = "Datasource Name", Name = "Name" };
            ServerField = new SingleLineTextFieldModel { Label = "Server", Name = "Server" };
            PortField = new IntegerFieldModel { Label = "Port", Name = "Port", Minimum=0, Maximum=int.MaxValue };
            DatabaseField = new SingleLineTextFieldModel { Label = "Database", Name = "Database" };
            UsernameField = new SingleLineTextFieldModel { Label = "Username", Name = "Username" };
            PasswordField = new SingleLineTextFieldModel { Label = "Password", Name = "Password" };

            // Subscribe to field changes
            NameField.PropertyChanged += OnFieldChanged;
            ServerField.PropertyChanged += OnFieldChanged;
            PortField.PropertyChanged += OnFieldChanged;
            DatabaseField.PropertyChanged += OnFieldChanged;
            UsernameField.PropertyChanged += OnFieldChanged;
            PasswordField.PropertyChanged += OnFieldChanged;

            AvailableObjects = new ObservableCollection<DatabaseObjectViewModel>();
        }

        /// <summary>
        /// The datasource being edited
        /// </summary>
        public MysqlDatasourceArtifact? Datasource
        {
            get => _datasource;
            set
            {
                if (_datasource!=null)
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
            if(e.PropertyName == nameof(MysqlDatasourceArtifact.Name) && !_isLoading)
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
        /// Is the view model currently loading data
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
                PortField.Value = _datasource.Port;
                DatabaseField.Value = _datasource.Database;
                UsernameField.Value = _datasource.Username;
                PasswordField.Value = _datasource.Password;

                AvailableObjects.Clear();
                ConnectionStatus = null;
                ErrorMessage = null;
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void OnFieldChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
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

            _datasource.Name = NameField.Value?.ToString() ?? "MySQL Datasource";
            _datasource.Server = ServerField.Value?.ToString() ?? "localhost";
            _datasource.Port = PortField.Value is int port ? port : 3306;
            _datasource.Database = DatabaseField.Value?.ToString() ?? string.Empty;
            _datasource.Username = UsernameField.Value?.ToString() ?? string.Empty;
            _datasource.Password = PasswordField.Value?.ToString() ?? string.Empty;
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
}
