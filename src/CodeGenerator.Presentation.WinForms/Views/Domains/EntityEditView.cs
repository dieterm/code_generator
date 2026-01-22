using CodeGenerator.Application.ViewModels.Workspace.Domains;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.Views;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views.Domains
{
    /// <summary>
    /// View for editing entity properties
    /// </summary>
    public partial class EntityEditView : UserControl, IView<EntityEditViewModel>
    {
        private EntityEditViewModel? _viewModel;

        public EntityEditView()
        {
            InitializeComponent();
        }

        public void BindViewModel(EntityEditViewModel? viewModel)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }

            _viewModel = viewModel;

            if (_viewModel == null) return;

            txtName.BindViewModel(_viewModel.NameField);
            txtDescription.BindViewModel(_viewModel.DescriptionField);

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((EntityEditViewModel)(object)viewModel);
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
