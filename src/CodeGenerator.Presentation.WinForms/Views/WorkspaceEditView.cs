using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.Views;
using System.ComponentModel;
using System.Xml.Linq;

namespace CodeGenerator.Presentation.WinForms.Views
{
    /// <summary>
    /// View for editing workspace properties
    /// </summary>
    public partial class WorkspaceEditView : UserControl, IView<WorkspaceEditViewModel>
    {
        private WorkspaceEditViewModel? _viewModel;

        public WorkspaceEditView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the ViewModel
        /// </summary>
        public WorkspaceEditViewModel? ViewModel
        {
            get => _viewModel;
            set => BindViewModel(value);
        }

        public void BindViewModel(WorkspaceEditViewModel? viewModel)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }

            _viewModel = viewModel;
            
            if (_viewModel == null) return;
            
            // Bind fields to their view models
            txtName.BindViewModel(_viewModel.NameField);
            txtRootNamespace.BindViewModel(_viewModel.RootNamespaceField);
            folderOutputDirectory.BindViewModel(_viewModel.DefaultOutputDirectoryField);
            cbxTargetFramework.BindViewModel(_viewModel.DefaultTargetFrameworkField);
            cbxLanguage.BindViewModel(_viewModel.DefaultLanguageField);

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Handle property changes if needed
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((WorkspaceEditViewModel)(object)viewModel);
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
