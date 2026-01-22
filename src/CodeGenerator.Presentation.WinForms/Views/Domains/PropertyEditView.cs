using CodeGenerator.Application.ViewModels.Workspace.Domains;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.ViewModels;
using CodeGenerator.UserControls.Views;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views.Domains
{
    /// <summary>
    /// View for editing property artifact properties
    /// </summary>
    public partial class PropertyEditView : UserControl, IView<PropertyEditViewModel>
    {
        private PropertyEditViewModel? _viewModel;

        public PropertyEditView()
        {
            InitializeComponent();
        }

        public void BindViewModel(PropertyEditViewModel? viewModel)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }

            _viewModel = viewModel;

            if (_viewModel == null) return;

            txtName.BindViewModel(_viewModel.NameField);
            cmbDataType.BindViewModel(_viewModel.DataTypeField);
            chkIsNullable.BindViewModel(_viewModel.IsNullableField);
            numMaxLength.BindViewModel(_viewModel.MaxLengthField);
            numPrecision.BindViewModel(_viewModel.PrecisionField);
            numScale.BindViewModel(_viewModel.ScaleField);
            txtDescription.BindViewModel(_viewModel.DescriptionField);
            txtExampleValue.BindViewModel(_viewModel.ExampleValueField);

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            UpdateFieldVisibility();
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PropertyEditViewModel.ShowMaxLength) ||
                e.PropertyName == nameof(PropertyEditViewModel.ShowPrecisionScale))
            {
                UpdateFieldVisibility();
            }
        }

        private void UpdateFieldVisibility()
        {
            if (_viewModel == null) return;

            numMaxLength.Visible = _viewModel.ShowMaxLength;
            numPrecision.Visible = _viewModel.ShowPrecisionScale;
            numScale.Visible = _viewModel.ShowPrecisionScale;
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((PropertyEditViewModel)(object)viewModel);
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
