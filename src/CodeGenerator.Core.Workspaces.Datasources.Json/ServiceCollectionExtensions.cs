using CodeGenerator.Core.Workspaces.Datasources.Json.Services;
using CodeGenerator.Core.Workspaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGenerator.Core.Workspaces.Datasources.Json;

/// <summary>
/// Extension methods for registering JSON datasource services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register JSON datasource services
    /// </summary>
    public static IServiceCollection AddJsonDatasourceServices(
        this IServiceCollection services,
        IConfiguration? configuration = null)
    {
        // Register the datasource provider
        services.AddSingleton<IDatasourceProvider, JsonDatasourceProvider>();

        // Register the schema reader
        services.AddTransient<JsonSchemaReader>();

        return services;
    }
}
