using CodeGenerator.Application.ViewModels.Generation;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;

namespace CodeGenerator.Presentation.WinForms.Views.Generation
{
    /// <summary>
    /// Tree view for displaying generation result artifacts.
    /// Inherits from ArtifactTreeView — no designer file needed.
    /// </summary>
    public class GenerationTreeView : ArtifactTreeView, IView<GenerationTreeViewModel>
    {
        public void BindViewModel(GenerationTreeViewModel viewModel)
        {
            base.BindViewModel(viewModel);
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((GenerationTreeViewModel)(object)viewModel);
        }
    }
}
