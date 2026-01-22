using CodeGenerator.Core.Workspaces.Datasources.SqlServer.Services;
using CodeGenerator.Core.Workspaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGenerator.Core.Workspaces.Datasources.SqlServer;

/// <summary>
/// Extension methods for registering SQL Server datasource services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register SQL Server datasource services
    /// </summary>
    public static IServiceCollection AddSqlServerDatasourceServices(
        this IServiceCollection services,
        IConfiguration? configuration = null)
    {
        // Register the datasource provider
        services.AddSingleton<IDatasourceProvider, SqlServerDatasourceProvider>();

        // Register the schema reader
        services.AddTransient<SqlServerSchemaReader>();

        return services;
    }
}
