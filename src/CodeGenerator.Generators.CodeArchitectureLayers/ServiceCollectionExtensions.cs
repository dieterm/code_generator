using CodeGenerator.Core.Generators;
using CodeGenerator.Generators.CodeArchitectureLayers.ApplicationLayer;
using CodeGenerator.Generators.CodeArchitectureLayers.DomainLayer;
using CodeGenerator.Generators.CodeArchitectureLayers.InfrastructureLayer;
using CodeGenerator.Generators.CodeArchitectureLayers.PresentationLayer;
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
    public static IServiceCollection AddCodeArchitectureLayersServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register Generators
        services.AddTransient<IMessageBusAwareGenerator, DomainLayerGenerator>();
        services.AddTransient<IMessageBusAwareGenerator, ApplicationLayerGenerator>();
        services.AddTransient<IMessageBusAwareGenerator, InfrastructureLayerGenerator>();
        services.AddTransient<IMessageBusAwareGenerator, PresentationLayerGenerator>();

        return services;
    }
}
