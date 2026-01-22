using CodeGenerator.Core.Workspaces.Datasources.PostgreSql.Services;
using CodeGenerator.Core.Workspaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGenerator.Core.Workspaces.Datasources.PostgreSql;

/// <summary>
/// Extension methods for registering PostgreSQL datasource services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register PostgreSQL datasource services
    /// </summary>
    public static IServiceCollection AddPostgreSqlDatasourceServices(
        this IServiceCollection services,
        IConfiguration? configuration = null)
    {
        // Register the datasource provider
        services.AddSingleton<IDatasourceProvider, PostgreSqlDatasourceProvider>();

        // Register the schema reader
        services.AddTransient<PostgreSqlSchemaReader>();

        return services;
    }
}
