using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Workspaces.MessageBus;
using CodeGenerator.Generators.DotNet.DomainLayer.Generators;
using CodeGenerator.Generators.DotNet.DomainLayer.Workspace.Subscribers;
using CodeGenerator.Shared.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGenerator.Generators.DotNet.ApplicationScope;

/// <summary>
/// Extension methods for configuring services in the DI container
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all application services in the DI container
    /// </summary>
    public static IServiceCollection AddDotNetDomainLayerGeneratorServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register Generators
        services.AddTransient<IMessageBusAwareGenerator, EntityTemplateGenerator>();

        // Workspace Message bus subscribers
        services.AddSingleton<IWorkspaceMessageBusSubscriber, EntityArtifactConstructionSubscriber>();
        // workspace controllers
        //services.AddSingleton<IWorkspaceArtifactController, ApplicationViewModelArtifactController>();

        // viewmodels
        //services.AddSingleton<ApplicationViewModelArtifactEditViewModel>();

        // View mappings
        //services.AddTransient<IView<ApplicationViewModelArtifactEditViewModel>, ApplicationViewModelArtifactEditView>();

        return services;
    }
}
