using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Core.Workspaces.Datasources.PostgreSql.Controllers;
using CodeGenerator.Core.Workspaces.Datasources.PostgreSql.Services;
using CodeGenerator.Core.Workspaces.Datasources.PostgreSql.ViewModels;
using CodeGenerator.Core.Workspaces.Datasources.PostgreSql.Views;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared.Views;
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
        services.AddSingleton<IWorkspaceArtifactController, PostgreSqlDatasourceController>();
        services.AddTransient<IView<PostgreSqlDatasourceEditViewModel>, PostgreSqlDatasourceEditView>();
        // Register the schema reader
        services.AddTransient<PostgreSqlSchemaReader>();

        return services;
    }
}
