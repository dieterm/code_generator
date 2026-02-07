using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Application.Controllers.Workspace.Datasources;
using CodeGenerator.Core.Workspaces.Datasources.Csv.Services;
using CodeGenerator.Core.Workspaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGenerator.Core.Workspaces.Datasources.Csv;

/// <summary>
/// Extension methods for registering CSV datasource services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register CSV datasource services
    /// </summary>
    public static IServiceCollection AddCsvDatasourceServices(
        this IServiceCollection services,
        IConfiguration? configuration = null)
    {
        // Register the datasource provider
        services.AddSingleton<IDatasourceProvider, CsvDatasourceProvider>();
        services.AddSingleton<IWorkspaceArtifactController, CsvDatasourceController>();
        // Register the schema reader
        services.AddTransient<CsvSchemaReader>();

        return services;
    }
}
