using CodeGenerator.Core.Workspaces.Datasources.Directory.Services;
using CodeGenerator.Core.Workspaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGenerator.Core.Workspaces.Datasources.Directory;

/// <summary>
/// Extension methods for registering Directory datasource services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register Directory datasource services
    /// </summary>
    public static IServiceCollection AddDirectoryDatasourceServices(
        this IServiceCollection services,
        IConfiguration? configuration = null)
    {
        // Register the datasource provider
        services.AddSingleton<IDatasourceProvider, DirectoryDatasourceProvider>();

        // Register the schema reader
        services.AddTransient<DirectorySchemaReader>();

        return services;
    }
}
