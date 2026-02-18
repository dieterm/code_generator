using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Workspaces.MessageBus;
using CodeGenerator.Generators.DotNet.Repositories.Csv.Controllers;
using CodeGenerator.Generators.DotNet.Repositories.Csv.ViewModels;
using CodeGenerator.Generators.DotNet.Repositories.Csv.Views;
using CodeGenerator.Generators.DotNet.Repositories.Csv.Workspace.Subscribers;
using CodeGenerator.Shared.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGenerator.Generators.DotNet.Repositories.Csv;

/// <summary>
/// Extension methods for configuring services in the DI container
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all application services in the DI container
    /// </summary>
    public static IServiceCollection AddDotNetCsvRepositoriesGeneratorServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register Generators
        services.AddTransient<IMessageBusAwareGenerator, CsvValueObjectReaderGenerator>();

        // Register Workspace Subscribers
        services.AddSingleton<IWorkspaceMessageBusSubscriber, TableArtifactContextMenuOpeningSubscriber>();

        // Register Controllers
        services.AddSingleton<IWorkspaceArtifactController, CsvValueObjectReaderController>();
        services.AddSingleton<IWorkspaceArtifactController, CsvValueObjectReaderImplementationController>();

        // Register Views and ViewModels
        services.AddTransient<IView<CsvValueObjectReaderArtifactEditViewModel>, CsvValueObjectReaderArtifactEditView>();
        services.AddTransient<IView<CsvValueObjectReaderImplementationArtifactEditViewModel>, CsvValueObjectReaderImplementationArtifactEditView>();

        return services;
    }
}
