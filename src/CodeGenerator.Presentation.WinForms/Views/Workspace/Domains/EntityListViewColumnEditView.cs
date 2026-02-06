using CodeGenerator.Application.ViewModels.Workspace.Domains;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.Views;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views.Domains
{
    /// <summary>
    /// View for editing EntityListViewColumnArtifact properties
    /// </summary>
    public partial class EntityListViewColumnEditView : UserControl, IView<EntityListViewColumnEditViewModel>
    {
        private EntityListViewColumnEditViewModel? _viewModel;

        public EntityListViewColumnEditView()
        {
            InitializeComponent();
        }

        public void BindViewModel(EntityListViewColumnEditViewModel? viewModel)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }

            _viewModel = viewModel;

            if (_viewModel == null) return;

            txtPropertyPath.BindViewModel(_viewModel.PropertyPathField);
            txtHeaderText.BindViewModel(_viewModel.HeaderTextField);
            cmbColumnType.BindViewModel(_viewModel.ColumnTypeField);
            numWidth.BindViewModel(_viewModel.WidthField);
            numDisplayOrder.BindViewModel(_viewModel.DisplayOrderField);
            txtFormatString.BindViewModel(_viewModel.FormatStringField);
            chkIsVisible.BindViewModel(_viewModel.IsVisibleField);
            chkIsSortable.BindViewModel(_viewModel.IsSortableField);
            chkIsFilterable.BindViewModel(_viewModel.IsFilterableField);

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((EntityListViewColumnEditViewModel)(object)viewModel);
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
