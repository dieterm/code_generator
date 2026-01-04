using System.ComponentModel;

namespace CodeGenerator.UserControls.Views
{
    public partial class DateOnlyField : UserControl
    {
        private ViewModels.DateOnlyFieldModel? _viewModel;
        private bool _isUpdatingFromViewModel = false;

        public DateOnlyField()
        {
            InitializeComponent();

            chkHasValue.CheckedChanged += ChkHasValue_CheckedChanged;
            dtpValue.ValueChanged += DtpValue_ValueChanged;
            dtpValue.Enabled = chkHasValue.Checked;

            Disposed += DateOnlyField_Disposed;
        }

        private void DateOnlyField_Disposed(object? sender, EventArgs e)
        {
            ClearDataBindings();
            if (_viewModel != null)
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        private void ChkHasValue_CheckedChanged(object? sender, EventArgs e)
        {
            dtpValue.Enabled = chkHasValue.Checked;
            if (!_isUpdatingFromViewModel && _viewModel != null)
            {
                if (chkHasValue.Checked)
                {
                    _viewModel.Value = DateOnly.FromDateTime(dtpValue.Value);
                }
                else
                {
                    _viewModel.Value = null;
                }
            }
        }

        private void DtpValue_ValueChanged(object? sender, EventArgs e)
        {
            if (!_isUpdatingFromViewModel && _viewModel != null && chkHasValue.Checked)
            {
                _viewModel.Value = DateOnly.FromDateTime(dtpValue.Value);
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

        public void BindViewModel(ViewModels.DateOnlyFieldModel viewModel)
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

            UpdateDateFromViewModel();

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_viewModel == null) return;

            if (e.PropertyName == nameof(_viewModel.Value))
            {
                UpdateDateFromViewModel();
            }
        }

        private void UpdateDateFromViewModel()
        {
            if (_viewModel == null) return;

            _isUpdatingFromViewModel = true;
            try
            {
                if (_viewModel.Value is DateOnly dateValue)
                {
                    chkHasValue.Checked = true;
                    dtpValue.Value = dateValue.ToDateTime(TimeOnly.MinValue);
                }
                else
                {
                    chkHasValue.Checked = false;
                    dtpValue.Value = DateTime.Today;
                }
            }
            finally
            {
                _isUpdatingFromViewModel = false;
            }
        }
    }
}
