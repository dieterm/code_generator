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
        public ICommand RequestShowTemplatesCommand { get; }
        public ICommand RequestRefreshTemplatesCommand { get; }

        public event EventHandler? RequestShowTemplates;
        public event EventHandler? RequestRefreshTemplates;

        public TemplateRibbonViewModel()
        {
            RequestShowTemplatesCommand = new RelayCommand((e) => RequestShowTemplates?.Invoke(this, EventArgs.Empty));
            RequestRefreshTemplatesCommand = new RelayCommand((e) => RequestRefreshTemplates?.Invoke(this, EventArgs.Empty));
        }
    }
}
