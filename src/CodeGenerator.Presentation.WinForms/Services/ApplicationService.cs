using CodeGenerator.Application.Services;
using CodeGenerator.Presentation.WinForms.ViewModels;
using CodeGenerator.Shared;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Presentation.WinForms.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly ILogger<ApplicationService> _logger;

        public ApplicationService(ILogger<ApplicationService> logger)
        {
            _logger = logger;
        }

        public void RunApplication(MainViewModel mainViewModel)
        {
            // Start the application with the provided ViewModel
            var mainView = ServiceProviderHolder.GetRequiredService<MainView>();
           
            mainView.BindViewModel(mainViewModel);
            System.Windows.Forms.Application.ThreadException += (sender, args) =>
            {
                // Handle unhandled exceptions
                // Log the exception or show a message box
                _logger.LogError(args.Exception, "An unhandled error occurred.");
                System.Windows.Forms.MessageBox.Show($"An unhandled error occurred: {args.Exception.Message}", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            };
            System.Windows.Forms.Application.Run(mainView);
        }

        public void ExitApplication()
        {
            // Stop the application
            System.Windows.Forms.Application.Exit();
        }
    }
}
