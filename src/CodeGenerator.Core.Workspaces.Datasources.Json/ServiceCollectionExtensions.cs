using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Core.Workspaces.Datasources.Json.Controllers;
using CodeGenerator.Core.Workspaces.Datasources.Json.Services;
using CodeGenerator.Core.Workspaces.Datasources.Json.ViewModels;
using CodeGenerator.Core.Workspaces.Datasources.Json.Views;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared.Views;
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
        services.AddSingleton<IWorkspaceArtifactController, JsonDatasourceController>();
        services.AddTransient<IView<JsonDatasourceEditViewModel>, JsonDatasourceEditView>();
        // Register the schema reader
        services.AddTransient<JsonSchemaReader>();

        return services;
    }
}
