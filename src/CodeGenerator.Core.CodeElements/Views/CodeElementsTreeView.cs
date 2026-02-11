using CodeGenerator.Core.Artifacts.ViewModels;
using CodeGenerator.Core.CodeElements.ViewModels;
using CodeGenerator.Presentation.WinForms.Views;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.CodeElements.Views
{
    public class CodeElementsTreeView : ArtifactTreeView, IView<CodeElementsTreeViewModel>
    {
        public void BindViewModel(CodeElementsTreeViewModel viewModel)
        {
            base.BindViewModel(viewModel);
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((CodeElementsTreeViewModel)(object)viewModel);
        }
    }
}
