using CodeGenerator.Shared;
using CodeGenerator.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CodeGenerator.Application.ViewModels.Template
{
    public class TemplateRibbonViewModel : ViewModelBase
    {
        public ICommand ShowTemplatesCommand { get; }
        public ICommand RefreshTemplatesCommand { get; }
        public ICommand ShowCodeElementsToolCommand { get; }

        public event EventHandler? RequestShowTemplates;
        public event EventHandler? RequestRefreshTemplates;
        public event EventHandler? RequestShowCodeElementsTool;

        public TemplateRibbonViewModel()
        {
            ShowTemplatesCommand = new RelayCommand((e) => RequestShowTemplates?.Invoke(this, EventArgs.Empty));
            RefreshTemplatesCommand = new RelayCommand((e) => RequestRefreshTemplates?.Invoke(this, EventArgs.Empty));
            ShowCodeElementsToolCommand = new RelayCommand((e) => RequestShowCodeElementsTool?.Invoke(this, EventArgs.Empty));
        }
    }
}
