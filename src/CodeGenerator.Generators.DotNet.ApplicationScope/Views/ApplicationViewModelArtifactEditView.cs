using CodeGenerator.Generators.DotNet.ApplicationScope.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.Views;
using System.ComponentModel;

namespace CodeGenerator.Generators.DotNet.ApplicationScope.Views
{
    /// <summary>
    /// View for editing ApplicationViewModelArtifact properties
    /// </summary>
    public partial class ApplicationViewModelArtifactEditView : UserControl, IView<ApplicationViewModelArtifactEditViewModel>
    {
        private ApplicationViewModelArtifactEditViewModel? _viewModel;

        public ApplicationViewModelArtifactEditView()
        {
            InitializeComponent();
        }

        public void BindViewModel(ApplicationViewModelArtifactEditViewModel? viewModel)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }

            _viewModel = viewModel;

            if (_viewModel == null) return;

            txtApplicationName.BindViewModel(_viewModel.ApplicationNameField);

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((ApplicationViewModelArtifactEditViewModel)(object)viewModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_viewModel != null)
                {
                    _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
                }
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
