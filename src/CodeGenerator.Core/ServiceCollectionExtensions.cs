using CodeGenerator.Core.Events;
using CodeGenerator.Core.Interfaces;
using CodeGenerator.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGenerator.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCodeGeneratorCoreServices(this IServiceCollection services)
    {
        // Core services
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<ISchemaParser, SchemaParser>();


        // Message Bus
        services.AddSingleton<IGeneratorMessageBus, GeneratorMessageBus>();

        // Runtime compilation services
        services.AddSingleton<RuntimeCompiler>();
        return services;
    }
}
