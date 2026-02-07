using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Application.Controllers.Workspace.Datasources;
using CodeGenerator.Application.Services;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.MessageBus;
using CodeGenerator.Core.Workspaces.Datasources.Csv;
using CodeGenerator.Presentation.WinForms.Resources;
using CodeGenerator.Presentation.WinForms.Services;
using CodeGenerator.Presentation.WinForms.Views.Application;
using CodeGenerator.Shared.Ribbon;
using CodeGenerator.UserControls;
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
        services.AddSharedUserControlViews();

        // Register Datasources
        services.AddCsvDatasourceServices(configuration);
        // Main form
        //var mainView = new MainView();
        //services.AddSingleton<MainView>(mainView);
        //var mainView = new MainView();
        services.AddSingleton<MainView>();
        services.AddSingleton<IWindowManagerService>((s) => new WindowManagerService(Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<MainView>(s)));
        // Resources
        services.AddSingleton<ITreeNodeIconResolver<ResourceManagerTreeNodeIcon>, ResourceManagerTreeNodeIconResolver>();
        
        return services;
    }
}
