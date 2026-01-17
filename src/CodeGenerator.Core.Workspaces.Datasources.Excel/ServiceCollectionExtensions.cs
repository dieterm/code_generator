using CodeGenerator.Core.Workspaces.Datasources.Excel.Services;
using CodeGenerator.Core.Workspaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGenerator.Core.Workspaces.Datasources.Excel;

/// <summary>
/// Extension methods for registering Excel datasource services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register Excel datasource services
    /// </summary>
    public static IServiceCollection AddExcelDatasourceServices(
        this IServiceCollection services,
        IConfiguration? configuration = null)
    {
        // Register the datasource provider
        services.AddSingleton<IDatasourceProvider, ExcelDatasourceProvider>();

        // Register the schema reader
        services.AddTransient<ExcelSchemaReader>();

        return services;
    }
}
