using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Core.Workspaces.Datasources.Xml.Controllers;
using CodeGenerator.Core.Workspaces.Datasources.Xml.Services;
using CodeGenerator.Core.Workspaces.Datasources.Xml.ViewModels;
using CodeGenerator.Core.Workspaces.Datasources.Xml.Views;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGenerator.Core.Workspaces.Datasources.Xml;

/// <summary>
/// Extension methods for registering XML datasource services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register XML datasource services
    /// </summary>
    public static IServiceCollection AddXmlDatasourceServices(
        this IServiceCollection services,
        IConfiguration? configuration = null)
    {
        // Register the datasource provider
        services.AddSingleton<IDatasourceProvider, XmlDatasourceProvider>();
        services.AddSingleton<IWorkspaceArtifactController, XmlDatasourceController>();
        services.AddTransient<IView<XmlDatasourceEditViewModel>, XmlDatasourceEditView>();

        // Register the schema reader
        services.AddTransient<XmlSchemaReader>();

        return services;
    }
}
