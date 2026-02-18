using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.UserControls.Views
{
    public partial class LabelField : UserControl, IView<LabelFieldModel>
    {
        private LabelFieldModel? _viewModel;

        public LabelField()
        {
            InitializeComponent();
            lblLabel.EnsureLabelVisible(lblValue, lblErrorMessage);
            Disposed += LabelField_Disposed;
        }

        private void LabelField_Disposed(object? sender, EventArgs e)
        {
            ClearBindings();
            if (_viewModel != null)
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
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

        private void ClearBindings()
        {
            lblLabel.DataBindings.Clear();
            lblErrorMessage.DataBindings.Clear();
        }

        public void BindViewModel(LabelFieldModel viewModel)
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            if (_viewModel != null)
            {
                ClearBindings();
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }

            _viewModel = viewModel;

            lblLabel.DataBindings.Add("Text", viewModel, nameof(viewModel.Label), false, DataSourceUpdateMode.OnPropertyChanged);
            lblErrorMessage.DataBindings.Add("Text", viewModel, nameof(viewModel.ErrorMessage), false, DataSourceUpdateMode.OnPropertyChanged);

            UpdateValueFromViewModel();
            toolTip.SetToolTip(lblValue, viewModel.Tooltip);

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_viewModel == null) return;

            if (e.PropertyName == nameof(_viewModel.Value))
            {
                UpdateValueFromViewModel();
            }
            else if (e.PropertyName == nameof(_viewModel.Tooltip))
            {
                toolTip.SetToolTip(lblValue, _viewModel.Tooltip);
            }
        }

        private void UpdateValueFromViewModel()
        {
            if (_viewModel == null) return;
            lblValue.Text = _viewModel.Value?.ToString() ?? string.Empty;
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((LabelFieldModel)(object)viewModel);
        }
    }
}
