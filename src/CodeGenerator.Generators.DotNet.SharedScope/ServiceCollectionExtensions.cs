using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Workspaces.MessageBus;
using CodeGenerator.Generators.DotNet.SharedScope.Generators;
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
    public static IServiceCollection AddDotNetSharedScopeGeneratorServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<IMessageBusAwareGenerator, DotNetSharedScopeGenerator>();

        return services;
    }
}
