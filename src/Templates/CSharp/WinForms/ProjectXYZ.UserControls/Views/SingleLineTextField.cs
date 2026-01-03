using System.ComponentModel;
using ProjectXYZ.UserControls.ViewModels;

namespace ProjectXYZ.UserControls.Views
{
    public partial class SingleLineTextField : UserControl
    {
        private SingleLineTextFieldModel _viewModel;

        public SingleLineTextField()
        {
            InitializeComponent();
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
        /// <summary>
        /// Gets or sets the label text for the field
        /// </summary>
        [Category("Appearance")]
        [Description("Show or hide the error message for this field")]
        [DefaultValue(0)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int TextboxWidth
        {
            get => _textboxWidth;
            set {
                _textboxWidth = value;
                if (value==0) {
                    txtValue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                    txtValue.Width = this.Width - txtValue.Left - 5;
                }
                else
                {
                    txtValue.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                    txtValue.Width = value;
                }
            }
        }

        public void BindViewModel(ViewModels.SingleLineTextFieldModel viewModel)
        {
            if (_viewModel != null)
            {
                lblLabel.DataBindings.Clear();
                txtValue.DataBindings.Clear();
                lblErrorMessage.DataBindings.Clear();
            }
            _viewModel = viewModel;
            // Bind the ViewModel properties to the control's UI elements
            lblLabel.DataBindings.Add("Text", viewModel, nameof(viewModel.Label), false, DataSourceUpdateMode.OnPropertyChanged);
            txtValue.DataBindings.Add("Text", viewModel, nameof(viewModel.Value), false, DataSourceUpdateMode.OnPropertyChanged);
            lblErrorMessage.DataBindings.Add("Text", viewModel, nameof(viewModel.ErrorMessage), false, DataSourceUpdateMode.OnPropertyChanged);
        }
    }
}
