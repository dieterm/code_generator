using CodeGenerator.Core.Workspaces.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.Views;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views.Domains
{
    /// <summary>
    /// View for editing entity relation properties
    /// </summary>
    public partial class EntityRelationEditView : UserControl, IView<EntityRelationEditViewModel>
    {
        private EntityRelationEditViewModel? _viewModel;

        public EntityRelationEditView()
        {
            InitializeComponent();
        }

        public void BindViewModel(EntityRelationEditViewModel? viewModel)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }

            _viewModel = viewModel;

            if (_viewModel == null) return;

            txtName.BindViewModel(_viewModel.NameField);
            cmbTargetEntity.BindViewModel(_viewModel.TargetEntityField);
            cmbSourceCardinality.BindViewModel(_viewModel.SourceCardinalityField);
            cmbTargetCardinality.BindViewModel(_viewModel.TargetCardinalityField);
            txtSourcePropertyName.BindViewModel(_viewModel.SourcePropertyNameField);
            txtTargetPropertyName.BindViewModel(_viewModel.TargetPropertyNameField);

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((EntityRelationEditViewModel)(object)viewModel);
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
