using CodeGenerator.Application.Controllers;
using CodeGenerator.Application.MessageBus;
using CodeGenerator.Application.Services;
using CodeGenerator.Application.ViewModels;
using CodeGenerator.Core.DomainSchema.Services;
using CodeGenerator.Core.Generators;
using CodeGenerator.Presentation.WinForms.ViewModels;
using CodeGenerator.Generators.CodeArchitectureLayers;
using CodeGenerator.Shared.Ribbon;
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

        // Register Controllers
        services.AddSingleton<ApplicationController>();
        services.AddSingleton<DomainSchemaController>();
        services.AddSingleton<GenerationController>();

        // Register Message Bus systems
        services.AddSingleton<ApplicationMessageBus>();
        services.AddSingleton<GeneratorMessageBus>();

        // Register Services
        services.AddSingleton<DomainSchemaParser>();
        services.AddTransient<GeneratorOrchestrator>();

        // Register Ribbon Builder
        services.AddSingleton<RibbonBuilder>(RibbonBuilder.Create());

        // Register Generators from other projects
        services.AddCodeArchitectureLayersServices(configuration);
        services.AddDotNetGeneratorServices(configuration);
        
        // Add logging
        services.AddLogging(builder =>
        {
            builder.AddDebug();
            builder.AddConfiguration(configuration.GetSection("Logging"));
        });

        return services;
    }
}
