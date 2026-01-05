using CodeGenerator.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Shared.Views
{
    public interface IView<TModel> where TModel : ViewModelBase
    {
        void BindViewModel(TModel viewModel);
    }
}
