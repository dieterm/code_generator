using CodeGenerator.Application;
using CodeGenerator.Application.Controllers;
using CodeGenerator.Application.Services;
using CodeGenerator.Plugin.Host;
using CodeGenerator.Presentation.WinForms.Services;
using CodeGenerator.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Syncfusion.Windows.Forms;
using System.Configuration;
using System.Diagnostics;

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
            var currentFolder = Path.GetDirectoryName(typeof(Program).Assembly.Location);
            var licensesPath = Path.Combine(currentFolder!, "SyncfusionLicenseKey.txt");
            Debug.WriteLine(licensesPath);
            if (!File.Exists(licensesPath))
            {
                // Show input prompt to enter license key
                string licenseKey = Microsoft.VisualBasic.Interaction.InputBox("No licence key found at " + licensesPath + ".\r\nEnter Syncfusion License Key:", "License Key Required", "");
                if(string.IsNullOrWhiteSpace(licenseKey))
                {
                    MessageBox.Show("A valid Syncfusion license key is required to run this application.", "License Key Missing", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }
                File.WriteAllText(licensesPath, licenseKey);
            }
            
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(File.ReadAllText(licensesPath));
            SkinManager.LoadAssembly(typeof(Syncfusion.WinForms.Themes.Office2019Theme).Assembly);

            // Eenmalig bij applicatie start - VOOR je MSBuild gebruikt
            Microsoft.Build.Locator.MSBuildLocator.RegisterDefaults();

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

            // Register PluginManager as singleton
            services.AddSingleton<PluginManager>();
            
            var serviceProvider = services.BuildServiceProvider();
            
            // Store service provider for global access
            ServiceProviderHolder.Initialize(serviceProvider);

            // Wire up PluginViewFactory as fallback for the ViewFactory
            ViewFactory.PluginViewFactoryFallback = PluginViewFactory.Instance.TryCreateView;

            // Load global plugins (from <app>/Plugins/)
            var pluginManager = ServiceProviderHolder.GetRequiredService<PluginManager>();
            pluginManager.LoadGlobalPlugins();

            // Get application controller and start
            using (var scope = serviceProvider.CreateScope())
            {
                var applicationController = ServiceProviderHolder.GetRequiredService<ApplicationController>();
                applicationController.Initialize();
                applicationController.StartApplication();
            }

            // Dispose plugin manager (shuts down all plugins)
            pluginManager.Dispose();

            // Dispose service provider
            if (serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}