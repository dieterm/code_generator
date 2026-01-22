using CodeGenerator.Core.Workspaces.Datasources.Yaml.Services;
using CodeGenerator.Core.Workspaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGenerator.Core.Workspaces.Datasources.Yaml;

/// <summary>
/// Extension methods for registering YAML datasource services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register YAML datasource services
    /// </summary>
    public static IServiceCollection AddYamlDatasourceServices(
        this IServiceCollection services,
        IConfiguration? configuration = null)
    {
        // Register the datasource provider
        services.AddSingleton<IDatasourceProvider, YamlDatasourceProvider>();

        // Register the schema reader
        services.AddTransient<YamlSchemaReader>();

        return services;
    }
}
