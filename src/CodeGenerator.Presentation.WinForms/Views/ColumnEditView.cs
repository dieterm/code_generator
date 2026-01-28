using CodeGenerator.Application.ViewModels.Workspace;
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
                _viewModel.DataTypeField.PropertyChanged -= DataTypeField_PropertyChanged;
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }

            _viewModel = viewModel;

            if (_viewModel == null) return;

            // Bind fields
            txtName.BindViewModel(_viewModel.NameField);
            cbxDataType.BindViewModel(_viewModel.DataTypeField);
            
            txtMaxLength.BindViewModel(_viewModel.MaxLengthField);
            txtPrecision.BindViewModel(_viewModel.PrecisionField);
            txtScale.BindViewModel(_viewModel.ScaleField);
            txtAllowedValues.BindViewModel(_viewModel.AllowedValuesField);
            chkNullable.BindViewModel(_viewModel.IsNullableField);
            chkPrimaryKey.BindViewModel(_viewModel.IsPrimaryKeyField);
            chkAutoIncrement.BindViewModel(_viewModel.IsAutoIncrementField);
            txtDefaultValue.BindViewModel(_viewModel.DefaultValueField);
            
            // set initial visibility
            UpdateVisibilityFields();
            
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            _viewModel.DataTypeField.PropertyChanged += DataTypeField_PropertyChanged;
        }

        /// <summary>
        /// Update visibility of length, precision, scale, and allowed values fields based on selected data type
        /// </summary>
        private void UpdateVisibilityFields()
        {
            if (_viewModel == null) return;
            var dataType = _viewModel.DataTypeField.SelectedItem as DataTypeComboboxItem;
            if (dataType != null)
            {
                txtMaxLength.Visible = dataType.UseMaxLength;
                txtPrecision.Visible = dataType.UsePrecision;
                txtScale.Visible = dataType.UseScale;
                txtAllowedValues.Visible = dataType.UseAllowedValues;
            }
        }

        private void DataTypeField_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(sender is ComboboxFieldModel) 
            { 
                if (e.PropertyName == nameof(ComboboxFieldModel.SelectedItem))
                {
                    UpdateVisibilityFields();
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
                    _viewModel.DataTypeField.PropertyChanged -= DataTypeField_PropertyChanged;  
                }
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
