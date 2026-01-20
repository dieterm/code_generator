using CodeGenerator.Application.Controllers;
using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Controllers.Template;
using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Application.Controllers.Workspace.Datasources;
using CodeGenerator.Application.Services;
using CodeGenerator.Application.ViewModels;
using CodeGenerator.Application.ViewModels.Template;
using CodeGenerator.Core.DomainSchema.Services;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Interfaces;
using CodeGenerator.Core.MessageBus;
using CodeGenerator.Core.Settings.Application;
using CodeGenerator.Core.Settings.Generators;
using CodeGenerator.Core.Settings.ViewModels;
using CodeGenerator.Core.Templates;
using CodeGenerator.Core.Workspaces.Datasources.Csv;
using CodeGenerator.Core.Workspaces.Datasources.Excel;
using CodeGenerator.Core.Workspaces.Datasources.Json;
using CodeGenerator.Core.Workspaces.Datasources.Mysql;
using CodeGenerator.Core.Workspaces.Datasources.PostgreSql;
using CodeGenerator.Core.Workspaces.Datasources.SqlServer;
using CodeGenerator.Core.Workspaces.Datasources.Xml;
using CodeGenerator.Core.Workspaces.Datasources.Yaml;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Core.Workspaces.Settings;
using CodeGenerator.Domain.CodeArchitecture;
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
        // Register ViewModels
        services.AddTransient<MainViewModel>();
        services.AddTransient<DomainSchemaTreeViewModel>();
        services.AddTransient<GenerationResultTreeViewModel>();
        services.AddTransient<ArtifactPreviewViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<TemplateTreeViewModel>();
        services.AddTransient<TemplateParametersViewModel>();

        // Register Controllers
        services.AddSingleton<ApplicationController>();
        services.AddSingleton<DomainSchemaController>();
        services.AddSingleton<GenerationController>();
        services.AddSingleton<SettingsController>();
        //services.AddSingleton<TemplateController>();
        
        // Register Workspace Controllers
        services.AddSingleton<WorkspaceTreeViewController>();
        services.AddSingleton<IWorkspaceContextProvider>(sp => sp.GetRequiredService<WorkspaceTreeViewController>());
        services.AddSingleton<IDatasourceFactory, DatasourceFactory>();
        services.AddSingleton<WorkspaceFileService>();

        // Register Template Controllers
        services.AddSingleton<TemplateTreeViewController>();

        // Workspace Artifact Controllers
        services.AddSingleton<WorkspaceArtifactController>();
        services.AddSingleton<DatasourcesContainerController>();
        services.AddSingleton<MysqlDatasourceController>();
        services.AddSingleton<SqlServerDatasourceController>();
        services.AddSingleton<PostgreSqlDatasourceController>();
        services.AddSingleton<ExcelDatasourceController>();
        services.AddSingleton<CsvDatasourceController>();
        services.AddSingleton<JsonDatasourceController>();
        services.AddSingleton<XmlDatasourceController>();
        services.AddSingleton<YamlDatasourceController>();
        services.AddSingleton<TableArtifactController>();
        services.AddSingleton<ViewArtifactController>();
        services.AddSingleton<ColumnArtifactController>();
        services.AddSingleton<IndexArtifactController>();
        services.AddSingleton<ForeignKeyArtifactController>();

        // Template Artifact Controllers
        services.AddSingleton<RootArtifactController>();
        services.AddSingleton<TemplateArtifactController>();
        services.AddSingleton<ExistingFolderArtifactController>();

        // Register Datasource Providers
        services.AddMysqlDatasourceServices(configuration);
        services.AddSqlServerDatasourceServices(configuration);
        services.AddPostgreSqlDatasourceServices(configuration);
        services.AddExcelDatasourceServices(configuration);
        services.AddCsvDatasourceServices(configuration);
        services.AddJsonDatasourceServices(configuration);
        services.AddXmlDatasourceServices(configuration);
        services.AddYamlDatasourceServices(configuration);
        
        // Register Message Bus systems
        services.AddSingleton<ApplicationMessageBus>();
        services.AddSingleton<GeneratorMessageBus>();

        // Register Services
        services.AddSingleton<DomainSchemaParser>();
        services.AddTransient<GeneratorOrchestrator>();
        services.AddSingleton<CodeArchitectureManager>();
        // Settings Managers
        services.AddSingleton<ApplicationSettingsManager>();
        services.AddSingleton<WorkspaceSettingsManager>();
        services.AddSingleton<GeneratorSettingsManager>();

        // Register Ribbon Builder
        services.AddSingleton<RibbonBuilder>(RibbonBuilder.Create());

        // Register Generators from other projects
        services.AddCodeArchitectureLayersServices(configuration);
        services.AddDotNetGeneratorServices(configuration);

        // Register Generator Descriptors
        // IMPORTANT: Ensure that all generators are registered here

        services.AddSingleton<TemplateEngineManager>();
        // Register Template Engines
        services.AddSingleton<DotNetProjectTemplateEngine>();
        services.AddSingleton<ITemplateEngine, CodeGenerator.TemplateEngines.DotNetProject.DotNetProjectTemplateEngine>((q) => q.GetRequiredService<DotNetProjectTemplateEngine>());
        
        services.AddSingleton<CodeGenerator.TemplateEngines.DotNetProject.Services.DotNetProjectService>();
        services.AddSingleton<ITemplateEngine, CodeGenerator.TemplateEngines.Scriban.ScribanTemplateEngine>();
        services.AddSingleton<ITemplateEngine, CodeGenerator.TemplateEngines.T4.T4TemplateEngine>();
        services.AddSingleton<ITemplateEngine, CodeGenerator.TemplateEngines.PlantUML.PlantUmlTemplateEngine>();

        // Add logging
        services.AddLogging(builder =>
        {
            builder.AddDebug();
            builder.AddConfiguration(configuration.GetSection("Logging"));
        });

        return services;
    }
}
