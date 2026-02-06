using CodeGenerator.Application.ViewModels.Workspace.Domains;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.Views;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views.Domains
{
    /// <summary>
    /// View for editing scope properties
    /// </summary>
    public partial class ScopeEditView : UserControl, IView<ScopeEditViewModel>
    {
        private ScopeEditViewModel? _viewModel;

        public ScopeEditView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the ViewModel
        /// </summary>
        public ScopeEditViewModel? ViewModel
        {
            get => _viewModel;
            set => BindViewModel(value);
        }

        public void BindViewModel(ScopeEditViewModel? viewModel)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }

            _viewModel = viewModel;

            if (_viewModel == null) return;

            // Bind fields to their view models
            txtName.BindViewModel(_viewModel.NameField);
            txtNamespace.BindViewModel(_viewModel.NamespaceField);

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Handle property changes if needed
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((ScopeEditViewModel)(object)viewModel);
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
