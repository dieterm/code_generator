using CodeGenerator.Core;
using CodeGenerator.Core.Events;
using CodeGenerator.Core.Interfaces;
using CodeGenerator.Core.Services;
using CodeGenerator.Generators;
using CodeGenerator.Generators.Application;
using CodeGenerator.Generators.Domain;
using CodeGenerator.Generators.Infrastructure;
using CodeGenerator.Generators.Presentation;
using CodeGenerator.Generators.Shared;
using CodeGenerator.Templates;
using CodeGenerator.WinForms.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CodeGenerator.Generators;
using CodeGenerator.Core;

namespace CodeGenerator.WinForms;

internal static class Program
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;
    public static LoggingService LoggingService { get; } = new LoggingService();

    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();
        ServiceProviderHolder.Initialize(ServiceProvider);
        Application.Run(ServiceProvider.GetRequiredService<MainForm>());
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Logging - add both Debug and LoggingService logger
        services.AddLogging(builder =>
        {
            builder.AddDebug();
            builder.AddLoggingService(LoggingService, LogLevel.Information);
            builder.SetMinimumLevel(LogLevel.Debug);
        });
        // Preview service
        services.AddSingleton<UserControlPreviewService>();
        services.AddSingleton<LoggingService>(LoggingService);
        // Core
        services.AddCodeGeneratorCoreServices();
        // Generators
        services.AddCodeGeneratorGeneratorsServices();

        // Forms
        services.AddTransient<MainForm>();
    }
}
