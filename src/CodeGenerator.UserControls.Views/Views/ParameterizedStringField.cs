using CodeGenerator.Shared.Models;
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
    public partial class ParameterizedStringField : UserControl, IView<ParameterizedStringFieldModel>
    {
        public ParameterizedStringFieldModel ViewModel { get; private set; }
        public ParameterizedStringField()
        {
            InitializeComponent();
            lblLabel.EnsureLabelVisible(txtValue, lblErrorMessage, (newLeft) =>
            {
                lblPreview.Left = newLeft;
            });

            Disposed += ParameterizedStringField_Disposed;
        }

        private void ParameterizedStringField_Disposed(object? sender, EventArgs e)
        {
            ClearDataBindings();
            if (ViewModel != null)
                ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        public void BindViewModel(ParameterizedStringFieldModel viewModel)
        {
            if(viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            if (ViewModel != null)
            {
                ClearDataBindings();
                ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }

            ViewModel = viewModel;
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            // Bind the ViewModel properties to the control's UI elements
            lblLabel.DataBindings.Add("Text", viewModel, nameof(viewModel.Label), false, DataSourceUpdateMode.OnPropertyChanged);
            txtValue.DataBindings.Add("Text", viewModel, nameof(viewModel.Value), false, DataSourceUpdateMode.OnPropertyChanged);
            lblErrorMessage.DataBindings.Add("Text", viewModel, nameof(viewModel.ErrorMessage), false, DataSourceUpdateMode.OnPropertyChanged);
            lblPreview.DataBindings.Add("Text", viewModel, nameof(viewModel.Preview), false, DataSourceUpdateMode.OnPropertyChanged);
            toolTip.SetToolTip(txtValue, viewModel.Tooltip);
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.Tooltip))
            {
                toolTip.SetToolTip(txtValue, ViewModel.Tooltip);
            }
        }

        private void ClearDataBindings()
        {
            lblLabel.DataBindings.Clear();
            txtValue.DataBindings.Clear();
            lblPreview.DataBindings.Clear();
            lblErrorMessage.DataBindings.Clear();
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((ParameterizedStringFieldModel)(object)viewModel);
        }

        private void btnShowParameters_Click(object sender, EventArgs e)
        {
            ctxMenuParameters.Show(btnShowParameters, new Point(0, btnShowParameters.Height));
            
        }

        private void ctxMenuParameters_Opening(object sender, CancelEventArgs e)
        {
            ctxMenuParameters.Items.Clear();
            ViewModel.SelectionStart = txtValue.SelectionStart;
            foreach (var parameter in ViewModel.Parameters)
            {
                var item = new ToolStripMenuItem(parameter.Parameter);
                item.Tag = parameter;
                item.CommandParameter = parameter;
                item.Command = ViewModel.InsertParameter;
                ctxMenuParameters.Items.Add(item);
            }
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
        private int _textboxWidth = 0;
        ///// <summary>
        ///// Gets or sets the label text for the field
        ///// </summary>
        //[Category("Appearance")]
        //[Description("Show or hide the error message for this field")]
        //[DefaultValue(0)]
        //[Browsable(true)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        //public int TextboxWidth
        //{
        //    get => _textboxWidth;
        //    set
        //    {
        //        _textboxWidth = value;
        //        if (value == 0)
        //        {
        //            txtValue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        //            txtValue.Width = this.Width - txtValue.Left - 5;
        //        }
        //        else
        //        {
        //            txtValue.Anchor = AnchorStyles.Top | AnchorStyles.Left;
        //            txtValue.Width = value;
        //        }
        //    }
        //}

    }
}
