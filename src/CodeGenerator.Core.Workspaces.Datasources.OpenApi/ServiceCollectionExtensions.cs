using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Application.Controllers.Workspace.Datasources;
using CodeGenerator.Core.Workspaces.Datasources.OpenApi.Services;
using CodeGenerator.Core.Workspaces.Datasources.OpenApi.ViewModels;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Presentation.WinForms.Views;
using CodeGenerator.Shared.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGenerator.Core.Workspaces.Datasources.OpenApi;

/// <summary>
/// Extension methods for registering OpenAPI datasource services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register OpenAPI datasource services
    /// </summary>
    public static IServiceCollection AddOpenApiDatasourceServices(
        this IServiceCollection services,
        IConfiguration? configuration = null)
    {
        // Register the datasource provider
        services.AddSingleton<IDatasourceProvider, OpenApiDatasourceProvider>();
        services.AddSingleton<IWorkspaceArtifactController, OpenApiDatasourceController>();
        services.AddTransient<IView<OpenApiDatasourceEditViewModel>, OpenApiDatasourceEditView>();

        // Register the schema reader
        services.AddTransient<OpenApiSchemaReader>();

        return services;
    }
}
