using CodeGenerator.Application.ViewModels.Workspace.Domains;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.Views;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views.Domains
{
    /// <summary>
    /// View for editing EntitySelectViewArtifact properties
    /// </summary>
    public partial class EntitySelectViewEditView : UserControl, IView<EntitySelectViewEditViewModel>
    {
        private EntitySelectViewEditViewModel? _viewModel;

        public EntitySelectViewEditView()
        {
            InitializeComponent();
        }

        public void BindViewModel(EntitySelectViewEditViewModel? viewModel)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }

            _viewModel = viewModel;

            if (_viewModel == null) return;

            txtName.BindViewModel(_viewModel.NameField);
            cmbDisplayMemberPath.BindViewModel(_viewModel.DisplayMemberPathField);
            cmbValueMemberPath.BindViewModel(_viewModel.ValueMemberPathField);
            txtDisplayFormat.BindViewModel(_viewModel.DisplayFormatField);
            cmbSearchPropertyPath.BindViewModel(_viewModel.SearchPropertyPathField);
            cmbSortPropertyPath.BindViewModel(_viewModel.SortPropertyPathField);
            chkSortAscending.BindViewModel(_viewModel.SortAscendingField);

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((EntitySelectViewEditViewModel)(object)viewModel);
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
