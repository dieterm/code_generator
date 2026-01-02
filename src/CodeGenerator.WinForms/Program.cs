using CodeGenerator.Core.Interfaces;
using CodeGenerator.Core.Services;
using CodeGenerator.Generators;
using CodeGenerator.Generators.Application;
using CodeGenerator.Generators.Domain;
using CodeGenerator.Generators.Infrastructure;
using CodeGenerator.Generators.Presentation;
using CodeGenerator.Templates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.WinForms;

internal static class Program
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();

        Application.Run(ServiceProvider.GetRequiredService<MainForm>());
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Logging
        services.AddLogging(builder =>
        {
            builder.AddDebug();
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        // Core services
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<ISchemaParser, SchemaParser>();
        services.AddSingleton<ITemplateEngine, ScribanTemplateEngine>();
        services.AddSingleton<IProjectGenerator, DotNetProjectGenerator>();

        // Generators
        services.AddSingleton<ICodeGenerator, EntityGenerator>();
        services.AddSingleton<ICodeGenerator, DbContextGenerator>();
        services.AddSingleton<ICodeGenerator, RepositoryGenerator>();
        services.AddSingleton<ICodeGenerator, DbScriptGenerator>();
        services.AddSingleton<ICodeGenerator, ControllerGenerator>();
        services.AddSingleton<ICodeGenerator, ViewModelGenerator>();
        services.AddSingleton<ICodeGenerator, WinFormsViewGenerator>();

        // Orchestrator
        services.AddSingleton<GeneratorOrchestrator>();

        // Forms
        services.AddTransient<MainForm>();
    }
}
