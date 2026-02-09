using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Workspaces.MessageBus;
using CodeGenerator.Generators.DotNet.WinformsRibbonApplication.Workspace.Subscribers;
using CodeGenerator.Shared.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGenerator.Generators.DotNet.WinformsRibbonApplication;

/// <summary>
/// Extension methods for configuring services in the DI container
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all application services in the DI container
    /// </summary>
    public static IServiceCollection AddDotNetWinformsRibbonApplicationGeneratorServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register Generators
        services.AddTransient<IMessageBusAwareGenerator, WinformsRibbonApplicationGenerator>();

        services.AddSingleton<IWorkspaceMessageBusSubscriber, OnionPresentationLayerContextMenuOpeningSubscriber>();
        services.AddSingleton<IWorkspaceMessageBusSubscriber, ViewsContainerToPresentationLayerArtifactChildAddedSubscriber>();
        services.AddSingleton<IWorkspaceMessageBusSubscriber, ControllersContainerToApplicationLayerArtifactChildAddedSubscriber>();

       

        return services;
    }
}
