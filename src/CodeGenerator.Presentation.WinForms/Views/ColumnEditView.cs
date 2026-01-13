using CodeGenerator.Application.ViewModels;
using CodeGenerator.Core.Workspaces.Models;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.ViewModels;
using CodeGenerator.UserControls.Views;
using Microsoft.DotNet.DesignTools.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views
{
    /// <summary>
    /// View for editing column properties
    /// </summary>
    public partial class ColumnEditView : UserControl, IView<ColumnEditViewModel>
    {
        private ColumnEditViewModel? _viewModel;

        public ColumnEditView()
        {
            InitializeComponent();
        }

        public void BindViewModel(ColumnEditViewModel viewModel)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }

            _viewModel = viewModel;

            if (_viewModel == null) return;

            // Bind fields
            txtName.BindViewModel(_viewModel.NameField);
            cbxDataType.BindViewModel(_viewModel.DataTypeField);
            _viewModel.DataTypeField.PropertyChanged += DataTypeField_PropertyChanged;
            txtMaxLength.BindViewModel(_viewModel.MaxLengthField);
            txtPrecision.BindViewModel(_viewModel.PrecisionField);
            txtScale.BindViewModel(_viewModel.ScaleField);
            chkNullable.BindViewModel(_viewModel.IsNullableField);
            chkPrimaryKey.BindViewModel(_viewModel.IsPrimaryKeyField);
            chkAutoIncrement.BindViewModel(_viewModel.IsAutoIncrementField);
            txtDefaultValue.BindViewModel(_viewModel.DefaultValueField);

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void DataTypeField_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ComboboxFieldModel.Value))
            {
                // Update visibility of length, precision, and scale fields based on selected data type
                var dataType = _viewModel?.DataTypeField.Value as DataTypeComboboxItem;
                if (dataType != null)
                {
                    // Example logic for visibility; adjust according to actual data types
                    txtMaxLength.Visible = dataType.UseMaxLength;
                    txtPrecision.Visible = dataType.UsePrecision;
                    txtScale.Visible = dataType.UseScale;
                }
            }
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Handle property changes if needed
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((ColumnEditViewModel)(object)viewModel);
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
