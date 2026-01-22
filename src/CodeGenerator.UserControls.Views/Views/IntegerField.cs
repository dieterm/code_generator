using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.UserControls.Views
{
    public partial class IntegerField : UserControl, IView<ViewModels.IntegerFieldModel>
    {
        private ViewModels.IntegerFieldModel? _viewModel;
        private bool _isUpdatingFromViewModel = false;

        public IntegerField()
        {
            InitializeComponent();
            lblLabel.EnsureLabelVisible(nudValue, lblErrorMessage);
            nudValue.ValueChanged += NudValue_ValueChanged;

            Disposed += IntegerField_Disposed;
        }

        private void IntegerField_Disposed(object? sender, EventArgs e)
        {
            ClearDataBindings();
            if (_viewModel != null)
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        private void NudValue_ValueChanged(object? sender, EventArgs e)
        {
            if (!_isUpdatingFromViewModel && _viewModel != null)
            {
                _viewModel.Value = (int)nudValue.Value;
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

        [Category("Data")]
        [Description("The minimum value")]
        [DefaultValue(0)]
        [Browsable(true)]
        public int Minimum
        {
            get => (int)nudValue.Minimum;
            set => nudValue.Minimum = value;
        }

        [Category("Data")]
        [Description("The maximum value")]
        [DefaultValue(100)]
        [Browsable(true)]
        public int Maximum
        {
            get => (int)nudValue.Maximum;
            set => nudValue.Maximum = value;
        }

        private void ClearDataBindings()
        {
            lblLabel.DataBindings.Clear();
            lblErrorMessage.DataBindings.Clear();
        }

        public void BindViewModel(ViewModels.IntegerFieldModel viewModel)
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

            // Set min/max from ViewModel if provided
            if (viewModel.Minimum.HasValue)
                nudValue.Minimum = viewModel.Minimum.Value;
            if (viewModel.Maximum.HasValue)
                nudValue.Maximum = viewModel.Maximum.Value;
            
            toolTip.SetToolTip(nudValue, viewModel.Tooltip);
            
            UpdateValueFromViewModel();

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_viewModel == null) return;

            if (e.PropertyName == nameof(_viewModel.Value))
            {
                UpdateValueFromViewModel();
            }
            else if (e.PropertyName == nameof(_viewModel.Minimum) && _viewModel.Minimum.HasValue)
            {
                nudValue.Minimum = _viewModel.Minimum.Value;
            }
            else if (e.PropertyName == nameof(_viewModel.Maximum) && _viewModel.Maximum.HasValue)
            {
                nudValue.Maximum = _viewModel.Maximum.Value;
            } else if(e.PropertyName == nameof(_viewModel.Tooltip))
            {
                toolTip.SetToolTip(nudValue, _viewModel.Tooltip);
            }

        }

        private void UpdateValueFromViewModel()
        {
            if (_viewModel == null) return;

            _isUpdatingFromViewModel = true;
            try
            {
                if (_viewModel.Value is int intValue)
                {
                    nudValue.Value = intValue;
                }
                else
                {
                    nudValue.Value = 0;
                }
            }
            finally
            {
                _isUpdatingFromViewModel = false;
            }
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((IntegerFieldModel)(object)viewModel);
        }
    }
}
