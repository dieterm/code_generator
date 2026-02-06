using CodeGenerator.Application.Controllers;
using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Controllers.Template;
using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Application.Controllers.Workspace.Datasources;
using CodeGenerator.Application.Controllers.Workspace.Domains;
using CodeGenerator.Application.Controllers.Workspace.Scopes;
using CodeGenerator.Application.Services;
using CodeGenerator.Application.ViewModels;
using CodeGenerator.Application.ViewModels.Generation;
using CodeGenerator.Application.ViewModels.Template;
using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Interfaces;
using CodeGenerator.Core.MessageBus;
using CodeGenerator.Core.Settings.Application;
using CodeGenerator.Core.Settings.Generators;
using CodeGenerator.Core.Settings.ViewModels;
using CodeGenerator.Core.Templates;
using CodeGenerator.Core.Templates.Settings;
using CodeGenerator.Core.Workspaces;
using CodeGenerator.Core.Workspaces.Datasources.Csv;
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
using CodeGenerator.Core.Workspaces.MessageBus;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Core.Workspaces.Settings;
using CodeGenerator.Domain;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.ProgrammingLanguages.CSharp;
using CodeGenerator.Generators.CodeArchitectureLayers;
using CodeGenerator.Presentation.WinForms.ViewModels;
using CodeGenerator.Shared.Ribbon;
using CodeGenerator.TemplateEngines.DotNetProject;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
namespace CodeGenerator.Application;

