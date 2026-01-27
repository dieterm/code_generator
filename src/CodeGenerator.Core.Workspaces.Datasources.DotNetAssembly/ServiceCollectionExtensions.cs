using CodeGenerator.Core.Workspaces.Datasources.DotNetAssembly.Services;
using CodeGenerator.Core.Workspaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGenerator.Core.Workspaces.Datasources.DotNetAssembly;

/// <summary>
/// Extension methods for registering .NET Assembly datasource services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register .NET Assembly datasource services
    /// </summary>
    public static IServiceCollection AddDotNetAssemblyDatasourceServices(
        this IServiceCollection services,
        IConfiguration? configuration = null)
    {
        // Register the datasource provider
        services.AddSingleton<IDatasourceProvider, DotNetAssemblyDatasourceProvider>();

        // Register the schema reader
        services.AddTransient<AssemblySchemaReader>();

        return services;
    }
}
