using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Workspaces.MessageBus;
using CodeGenerator.Generators.DotNet.Generators;
using CodeGenerator.Generators.DotNet.Generators.ApplicationLayer.ApplicationScope;
using CodeGenerator.Generators.DotNet.Generators.ApplicationLayer.DomainScope;
using CodeGenerator.Generators.DotNet.Generators.ApplicationLayer.SharedScope;
using CodeGenerator.Generators.DotNet.Generators.DomainLayer.ApplicationScope;
using CodeGenerator.Generators.DotNet.Generators.DomainLayer.DomainScope;
using CodeGenerator.Generators.DotNet.Generators.DomainLayer.SharedScope;
using CodeGenerator.Generators.DotNet.Generators.InfrastructureLayer.ApplicationScope;
using CodeGenerator.Generators.DotNet.Generators.InfrastructureLayer.DomainScope;
using CodeGenerator.Generators.DotNet.Generators.InfrastructureLayer.SharedScope;
using CodeGenerator.Generators.DotNet.Generators.PresentationLayer.ApplicationScope;
using CodeGenerator.Generators.DotNet.Generators.PresentationLayer.DomainScope;
using CodeGenerator.Generators.DotNet.Generators.PresentationLayer.SharedScope;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGenerator.Generators.CodeArchitectureLayers;

/// <summary>
/// Extension methods for configuring services in the DI container
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all application services in the DI container
    /// </summary>
    public static IServiceCollection AddDotNetGeneratorServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register Generators
        // ApplicationLayer
        services.AddTransient<IMessageBusAwareGenerator, ApplicationLayerApplicationScopeDotNetProjectGenerator>();
        services.AddTransient<IMessageBusAwareGenerator, ApplicationLayerSharedScopeDotNetProjectGenerator>();
        services.AddTransient<IMessageBusAwareGenerator, ApplicationLayerDomainScopeDotNetProjectGenerator>();
        // DomainLayer
        services.AddTransient<IMessageBusAwareGenerator, DomainLayerApplicationScopeDotNetProjectGenerator>();
        services.AddTransient<IMessageBusAwareGenerator, DomainLayerSharedScopeDotNetProjectGenerator>();
        services.AddTransient<IMessageBusAwareGenerator, DomainLayerDomainScopeDotNetProjectGenerator>();
        // PresentationLayer
        services.AddTransient<IMessageBusAwareGenerator, PresentationLayerApplicationScopeDotNetProjectGenerator>();
        services.AddTransient<IMessageBusAwareGenerator, PresentationLayerSharedScopeDotNetProjectGenerator>();
        services.AddTransient<IMessageBusAwareGenerator, PresentationLayerDomainScopeDotNetProjectGenerator>();
        // InfrastructureLayer
        services.AddTransient<IMessageBusAwareGenerator, InfrastructureLayerApplicationScopeDotNetProjectGenerator>();
        services.AddTransient<IMessageBusAwareGenerator, InfrastructureLayerSharedScopeDotNetProjectGenerator>();
        services.AddTransient<IMessageBusAwareGenerator, InfrastructureLayerDomainScopeDotNetProjectGenerator>();

        services.AddTransient<IMessageBusAwareGenerator, EntitiesClassGenerator>();


        return services;
    }
}
