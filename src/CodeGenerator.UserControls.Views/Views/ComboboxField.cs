using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.ViewModels;
using Microsoft.DotNet.DesignTools.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeGenerator.UserControls.Views
{
    public partial class ComboboxField : UserControl, IView<ComboboxFieldModel>
    {
        private ComboboxFieldModel? _viewModel;
        public ComboboxField()
        {
            InitializeComponent();

            lblLabel.EnsureLabelVisible(cbxItems, lblErrorMessage);
            cbxItems.DisplayMember = "DisplayName";
            cbxItems.ValueMember = "Id";

            Disposed += ComboboxField_Disposed;
        }

        private void ComboboxField_Disposed(object? sender, EventArgs e)
        {
            ClearDataBindings();
            if(_viewModel!=null)
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
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
            get => lblLabel.Text;
            set => lblLabel.Text = value;
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
            get => lblErrorMessage.Visible;
            set => lblErrorMessage.Visible = value;
        }

        /// <summary>
        /// Gets or sets the label text for the field
        /// </summary>
        [Category("Appearance")]
        [Description("Show or hide the error message for this field")]
        [DefaultValue("DisplayName")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ItemsDisplayMember
        {
            get => cbxItems.DisplayMember;
            set => cbxItems.DisplayMember = value;
        }

        private void ClearDataBindings()
        {
            lblLabel.DataBindings.Clear();
            cbxItems.DataBindings.Clear();
            lblErrorMessage.DataBindings.Clear();
        }

        public void BindViewModel(ComboboxFieldModel viewModel)
        {
            if(_viewModel != null)
            {
                ClearDataBindings();
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }

            _viewModel = viewModel;

            if (_viewModel == null)
                return;

            // Bind the ViewModel properties to the control's UI elements
            lblLabel.DataBindings.Add("Text", viewModel, nameof(viewModel.Label), false, DataSourceUpdateMode.OnPropertyChanged);
            lblErrorMessage.DataBindings.Add("Text", viewModel, nameof(viewModel.ErrorMessage), false, DataSourceUpdateMode.OnPropertyChanged);
            
            // For SfComboBox, we need to set DataSource and handle SelectedValue manually
            cbxItems.DataSource = viewModel.Items;
            cbxItems.SelectedItem = viewModel.Value;
            cbxItems.DisplayMember = viewModel.DisplayMember;

            // Subscribe to PropertyChanged to update the control when ViewModel changes
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            
            // Subscribe to selection changes to update the ViewModel
            cbxItems.SelectedValueChanged += (s, e) =>
            {
                if (_viewModel != null)
                {
                    _viewModel.Value = cbxItems.SelectedValue;
                }
            };
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_viewModel == null) return;

            if (e.PropertyName == nameof(_viewModel.Items))
            {
                cbxItems.DataSource = _viewModel.Items;
            }
            else if (e.PropertyName == nameof(_viewModel.Value))
            {
                cbxItems.SelectedItem = _viewModel.Value;
            }
            else if(e.PropertyName == nameof(_viewModel.DisplayMember))
            {
                cbxItems.DisplayMember = _viewModel.DisplayMember;
            }
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((ComboboxFieldModel)(object)viewModel);
        }
    }
}