/// <summary>
/// Extension methods for configuring services in the DI container
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all application services in the DI container
    /// </summary>
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register Domain services
        services.AddDomainServices(configuration);
        services.AddDomainDotNetServices(configuration);
        CSharpCodeGeneratorExtensions.RegisterCSharpCodeGenerator();
        // Register ViewModels
        services.AddTransient<MainViewModel>();
        
        services.AddTransient<GenerationResultTreeViewModel>();
        services.AddTransient<ArtifactPreviewViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<TemplateTreeViewModel>();
        services.AddTransient<TemplateParametersViewModel>();

        // Register Controllers
        services.AddSingleton<ApplicationController>();
        services.AddSingleton<GenerationController>();
        services.AddSingleton<GenerationRibbonViewModel>();
        
        services.AddSingleton<SettingsController>();
        
        
        // Register Workspace Controllers
        services.AddSingleton<WorkspaceController>();
        services.AddSingleton<WorkspaceTreeViewController>();
        services.AddSingleton<IWorkspaceContextProvider>(sp => sp.GetRequiredService<WorkspaceTreeViewController>());
        services.AddSingleton<IDatasourceFactory, DatasourceFactory>();
       
        services.AddSingleton<WorkspaceRibbonViewModel>();
        // Register Template Controllers
        services.AddSingleton<TemplateTreeViewController>();
        services.AddSingleton<TemplateRibbonViewModel>();
        services.AddSingleton<TemplateController>();
        // Workspace Artifact Controllers
        services.AddSingleton<IWorkspaceArtifactController, WorkspaceArtifactController>();

        services.AddSingleton<IWorkspaceArtifactController, DatasourcesContainerController>();
        services.AddSingleton<IWorkspaceArtifactController, MysqlDatasourceController>();
        services.AddSingleton<IWorkspaceArtifactController, SqlServerDatasourceController>();
        services.AddSingleton<IWorkspaceArtifactController, PostgreSqlDatasourceController>();
        services.AddSingleton<IWorkspaceArtifactController, ExcelDatasourceController>();
        services.AddSingleton<IWorkspaceArtifactController, CsvDatasourceController>();
        services.AddSingleton<IWorkspaceArtifactController, JsonDatasourceController>();
        services.AddSingleton<IWorkspaceArtifactController, XmlDatasourceController>();
        services.AddSingleton<IWorkspaceArtifactController, YamlDatasourceController>();
        services.AddSingleton<IWorkspaceArtifactController, DotNetAssemblyDatasourceController>();
        services.AddSingleton<IWorkspaceArtifactController, OpenApiDatasourceController>();
        services.AddSingleton<IWorkspaceArtifactController, DirectoryDatasourceController>();
        services.AddSingleton<IWorkspaceArtifactController, TableArtifactController>();
        services.AddSingleton<IWorkspaceArtifactController, ViewArtifactController>();
        services.AddSingleton<IWorkspaceArtifactController, ColumnArtifactController>();
        services.AddSingleton<IWorkspaceArtifactController, IndexArtifactController>();
        services.AddSingleton<IWorkspaceArtifactController, ForeignKeyArtifactController>();

        services.AddSingleton<IWorkspaceArtifactController, DomainLayerController>();
        services.AddSingleton<IWorkspaceArtifactController, ScopesContainerController>();
        services.AddSingleton<IWorkspaceArtifactController, SubScopesContainerController>();
        services.AddSingleton<IWorkspaceArtifactController, ScopeArtifactController>();
        services.AddSingleton<IWorkspaceArtifactController, DomainController>();
        services.AddSingleton<IWorkspaceArtifactController, EntitiesContainerController>();
        services.AddSingleton<IWorkspaceArtifactController, EntityController>();
        services.AddSingleton<IWorkspaceArtifactController, EntityStatesContainerController>();
        services.AddSingleton<IWorkspaceArtifactController, EntityStateController>();
        services.AddSingleton<IWorkspaceArtifactController, PropertyController>();
        services.AddSingleton<IWorkspaceArtifactController, EntityRelationsContainerController>();
        services.AddSingleton<IWorkspaceArtifactController, EntityRelationController>();
        services.AddSingleton<IWorkspaceArtifactController, ValueTypesContainerController>();
        services.AddSingleton<IWorkspaceArtifactController, ValueTypeController>();
        services.AddSingleton<IWorkspaceArtifactController, EntityViewsContainerController>();
        services.AddSingleton<IWorkspaceArtifactController, EntityEditViewArtifactController>();
        services.AddSingleton<IWorkspaceArtifactController, EntityEditViewFieldController>();
        services.AddSingleton<IWorkspaceArtifactController, EntityListViewArtifactController>();
        services.AddSingleton<IWorkspaceArtifactController, EntityListViewColumnController>();
        services.AddSingleton<IWorkspaceArtifactController, EntitySelectViewArtifactController>();

        // Template Artifact Controllers
        services.AddSingleton<ITemplateArtifactController, RootArtifactController>();
        services.AddSingleton<ITemplateArtifactController, TemplateArtifactController>();
        services.AddSingleton<ITemplateArtifactController, ExistingFolderArtifactController>();

        // Register Datasource Providers
        services.AddMysqlDatasourceServices(configuration);
        services.AddSqlServerDatasourceServices(configuration);
        services.AddPostgreSqlDatasourceServices(configuration);
        services.AddExcelDatasourceServices(configuration);
        services.AddCsvDatasourceServices(configuration);
        services.AddJsonDatasourceServices(configuration);
        services.AddXmlDatasourceServices(configuration);
        services.AddYamlDatasourceServices(configuration);
        services.AddDotNetAssemblyDatasourceServices(configuration);
        services.AddOpenApiDatasourceServices(configuration);
        services.AddDirectoryDatasourceServices(configuration);

        // Register Message Bus systems
        services.AddSingleton<ApplicationMessageBus>();
        services.AddSingleton<GeneratorMessageBus>();
        services.AddSingleton<WorkspaceMessageBus>();
        
        services.AddWorkspaceServices(configuration);

        // Register Services
        services.AddSingleton<GeneratorOrchestrator>();
        services.AddSingleton<CodeArchitectureManager>();
        // Settings Managers
        services.AddSingleton<ApplicationSettingsManager>();
        services.AddSingleton<WorkspaceSettingsManager>();
        services.AddSingleton<TemplateEngineSettingsManager>();
        services.AddSingleton<GeneratorSettingsManager>();

        // Register Ribbon Builder
        services.AddSingleton<RibbonBuilder>(RibbonBuilder.Create());

        // Register Generators from other projects
        services.AddCodeArchitectureLayersServices(configuration);
        services.AddDotNetGeneratorServices(configuration);

        // Register Generator Descriptors
        // IMPORTANT: Ensure that all generators are registered here

        services.AddSingleton<TemplateManager>();
        services.AddSingleton<TemplatePathResolver>();
        services.AddSingleton<TemplateEngineManager>();
        // Register Template Engines
        services.AddSingleton<DotNetProjectTemplateEngine>();
        services.AddSingleton<ITemplateEngine, CodeGenerator.TemplateEngines.DotNetProject.DotNetProjectTemplateEngine>((q) => q.GetRequiredService<DotNetProjectTemplateEngine>());
        
        services.AddSingleton<CodeGenerator.TemplateEngines.DotNetProject.Services.DotNetProjectService>();
        services.AddSingleton<ITemplateEngine, CodeGenerator.TemplateEngines.Scriban.ScribanTemplateEngine>();
        services.AddSingleton<ITemplateEngine, CodeGenerator.TemplateEngines.T4.T4TemplateEngine>();
        services.AddSingleton<ITemplateEngine, CodeGenerator.TemplateEngines.PlantUML.PlantUmlTemplateEngine>();
        
        services.AddDotNetWinformsRibbonApplicationGeneratorServices(configuration);
        services.AddDotNetSharedScopeGeneratorServices(configuration);

        // Add logging
        services.AddLogging(builder =>
        {
            builder.AddDebug();
            builder.SetMinimumLevel(LogLevel.Debug); // Enable Debug level logging

            // Filter out noisy Microsoft and System logs
            builder.AddFilter("Microsoft", LogLevel.Warning);
            builder.AddFilter("System", LogLevel.Warning);
            builder.AddFilter("CodeGenerator", LogLevel.Debug); // Debug for your application
            //builder.AddFilter("CodeGenerator.Application", LogLevel.Debug);      // Alleen Application layer
            //builder.AddFilter("CodeGenerator.Core.Templates", LogLevel.Trace);   // Extra verbose voor Templates
            //builder.AddFilter("CodeGenerator.Generators", LogLevel.Information); // Minder verbose voor Generators
            builder.AddConfiguration(configuration.GetSection("Logging"));
        });

        return services;
    }
}
