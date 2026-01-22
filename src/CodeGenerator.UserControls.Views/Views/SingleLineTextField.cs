using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.ViewModels;
using Microsoft.DotNet.DesignTools.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.UserControls.Views
{
    public partial class SingleLineTextField : UserControl, IView<SingleLineTextFieldModel>
    {
        private SingleLineTextFieldModel _viewModel;

        public SingleLineTextField()
        {
            InitializeComponent();
            lblLabel.EnsureLabelVisible(txtValue, lblErrorMessage);
            Disposed += SingleLineTextField_Disposed;
        }

        private void SingleLineTextField_Disposed(object? sender, EventArgs e)
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

        private void ClearBindings()
        {
            lblLabel.DataBindings.Clear();
            txtValue.DataBindings.Clear();
            lblErrorMessage.DataBindings.Clear();
        }

        public void BindViewModel(ViewModels.SingleLineTextFieldModel viewModel)
        {
            if(viewModel == null)
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
            txtValue.DataBindings.Add("Text", viewModel, nameof(viewModel.Value), false, DataSourceUpdateMode.OnPropertyChanged);
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
            BindViewModel((SingleLineTextFieldModel)(object)viewModel);
        }
    }
}
