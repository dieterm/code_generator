using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;

namespace CodeGenerator.Presentation.WinForms.Views.Workspace
{
    public class WorkspaceTreeView : ArtifactTreeView, IView<WorkspaceTreeViewModel>
    {
        public void BindViewModel(WorkspaceTreeViewModel viewModel)
        {
            base.BindViewModel(viewModel);
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((WorkspaceTreeViewModel)(object)viewModel);
        }
    }
    
}
