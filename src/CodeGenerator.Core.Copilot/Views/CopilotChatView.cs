using CodeGenerator.Core.Copilot.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeGenerator.Core.Copilot
{
    public partial class CopilotChatView : UserControl, IView<CopilotChatViewModel>
    {
        public CopilotChatView()
        {
            InitializeComponent();
        }

        public void BindViewModel(CopilotChatViewModel viewModel)
        {
            
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((CopilotChatViewModel)(object)viewModel);
        }
    }
}
