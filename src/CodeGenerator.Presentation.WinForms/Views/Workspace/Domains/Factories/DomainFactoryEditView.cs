using CodeGenerator.Application.ViewModels.Workspace.Domains.Factories;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.Views;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views.Workspace.Domains.Factories
{
    /// <summary>
    /// View for editing domain factory properties
    /// </summary>
    public partial class DomainFactoryEditView : UserControl, IView<DomainFactoryEditViewModel>
    {
        private DomainFactoryEditViewModel? _viewModel;

        public DomainFactoryEditView()
        {
            InitializeComponent();
        }

        public void BindViewModel(DomainFactoryEditViewModel? viewModel)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }

            _viewModel = viewModel;

            if (_viewModel == null) return;

            txtName.BindViewModel(_viewModel.NameField);
            txtDescription.BindViewModel(_viewModel.DescriptionField);
            txtCategory.BindViewModel(_viewModel.CategoryField);
            chkCreatesAggregates.BindViewModel(_viewModel.CreatesAggregatesField);
            chkIsStateless.BindViewModel(_viewModel.IsStatelessField);
            chkHasDependencies.BindViewModel(_viewModel.HasDependenciesField);

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((DomainFactoryEditViewModel)(object)viewModel);
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
