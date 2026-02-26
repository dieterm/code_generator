using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Core.Workspaces.Datasources.Yaml.Controllers;
using CodeGenerator.Core.Workspaces.Datasources.Yaml.Services;
using CodeGenerator.Core.Workspaces.Datasources.Yaml.ViewModels;
using CodeGenerator.Core.Workspaces.Datasources.Yaml.Views;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared.Views;
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
        services.AddSingleton<IWorkspaceArtifactController, YamlDatasourceController>();
        services.AddTransient<IView<YamlDatasourceEditViewModel>, YamlDatasourceEditView>();

        // Register the schema reader
        services.AddTransient<YamlSchemaReader>();

        return services;
    }
}
