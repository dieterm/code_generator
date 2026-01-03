using CodeGenerator.Core.Events;
using CodeGenerator.Core.Interfaces;
using CodeGenerator.Core.Services;
using CodeGenerator.Generators.Application;
using CodeGenerator.Generators.Domain;
using CodeGenerator.Generators.Infrastructure;
using CodeGenerator.Generators.Presentation;
using CodeGenerator.Generators.Shared;
using CodeGenerator.Templates;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGenerator.Generators;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCodeGeneratorGeneratorsServices(this IServiceCollection services)
    {
        services.AddSingleton<ITemplateEngine, ScribanTemplateEngine>();
        services.AddSingleton<IProjectGenerator, DotNetProjectGenerator>();

        // Generators - register concrete types first, then forward to interface
        services.AddSingleton<SharedProjectGenerator>();
        services.AddSingleton<IMessageBusAwareGenerator>(sp => sp.GetRequiredService<SharedProjectGenerator>());

        services.AddSingleton<DomainProjectGenerator>();
        services.AddSingleton<IMessageBusAwareGenerator>(sp => sp.GetRequiredService<DomainProjectGenerator>());

        services.AddSingleton<ApplicationProjectGenerator>();
        services.AddSingleton<IMessageBusAwareGenerator>(sp => sp.GetRequiredService<ApplicationProjectGenerator>());

        services.AddSingleton<InfrastructureProjectGenerator>();
        services.AddSingleton<IMessageBusAwareGenerator>(sp => sp.GetRequiredService<InfrastructureProjectGenerator>());

        services.AddSingleton<PresentationProjectGenerator>();
        services.AddSingleton<IMessageBusAwareGenerator>(sp => sp.GetRequiredService<PresentationProjectGenerator>());

        services.AddSingleton<UserControlsProjectGenerator>();
        services.AddSingleton<IMessageBusAwareGenerator>(sp => sp.GetRequiredService<UserControlsProjectGenerator>());

        // Orchestrator
        services.AddSingleton<GeneratorOrchestrator>();
        return services;
    }
}
