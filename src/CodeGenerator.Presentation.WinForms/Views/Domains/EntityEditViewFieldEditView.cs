using CodeGenerator.Application.ViewModels.Workspace.Domains;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.Views;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views.Domains
{
    /// <summary>
    /// View for editing EntityEditViewFieldArtifact properties
    /// </summary>
    public partial class EntityEditViewFieldEditView : UserControl, IView<EntityEditViewFieldEditViewModel>
    {
        private EntityEditViewFieldEditViewModel? _viewModel;

        public EntityEditViewFieldEditView()
        {
            InitializeComponent();
        }

        public void BindViewModel(EntityEditViewFieldEditViewModel? viewModel)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }

            _viewModel = viewModel;

            if (_viewModel == null) return;

            txtPropertyName.BindViewModel(_viewModel.PropertyNameField);
            cmbControlType.BindViewModel(_viewModel.ControlTypeField);
            txtLabel.BindViewModel(_viewModel.LabelField);
            txtTooltip.BindViewModel(_viewModel.TooltipField);
            txtPlaceholder.BindViewModel(_viewModel.PlaceholderField);
            chkIsReadOnly.BindViewModel(_viewModel.IsReadOnlyField);
            chkIsRequired.BindViewModel(_viewModel.IsRequiredField);
            numDisplayOrder.BindViewModel(_viewModel.DisplayOrderField);

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((EntityEditViewFieldEditViewModel)(object)viewModel);
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
