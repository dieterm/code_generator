using CodeGenerator.Application.Services;
using CodeGenerator.Presentation.WinForms.Services;
using CodeGenerator.Shared.Ribbon;
using CodeGenerator.UserControls.Ribbon;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGenerator.Presentation.WinForms;

/// <summary>
/// Extension methods for configuring services in the DI container
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all application services in the DI container
    /// </summary>
    public static IServiceCollection AddPresentationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IApplicationService, ApplicationService>();
        services.AddSingleton<IMessageBoxService, MessageBoxService>();
        services.AddSingleton<IFileSystemDialogService, FileSystemDialogService>();
        services.AddSingleton<IRibbonRenderer, SyncfusionRibbonRenderer>();
        return services;
    }
}
