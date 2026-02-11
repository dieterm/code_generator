using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Application.Controllers.Workspace.Datasources;
using CodeGenerator.Application.Services;
using CodeGenerator.Application.ViewModels.Template;
using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Application.ViewModels.Workspace.Datasources;
using CodeGenerator.Application.ViewModels.Workspace.Domains;
using CodeGenerator.Application.ViewModels.Workspace.Domains.Factories;
using CodeGenerator.Application.ViewModels.Workspace.Domains.Specifications;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Copilot;
using CodeGenerator.Core.CodeElements;
using CodeGenerator.Core.MessageBus;
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
using CodeGenerator.Presentation.WinForms.Resources;
using CodeGenerator.Presentation.WinForms.Services;
using CodeGenerator.Presentation.WinForms.Views;
using CodeGenerator.Presentation.WinForms.Views.Application;
using CodeGenerator.Presentation.WinForms.Views.Domains;
using CodeGenerator.Presentation.WinForms.Views.Workspace.Domains.Factories;
using CodeGenerator.Presentation.WinForms.Views.Workspace.Domains.Specifications;
using CodeGenerator.Shared.Ribbon;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls;
using CodeGenerator.UserControls.Ribbon;
using CodeGenerator.Generators.DotNet.WinformsRibbonApplication;
using CodeGenerator.Generators.DotNet.ApplicationScope;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CodeGenerator.Core.Copilot.Services;
using CodeGenerator.Core.CodeElements.Services;

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
        services.AddCopilotServices(configuration);
        services.AddCodeElementsServices(configuration);
        // Register Datasources
        services.AddCsvDatasourceServices(configuration);

        // Register Generators
        services.AddDotNetWinformsRibbonApplicationGeneratorServices(configuration);
        services.AddDotNetApplicationScopeGeneratorServices(configuration);
        // Main form
        services.AddSingleton<MainView>();
        services.AddSingleton<WindowManagerService>((s) => new WindowManagerService(Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<MainView>(s)));
        services.AddSingleton<IWindowManagerService>((s) => Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<WindowManagerService>(s));
        services.AddSingleton<ITemplateWindowManagerService>((s) => Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<WindowManagerService>(s));
        services.AddSingleton<ICopilotWindowManagerService>((s) => Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<WindowManagerService>(s));
        services.AddSingleton<IWorkspaceWindowManagerService>((s) => Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<WindowManagerService>(s));
        services.AddSingleton<ICodeElementsWindowManagerService>((s) => Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<WindowManagerService>(s));
        // Resources
        services.AddSingleton<ITreeNodeIconResolver<ResourceManagerTreeNodeIcon>, ResourceManagerTreeNodeIconResolver>();

        // View Factory
        services.AddSingleton<IViewFactory, ViewFactory>();

        // ViewModel ? View registrations (used by IViewFactory / WorkspaceArtifactDetailsView)
        services.AddViewMappings();

        return services;
    }

    /// <summary>
    /// Registers all IView&lt;TViewModel&gt; mappings so the ViewFactory can resolve
    /// the correct View for any ViewModel. Add new mappings here when creating new View/ViewModel pairs.
    /// </summary>
    private static IServiceCollection AddViewMappings(this IServiceCollection services)
    {
        // Workspace
        services.AddTransient<IView<WorkspaceEditViewModel>, WorkspaceEditView>();

        // Datasources - Relational
        services.AddTransient<IView<MysqlDatasourceEditViewModel>, MysqlDatasourceEditView>();
        services.AddTransient<IView<SqlServerDatasourceEditViewModel>, SqlServerDatasourceEditView>();
        services.AddTransient<IView<PostgreSqlDatasourceEditViewModel>, PostgreSqlDatasourceEditView>();
        services.AddTransient<IView<ExcelDatasourceEditViewModel>, ExcelDatasourceEditView>();

        // Datasources - File-based
        services.AddTransient<IView<CsvDatasourceEditViewModel>, CodeGenerator.Core.Workspaces.Datasources.Csv.Views.CsvDatasourceEditView>();
        services.AddTransient<IView<JsonDatasourceEditViewModel>, JsonDatasourceEditView>();
        services.AddTransient<IView<XmlDatasourceEditViewModel>, XmlDatasourceEditView>();
        services.AddTransient<IView<YamlDatasourceEditViewModel>, YamlDatasourceEditView>();
        services.AddTransient<IView<DotNetAssemblyDatasourceEditViewModel>, DotNetAssemblyDatasourceEditView>();
        services.AddTransient<IView<OpenApiDatasourceEditViewModel>, OpenApiDatasourceEditView>();
        services.AddTransient<IView<DirectoryDatasourceEditViewModel>, DirectoryDatasourceEditView>();
        services.AddTransient<IView<TemplateParametersViewModel>, TemplateParametersView>();

        // Datasources - Relational schema objects
        services.AddTransient<IView<ColumnEditViewModel>, ColumnEditView>();
        services.AddTransient<IView<IndexEditViewModel>, IndexEditView>();
        services.AddTransient<IView<ForeignKeyEditViewModel>, ForeignKeyEditView>();
        services.AddTransient<IView<TableEditViewModel>, TableEditView>();

        // Scopes & Domains
        services.AddTransient<IView<ScopeEditViewModel>, ScopeEditView>();
        services.AddTransient<IView<DomainEditViewModel>, DomainEditView>();

        // Entities
        services.AddTransient<IView<EntityEditViewModel>, EntityEditView>();
        services.AddTransient<IView<EntityStateEditViewModel>, EntityStateEditView>();
        services.AddTransient<IView<PropertyEditViewModel>, PropertyEditView>();
        services.AddTransient<IView<EntityRelationEditViewModel>, EntityRelationEditView>();

        // Value Types
        services.AddTransient<IView<ValueTypeEditViewModel>, ValueTypeEditView>();

        // Entity Views
        services.AddTransient<IView<EntityEditViewEditViewModel>, EntityEditViewEditView>();
        services.AddTransient<IView<EntityEditViewFieldEditViewModel>, EntityEditViewFieldEditView>();
        services.AddTransient<IView<EntityListViewEditViewModel>, EntityListViewEditView>();
        services.AddTransient<IView<EntityListViewColumnEditViewModel>, EntityListViewColumnEditView>();
        services.AddTransient<IView<EntitySelectViewEditViewModel>, EntitySelectViewEditView>();

        // Domain Specifications & Factories
        services.AddTransient<IView<DomainSpecificationEditViewModel>, DomainSpecificationEditView>();
        services.AddTransient<IView<DomainFactoryEditViewModel>, DomainFactoryEditView>();

        return services;
    }
}
