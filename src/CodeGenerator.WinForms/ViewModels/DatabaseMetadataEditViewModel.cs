using CodeGenerator.Core.Models.Schema;
using CodeGenerator.Shared.ViewModels;

namespace CodeGenerator.WinForms.ViewModels;

public class DatabaseMetadataEditViewModel : ValidationViewModelBase
{
    private DatabaseMetadata? _metadata;
    private string _databaseName = string.Empty;
    private string _schema = "dbo";
    private string _provider = "SqlServer";
    private string _connectionStringName = string.Empty;
    private bool _useMigrations = true;
    private bool _seedData;

    public DatabaseMetadataEditViewModel()
    {
    }

    public DatabaseMetadataEditViewModel(DatabaseMetadata metadata)
    {
        LoadFromMetadata(metadata);
    }

    public string DatabaseName
    {
        get => _databaseName;
        set => SetProperty(ref _databaseName, value);
    }

    public string Schema
    {
        get => _schema;
        set => SetProperty(ref _schema, value);
    }

    public string Provider
    {
        get => _provider;
        set => SetProperty(ref _provider, value);
    }

    public string ConnectionStringName
    {
        get => _connectionStringName;
        set => SetProperty(ref _connectionStringName, value);
    }

    public bool UseMigrations
    {
        get => _useMigrations;
        set => SetProperty(ref _useMigrations, value);
    }

    public bool SeedData
    {
        get => _seedData;
        set => SetProperty(ref _seedData, value);
    }

    public void LoadFromMetadata(DatabaseMetadata metadata)
    {
        _metadata = metadata;
        DatabaseName = metadata.DatabaseName ?? string.Empty;
        Schema = metadata.Schema ?? "dbo";
        Provider = metadata.Provider ?? "SqlServer";
        ConnectionStringName = metadata.ConnectionStringName ?? string.Empty;
        UseMigrations = metadata.UseMigrations;
        SeedData = metadata.SeedData;
    }

    public void UpdateMetadata()
    {
        if (_metadata != null)
        {
            _metadata.DatabaseName = string.IsNullOrWhiteSpace(DatabaseName) ? null : DatabaseName;
            _metadata.Schema = Schema;
            _metadata.Provider = Provider;
            _metadata.ConnectionStringName = string.IsNullOrWhiteSpace(ConnectionStringName) ? null : ConnectionStringName;
            _metadata.UseMigrations = UseMigrations;
            _metadata.SeedData = SeedData;
        }
    }

    public override bool Validate()
    {
        ClearValidationErrors();
        return IsValid;
    }
}
