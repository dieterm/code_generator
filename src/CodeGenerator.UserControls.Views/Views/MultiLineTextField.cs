using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.UserControls.Views
{
    public partial class MultiLineTextField : UserControl, IView<MultiLineTextFieldModel>
    {
        private MultiLineTextFieldModel _viewModel;

        public MultiLineTextField()
        {
            InitializeComponent();
            lblLabel.EnsureLabelVisible(txtValue, lblErrorMessage);
            Disposed += MultiLineTextField_Disposed;
        }

        private void MultiLineTextField_Disposed(object? sender, EventArgs e)
        {
            ClearBindings();
            if (_viewModel != null)
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
        /// Gets or sets visibility of the error message label
        /// </summary>
        [Category("Appearance")]
        [Description("Show or hide the error message for this field")]
        [DefaultValue(false)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ErrorMessageVisible
        {
            get => lblErrorMessage.Visible;
            set => lblErrorMessage.Visible = value;
        }

        private void ClearBindings()
        {
            lblLabel.DataBindings.Clear();
            txtValue.DataBindings.Clear();
            lblErrorMessage.DataBindings.Clear();
        }

        public void BindViewModel(MultiLineTextFieldModel viewModel)
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            if (_viewModel != null)
            {
                ClearBindings();
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }

            _viewModel = viewModel;

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;

            // Bind the ViewModel properties to the control's UI elements
            lblLabel.DataBindings.Add("Text", viewModel, nameof(viewModel.Label), false, DataSourceUpdateMode.OnPropertyChanged);
            txtValue.DataBindings.Add("Text", viewModel, nameof(viewModel.Value), false, DataSourceUpdateMode.OnValidation);
            lblErrorMessage.DataBindings.Add("Text", viewModel, nameof(viewModel.ErrorMessage), false, DataSourceUpdateMode.OnPropertyChanged);
            toolTip.SetToolTip(txtValue, viewModel.Tooltip);
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_viewModel.Tooltip))
            {
                toolTip.SetToolTip(txtValue, _viewModel.Tooltip);
            }
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((MultiLineTextFieldModel)(object)viewModel);
        }
    }
}
