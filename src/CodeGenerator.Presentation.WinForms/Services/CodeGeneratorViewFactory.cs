using CodeGenerator.Core.Workspaces.ViewModels.Common;
using CodeGenerator.Presentation.WinForms.Views.Workspace;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Presentation.WinForms.Services
{
    public class CodeGeneratorViewFactory : ViewFactory
    {
        public CodeGeneratorViewFactory(IServiceProvider serviceProvider) 
            : base(serviceProvider)
        {
        }

        public override IView? CreateView(IViewModel viewModel)
        {
            if(viewModel is IArtifactEditViewModel)
            {
                var view = new ArtifactEditView();
                view.BindViewModel(viewModel);
                return view;
            }
            return base.CreateView(viewModel);
        }
    }
}
