using CodeGenerator.Core.CodeElements.ViewModels.EditFields;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeGenerator.Core.CodeElements.Views.EditFields
{
    public partial class CodeFileElementField : UserControl, IView<CodeFileElementFieldModel>
    {
        private CodeFileElementFieldModel? _viewModel;
        public CodeFileElementField()
        {
            InitializeComponent();

            Disposed += CodeFileElementField_Disposed;
        }

        private void CodeFileElementField_Disposed(object? sender, EventArgs e)
        {
            if (_viewModel != null)
            {
                ClearBindings();
                _viewModel = null;
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

        public void BindViewModel(CodeFileElementFieldModel viewModel)
        {
            if (_viewModel != null)
            {
                ClearBindings();
            }

            _viewModel = viewModel;

            if(_viewModel!=null)
            {
                lblLabel.DataBindings.Add("Text", viewModel, nameof(viewModel.Label), false, DataSourceUpdateMode.OnPropertyChanged);
                lblErrorMessage.DataBindings.Add("Text", viewModel, nameof(viewModel.ErrorMessage), false, DataSourceUpdateMode.OnPropertyChanged);
                btnLoadCodeFileElement.Command = viewModel.LoadCommand;
                btnSaveCodeFileElement.Command = viewModel.SaveCommand;
            }
        }

        private void ClearBindings()
        {
            lblLabel.DataBindings.Clear();
            lblErrorMessage.DataBindings.Clear();
            btnLoadCodeFileElement.Command = null;
            btnSaveCodeFileElement.Command = null;
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : IViewModel
        {
            BindViewModel((CodeFileElementFieldModel)(object)viewModel);
        }
    }
}
