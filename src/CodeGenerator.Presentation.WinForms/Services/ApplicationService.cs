using CodeGenerator.Application.Services;
using CodeGenerator.Presentation.WinForms.ViewModels;
using CodeGenerator.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Presentation.WinForms.Services
{
    public class ApplicationService : IApplicationService
    {
        public void RunApplication(MainViewModel mainViewModel)
        {
            // Start the application with the provided ViewModel
            var mainView = ServiceProviderHolder.GetRequiredService<MainView>();
           
            mainView.BindViewModel(mainViewModel);
            System.Windows.Forms.Application.Run(mainView);
        }

        public void ExitApplication()
        {
            // Stop the application
            System.Windows.Forms.Application.Exit();
        }
    }
}
