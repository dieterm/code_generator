using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.UserControls.Views
{
    public partial class BooleanField : UserControl, IView<ViewModels.BooleanFieldModel>
    {
        private ViewModels.BooleanFieldModel? _viewModel;
        private bool _isUpdatingFromViewModel = false;

        public BooleanField()
        {
            InitializeComponent();
            
            lblLabel.EnsureLabelVisible(rbYes, lblErrorMessage, (newLeft) => { rbNo.Left = rbYes.Left + 55; });

            rbYes.CheckedChanged += RbYes_CheckedChanged;
            rbNo.CheckedChanged += RbNo_CheckedChanged;

            Disposed += BooleanField_Disposed;
        }

        private void BooleanField_Disposed(object? sender, EventArgs e)
        {
            ClearDataBindings();
            if (_viewModel != null)
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        private void RbYes_CheckedChanged(object? sender, EventArgs e)
        {
            if (!_isUpdatingFromViewModel && _viewModel != null && rbYes.Checked)
            {
                _viewModel.Value = true;
            }
        }

        private void RbNo_CheckedChanged(object? sender, EventArgs e)
        {
            if (!_isUpdatingFromViewModel && _viewModel != null && rbNo.Checked)
            {
                _viewModel.Value = false;
            }
        }

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

        private void ClearDataBindings()
        {
            lblLabel.DataBindings.Clear();
            lblErrorMessage.DataBindings.Clear();
        }

        public void BindViewModel(ViewModels.BooleanFieldModel viewModel)
        {
            if (_viewModel != null)
            {
                ClearDataBindings();
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }

            _viewModel = viewModel;

            if (_viewModel == null)
                return;

            lblLabel.DataBindings.Add("Text", viewModel, nameof(viewModel.Label), false, DataSourceUpdateMode.OnPropertyChanged);
            lblErrorMessage.DataBindings.Add("Text", viewModel, nameof(viewModel.ErrorMessage), false, DataSourceUpdateMode.OnPropertyChanged);
            toolTip.SetToolTip(rbYes, _viewModel.Tooltip);
            toolTip.SetToolTip(rbNo, _viewModel.Tooltip);
            UpdateRadioButtonsFromViewModel();

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_viewModel == null) return;

            if (e.PropertyName == nameof(_viewModel.Value))
            {
                UpdateRadioButtonsFromViewModel();
            }
            else if(e.PropertyName == nameof(_viewModel.Tooltip))
            {
                toolTip.SetToolTip(rbYes, _viewModel.Tooltip);
                toolTip.SetToolTip(rbNo, _viewModel.Tooltip);
            }
        }

        private void UpdateRadioButtonsFromViewModel()
        {
            if (_viewModel == null) return;

            _isUpdatingFromViewModel = true;
            try
            {
                if (_viewModel.Value is bool boolValue)
                {
                    rbYes.Checked = boolValue;
                    rbNo.Checked = !boolValue;
                }
                else
                {
                    rbYes.Checked = false;
                    rbNo.Checked = false;
                }
            }
            finally
            {
                _isUpdatingFromViewModel = false;
            }
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((BooleanFieldModel)(object)viewModel);
        }
    }
}
