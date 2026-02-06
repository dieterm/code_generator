using CodeGenerator.Application.ViewModels.Workspace.Domains.Specifications;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.Views;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views.Workspace.Domains.Specifications
{
    /// <summary>
    /// View for editing domain specification properties
    /// </summary>
    public partial class DomainSpecificationEditView : UserControl, IView<DomainSpecificationEditViewModel>
    {
        private DomainSpecificationEditViewModel? _viewModel;

        public DomainSpecificationEditView()
        {
            InitializeComponent();
        }

        public void BindViewModel(DomainSpecificationEditViewModel? viewModel)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }

            _viewModel = viewModel;

            if (_viewModel == null) return;

            txtName.BindViewModel(_viewModel.NameField);
            txtDescription.BindViewModel(_viewModel.DescriptionField);
            txtCriteria.BindViewModel(_viewModel.CriteriaField);
            txtCategory.BindViewModel(_viewModel.CategoryField);
            chkIsComposite.BindViewModel(_viewModel.IsCompositeField);
            chkIsReusable.BindViewModel(_viewModel.IsReusableField);

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((DomainSpecificationEditViewModel)(object)viewModel);
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
