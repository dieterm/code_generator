using CodeGenerator.Application;
using CodeGenerator.Application.Controllers;
using CodeGenerator.Application.Services;
using CodeGenerator.Presentation.WinForms.Services;
using CodeGenerator.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Syncfusion.Windows.Forms;
using System.Configuration;

namespace CodeGenerator.Presentation.WinForms
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JGaF5cXGpCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWH1cc3VURmlZVkRxXUpWYEs=");
            SkinManager.LoadAssembly(typeof(Syncfusion.WinForms.Themes.Office2019Theme).Assembly);
            
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            // Build service provider
            var services = new ServiceCollection();

            // Configure DI container for all components
            services.AddApplicationServices(configuration);
            services.AddPresentationServices(configuration);

            var serviceProvider = services.BuildServiceProvider();

            // Store service provider for global access
            ServiceProviderHolder.Initialize(serviceProvider);

            // Get application controller and start
            using (var scope = serviceProvider.CreateScope())
            {
                var applicationController = ServiceProviderHolder.GetRequiredService<ApplicationController>();
                applicationController.StartApplication();
            }

            // Dispose service provider
            if (serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}