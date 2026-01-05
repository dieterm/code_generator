using CodeGenerator.Presentation.WinForms.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Application.Services
{
    public interface IApplicationService
    {
        void RunApplication(MainViewModel mainViewModel);
        void ExitApplication();
    }
}
