using CodeGenerator.Core.ViewModels.Browser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Services
{
    public interface IBrowserWindowManagerService
    {
        void ShowBrowserWindow(string url);
        void ShowBrowserWindow(BrowserViewModel viewModel);
    }
}
