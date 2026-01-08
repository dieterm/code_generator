using Microsoft.DotNet.DesignTools.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeGenerator.Shared.Views;
using CodeGenerator.Shared.ViewModels;

namespace CodeGenerator.UserControls.Views
{
    public partial class DenominationField : UserControl, IView<DenominationFieldModel>
    {
        DenominationFieldModel _viewModel;
        private string _label = "Field Label:"; // fallback label during design time
        public DenominationField()
        {
            InitializeComponent();

            txtDutch.TextboxWidth = 0;
            txtFrench.TextboxWidth = 0;
            txtEnglish.TextboxWidth = 0;
            txtGerman.TextboxWidth = 0;
            Disposed += DenominationField_Disposed;
        }
        /// <summary>
        /// Gets or sets the label text for the field
        /// </summary>
        [Category("Appearance")]
        [Description("The label text displayed for this field")]
        [DefaultValue("Field Label:")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Label
        {
            get => _viewModel==null? _label : _viewModel.Label;
            set
            {
                _label = value;
                if (_viewModel != null)
                {
                    _viewModel.Label = value;
                    
                }
                UpdateLabels(value);
            }
        }

        /// <summary>
        /// Gets or sets the label text for the field
        /// </summary>
        [Category("Appearance")]
        [Description("Show or hide the error message for this field")]
        [DefaultValue("Error Message")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ErrorMessageVisible
        {
            get => txtDutch.ErrorMessageVisible;
            set => txtDutch.ErrorMessageVisible = value;
        }
        private void DenominationField_Disposed(object? sender, EventArgs e)
        {
            // unsubscribe from model
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }
        }

        public void BindViewModel(DenominationFieldModel viewModel)
        {
            // unsubscribe from old model
            if(_viewModel!=null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }
            _viewModel = viewModel;
            txtDutch.BindViewModel(viewModel.Dutch);
            txtFrench.BindViewModel(viewModel.French);
            txtEnglish.BindViewModel(viewModel.English);
            txtGerman.BindViewModel(viewModel.German);
            if (_viewModel != null) {
                UpdateLabels(_viewModel.Label);
            }
            
            UpdateIsRequired();
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }
        private bool isUpdatingLabels = false;
        private void UpdateLabels(string? label)
        {
            if(isUpdatingLabels) return;
            isUpdatingLabels = true;
            if (_viewModel != null)
            {
                _viewModel.Dutch.Label = $"{label} (NL)";
                _viewModel.French.Label = $"{label} (FR)";
                _viewModel.English.Label = $"{label} (EN)";
                _viewModel.German.Label = $"{label} (DE)";
            }
            else
            {
                txtDutch.Label = $"{label} (NL)"; 
                txtFrench.Label = $"{label} (FR)";
                txtEnglish.Label = $"{label} (EN)";
                txtGerman.Label = $"{label} (DE)";
            }
            isUpdatingLabels = false;
        }
        private void UpdateIsRequired()
        {
            if (_viewModel != null) { 
                _viewModel.Dutch.IsRequired = _viewModel.IsRequired;
                _viewModel.French.IsRequired = _viewModel.IsRequired;
                _viewModel.English.IsRequired = _viewModel.IsRequired;
                _viewModel.German.IsRequired = _viewModel.IsRequired;   
            } 
        }
        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_viewModel==null) return;

            if(e.PropertyName == nameof(DenominationFieldModel.ErrorMessage))
            {
                _viewModel.Dutch.ErrorMessage = _viewModel.ErrorMessage;
            }
            else if(e.PropertyName == nameof(DenominationFieldModel.Label))
            {
                UpdateLabels(_viewModel.Label);
            }
            else if(e.PropertyName == nameof(DenominationFieldModel.IsRequired))
            {
                UpdateIsRequired();
            }
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((DenominationFieldModel)(object)viewModel);
        }
    }
}
