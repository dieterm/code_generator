using CodeGenerator.Application.Controllers;
using CodeGenerator.Application.Controllers.ArtifactPreview;
using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Controllers.Generation;
using CodeGenerator.Application.Controllers.Template;
using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Application.Controllers.Workspace.Datasources;
using CodeGenerator.Application.Controllers.Workspace.Domains;
using CodeGenerator.Application.Controllers.Workspace.Domains.Entities;
using CodeGenerator.Application.Controllers.Workspace.Domains.Events;
using CodeGenerator.Application.Controllers.Workspace.Domains.Factories;
using CodeGenerator.Application.Controllers.Workspace.Domains.Repositories;
using CodeGenerator.Application.Controllers.Workspace.Domains.Services;
using CodeGenerator.Application.Controllers.Workspace.Domains.Specifications;
using CodeGenerator.Application.Controllers.Workspace.Domains.ValueTypes;
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
using CodeGenerator.Core.Workspaces.Operations.Domains;
using CodeGenerator.Core.Workspaces.Operations.Scopes;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Core.Workspaces.Settings;
using CodeGenerator.Domain;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.ProgrammingLanguages.CSharp;
using CodeGenerator.Domain.ProgrammingLanguages.Python;
using CodeGenerator.Generators.CodeArchitectureLayers;
using CodeGenerator.Presentation.WinForms.ViewModels;
using CodeGenerator.Shared.Operations;
using CodeGenerator.Shared.Ribbon;
using CodeGenerator.Shared.UndoRedo;
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

        // Register CodeGenerators
        // TODO: use DI-container to register generators instead of static registration
        CSharpCodeGeneratorExtensions.RegisterCSharpCodeGenerator();
        PythonCodeGeneratorExtensions.RegisterPythonCodeGenerator();

        // Register CodeParsers
        CSharpCodeParserExtensions.RegisterCSharpCodeParser();
        PythonCodeParserExtensions.RegisterPythonCodeParser();

        // Artifact Preview Controllers & ViewModels
        services.AddSingleton<ArtifactPreviewController>();
        services.AddTransient<ArtifactPreviewViewModel>();
        
        // Application Controllers
        services.AddSingleton<ApplicationController>();
        services.AddSingleton<MainViewModel>();

        // Generation Controllers & Viewmodels
        services.AddSingleton<GenerationController>();
        services.AddSingleton<GenerationTreeViewController>();
        services.AddSingleton<GenerationRibbonViewModel>();
        services.AddSingleton<GenerationResultTreeViewModel>();
        services.AddSingleton<GenerationTreeViewModel>();
        services.AddSingleton<GenerationDetailsViewModel>();

        // Generation Artifact Controllers
        services.AddSingleton<IGenerationArtifactController, GenerationArtifactController>();

        // Settings Controllers & ViewModels
        services.AddSingleton<SettingsController>
        ();
        services.AddTransient<SettingsViewModel>();

        // Register Template Controllers & ViewModels
        services.AddSingleton<TemplateTreeViewController>();
        services.AddSingleton<TemplateController>();
        services.AddTransient<TemplateTreeViewModel>();
        services.AddTransient<TemplateParametersViewModel>();
        services.AddSingleton<TemplateRibbonViewModel>();

        // Register Workspace Controllers
        services.AddSingleton<WorkspaceController>();
        services.AddSingleton<WorkspaceTreeViewController>();
        services.AddSingleton<IWorkspaceContextProvider>(sp => Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<WorkspaceTreeViewController>(sp));
        services.AddSingleton<IDatasourceFactory, DatasourceFactory>();
       
        services.AddSingleton<WorkspaceRibbonViewModel>();

        // Workspace Artifact Controllers
        services.AddSingleton<IWorkspaceArtifactController, WorkspaceArtifactController>();

        services.AddSingleton<IWorkspaceArtifactController, DatasourcesContainerController>();
        services.AddSingleton<IWorkspaceArtifactController, MysqlDatasourceController>();
        services.AddSingleton<IWorkspaceArtifactController, SqlServerDatasourceController>();
        services.AddSingleton<IWorkspaceArtifactController, PostgreSqlDatasourceController>();
        services.AddSingleton<IWorkspaceArtifactController, ExcelDatasourceController>();
        
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
        services.AddSingleton<IWorkspaceArtifactController, PropertiesContainerController>();
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

        // Domain Events Controllers
        services.AddSingleton<IWorkspaceArtifactController, DomainEventsContainerController>();
        services.AddSingleton<IWorkspaceArtifactController, DomainEventController>();

        // Domain Repositories Controllers
        services.AddSingleton<IWorkspaceArtifactController, DomainRepositoriesContainerController>();
        services.AddSingleton<IWorkspaceArtifactController, DomainRepositoryController>();
        services.AddSingleton<IWorkspaceArtifactController, DomainRepositoryMethodController>();

        // Domain Services Controllers
        services.AddSingleton<IWorkspaceArtifactController, DomainServicesContainerController>();
        services.AddSingleton<IWorkspaceArtifactController, DomainServiceController>();
        services.AddSingleton<IWorkspaceArtifactController, DomainServiceMethodController>();

        // Domain Specifications Controllers
        services.AddSingleton<IWorkspaceArtifactController, DomainSpecificationsContainerController>();
        services.AddSingleton<IWorkspaceArtifactController, DomainSpecificationController>();

        // Domain Factories Controllers
        services.AddSingleton<IWorkspaceArtifactController, DomainFactoriesContainerController>();
        services.AddSingleton<IWorkspaceArtifactController, DomainFactoryController>();

        // Template Artifact Controllers
        services.AddSingleton<ITemplateArtifactController, RootArtifactController>();
        services.AddSingleton<ITemplateArtifactController, TemplateArtifactController>();
        services.AddSingleton<ITemplateArtifactController, ExistingFolderArtifactController>();

        // Register Datasource Providers
        services.AddMysqlDatasourceServices(configuration);
        services.AddSqlServerDatasourceServices(configuration);
        services.AddPostgreSqlDatasourceServices(configuration);
        services.AddExcelDatasourceServices(configuration);
        //
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
        
        // Register Undo/Redo
        services.AddSingleton<UndoRedoManager>();

        // Register Operations
        services.AddSingleton<OperationExecutor>();
        services.AddSingleton<AddScopeToWorkspaceOperation>();
        services.AddSingleton<IOperation, AddScopeToWorkspaceOperation>(sp => Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<AddScopeToWorkspaceOperation>(sp));
        services.AddSingleton<AddSubScopeToScopeOperation>();
        services.AddSingleton<IOperation, AddSubScopeToScopeOperation>(sp => Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<AddSubScopeToScopeOperation>(sp));
        services.AddSingleton<AddDomainToScopeOperation>();
        services.AddSingleton<IOperation, AddDomainToScopeOperation>(sp => Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<AddDomainToScopeOperation>(sp));
        services.AddSingleton<AddEntityToDomainOperation>();
        services.AddSingleton<IOperation, AddEntityToDomainOperation>(sp => Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<AddEntityToDomainOperation>(sp));
        //services.AddSingleton<AddEntityWithPropertiesToDomainOperation>();
        //services.AddSingleton<IOperation, AddEntityWithPropertiesToDomainOperation>(sp => sp.GetRequiredService<AddEntityWithPropertiesToDomainOperation>());
        services.AddSingleton<AddPropertyToEntityOperation>();
        services.AddSingleton<IOperation, AddPropertyToEntityOperation>(sp => Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<AddPropertyToEntityOperation>(sp));
        services.AddSingleton<AddPropertyToEntityStateOperation>();
        services.AddSingleton<IOperation, AddPropertyToEntityStateOperation>(sp => Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<AddPropertyToEntityStateOperation>(sp));
        services.AddSingleton<AddPropertyToValueTypeOperation>();
        services.AddSingleton<IOperation, AddPropertyToValueTypeOperation>(sp => Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<AddPropertyToValueTypeOperation>(sp));
        services.AddSingleton<AddValueTypeToDomainOperation>();
        services.AddSingleton<IOperation, AddValueTypeToDomainOperation>(sp => Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<AddValueTypeToDomainOperation>(sp));

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
        services.AddSingleton<ITemplateEngine, CodeGenerator.TemplateEngines.DotNetProject.DotNetProjectTemplateEngine>((q) => Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<DotNetProjectTemplateEngine>(q  ));
        
        services.AddSingleton<CodeGenerator.TemplateEngines.DotNetProject.Services.DotNetProjectService>();
        services.AddSingleton<ITemplateEngine, CodeGenerator.TemplateEngines.Scriban.ScribanTemplateEngine>();
        services.AddSingleton<ITemplateEngine, CodeGenerator.TemplateEngines.T4.T4TemplateEngine>();
        services.AddSingleton<ITemplateEngine, CodeGenerator.TemplateEngines.PlantUML.PlantUmlTemplateEngine>();
        services.AddSingleton<ITemplateEngine, CodeGenerator.TemplateEngines.Folder.FolderTemplateEngine>();


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
