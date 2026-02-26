using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Application.Controllers.Workspace.Datasources;
using CodeGenerator.Application.Services;
using CodeGenerator.Application.ViewModels.Template;
using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Application.ViewModels.Workspace.Domains;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.CodeElements;
using CodeGenerator.Core.CodeElements.Services;
using CodeGenerator.Core.LLM;
using CodeGenerator.Core.LLM.Copilot;
using CodeGenerator.Core.LLM.Ollama;
using CodeGenerator.Core.LLM.Services;
using CodeGenerator.Core.MessageBus;
using CodeGenerator.Core.Services;
using CodeGenerator.Core.Workspaces.Datasources.Csv;
using CodeGenerator.Core.Workspaces.Datasources.Csv.ViewModels;
using CodeGenerator.Core.Workspaces.Datasources.Directory.ViewModels;
using CodeGenerator.Core.Workspaces.Datasources.DotNetAssembly.ViewModels;
using CodeGenerator.Core.Workspaces.Datasources.Excel.ViewModels;
using CodeGenerator.Core.Workspaces.Datasources.Json.ViewModels;
using CodeGenerator.Core.Workspaces.Datasources.Mysql.ViewModels;
using CodeGenerator.Core.Workspaces.Datasources.OpenApi.ViewModels;
using CodeGenerator.Core.Workspaces.Datasources.PostgreSql.ViewModels;
using CodeGenerator.Core.Workspaces.Datasources.SqlServer.ViewModels;
using CodeGenerator.Core.Workspaces.Datasources.Xml.ViewModels;
using CodeGenerator.Core.Workspaces.Datasources.Yaml.ViewModels;
using CodeGenerator.Core.Workspaces.ViewModels;
using CodeGenerator.Core.Workspaces.ViewModels.Common;
using CodeGenerator.Core.Workspaces.ViewModels.Datasources;
using CodeGenerator.Core.Workspaces.ViewModels.Workspace;
using CodeGenerator.Generators.DotNet.ApplicationScope;
using CodeGenerator.Generators.DotNet.Repositories.Csv;
using CodeGenerator.Generators.DotNet.WinformsRibbonApplication;
using CodeGenerator.Presentation.WinForms.Resources;
using CodeGenerator.Presentation.WinForms.Services;
using CodeGenerator.Presentation.WinForms.Views;
using CodeGenerator.Presentation.WinForms.Views.Application;
using CodeGenerator.Presentation.WinForms.Views.Domains;
using CodeGenerator.Presentation.WinForms.Views.Workspace;
using CodeGenerator.Core.Workspaces.Views;
using CodeGenerator.Shared.Ribbon;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls;
using CodeGenerator.UserControls.Ribbon;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CodeGenerator.Core.Workspaces.Datasources.Directory;
using CodeGenerator.Core.Workspaces.Datasources.DotNetAssembly;
using CodeGenerator.Core.Workspaces.Datasources.Excel;
using CodeGenerator.Core.Workspaces.Datasources.Json;
using CodeGenerator.Core.Workspaces.Datasources.Mysql;
using CodeGenerator.Core.Workspaces.Datasources.OpenApi;
using CodeGenerator.Core.Workspaces.Datasources.PostgreSql;
using CodeGenerator.Core.Workspaces.Datasources.SqlServer;
using CodeGenerator.Core.Workspaces.Datasources.Xml;
using CodeGenerator.Core.Workspaces.Datasources.Yaml;
namespace CodeGenerator.Presentation.WinForms;

/// <summary>
/// Extension methods for configuring services in the DI container
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all application services in the DI container
    /// </summary>
    public static IServiceCollection AddPresentationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IApplicationService, ApplicationService>();
        services.AddSingleton<IMessageBoxService, MessageBoxService>();
        services.AddSingleton<IFileSystemDialogService, FileSystemDialogService>();
        services.AddSingleton<IRibbonRenderer, SyncfusionRibbonRenderer>();
        services.AddSharedUserControlViews();

        // Register LLM abstraction layer + providers (replaces AddCopilotServices)
        services.AddLlmServices(configuration);
        services.AddCopilotLlmProvider(configuration);
        services.AddOllamaLlmProvider(configuration);

        services.AddCodeElementsServices(configuration);
        
        // Register workspace views
        services.AddWorkspaceViewsServices(configuration);

        // Register Datasources
        services.AddCsvDatasourceServices(configuration);
        services.AddDirectoryDatasourceServices(configuration);
        services.AddDotNetAssemblyDatasourceServices(configuration);
        services.AddExcelDatasourceServices(configuration);
        services.AddJsonDatasourceServices(configuration);
        services.AddMysqlDatasourceServices(configuration);
        services.AddOpenApiDatasourceServices(configuration);
        services.AddPostgreSqlDatasourceServices(configuration);
        services.AddSqlServerDatasourceServices(configuration);
        services.AddXmlDatasourceServices(configuration);
        services.AddYamlDatasourceServices(configuration);

        // Register Generators
        services.AddDotNetWinformsRibbonApplicationGeneratorServices(configuration);
        services.AddDotNetApplicationScopeGeneratorServices(configuration);
        services.AddDotNetDomainLayerGeneratorServices(configuration);
        services.AddDotNetCsvRepositoriesGeneratorServices(configuration);

        // Main form
        services.AddSingleton<MainView>();
        services.AddSingleton<WindowManagerService>((s) => new WindowManagerService(Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<MainView>(s)));
        services.AddSingleton<IWindowManagerService>((s) => Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<WindowManagerService>(s));
        services.AddSingleton<ITemplateWindowManagerService>((s) => Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<WindowManagerService>(s));
        services.AddSingleton<ILlmWindowManagerService>((s) => Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<WindowManagerService>(s));
        services.AddSingleton<IWorkspaceWindowManagerService>((s) => Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<WindowManagerService>(s));
        services.AddSingleton<ICodeElementsWindowManagerService>((s) => Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<WindowManagerService>(s));
        services.AddSingleton<IBrowserWindowManagerService>((s) => Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<WindowManagerService>(s));
        // Resources
        services.AddSingleton<ITreeNodeIconResolver<ResourceManagerTreeNodeIcon>, ResourceManagerTreeNodeIconResolver>();

        // View Factory
        services.AddSingleton<IViewFactory, CodeGeneratorViewFactory>();

        return services;
    }

}
