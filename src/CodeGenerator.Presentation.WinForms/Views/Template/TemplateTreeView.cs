using CodeGenerator.Application.ViewModels.Template;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;

namespace CodeGenerator.Presentation.WinForms.Views.Template
{
    /// <summary>
    /// TreeView control for displaying and browsing templates
    /// </summary>
    public partial class TemplateTreeView : ArtifactTreeView, IView<TemplateTreeViewModel>
    {
        public void BindViewModel(TemplateTreeViewModel viewModel)
        {
            base.BindViewModel(viewModel);
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((TemplateTreeViewModel)(object)viewModel);
        }
    }
}
