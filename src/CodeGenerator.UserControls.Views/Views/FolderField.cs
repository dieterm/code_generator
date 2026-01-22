using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;
using System.Diagnostics;

namespace CodeGenerator.UserControls.Views
{
    public partial class FolderField : UserControl, IView<ViewModels.FolderFieldModel>
    {
        private ViewModels.FolderFieldModel? _viewModel;

        public FolderField()
        {
            InitializeComponent();
            lblLabel.EnsureLabelVisible(txtValue, lblErrorMessage);
            btnBrowse.Click += BtnBrowse_Click;
            btnOpenFolder.Click += BtnOpenFolder_Click;

            Disposed += FolderField_Disposed;
        }

        private void FolderField_Disposed(object? sender, EventArgs e)
        {
            ClearDataBindings();
            if (_viewModel != null)
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        private void BtnBrowse_Click(object? sender, EventArgs e)
        {
            if (_viewModel == null) return;

            using var dialog = new FolderBrowserDialog();
            dialog.Description = _viewModel.Description ?? "Select a folder";
            dialog.ShowNewFolderButton = true;

            if (!string.IsNullOrEmpty(txtValue.Text) && Directory.Exists(txtValue.Text))
            {
                dialog.SelectedPath = txtValue.Text;
            }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _viewModel.Value = dialog.SelectedPath;
            }
        }

        private void BtnOpenFolder_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtValue.Text)) return;

            if (Directory.Exists(txtValue.Text))
            {
                Process.Start("explorer.exe", txtValue.Text);
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
            txtValue.DataBindings.Clear();
            lblErrorMessage.DataBindings.Clear();
        }

        public void BindViewModel(ViewModels.FolderFieldModel viewModel)
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
            txtValue.DataBindings.Add("Text", viewModel, nameof(viewModel.Value), false, DataSourceUpdateMode.OnPropertyChanged);
            lblErrorMessage.DataBindings.Add("Text", viewModel, nameof(viewModel.ErrorMessage), false, DataSourceUpdateMode.OnPropertyChanged);
            toolTip.SetToolTip(txtValue, viewModel.Tooltip);
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(FolderFieldModel.Tooltip))
            {
                toolTip.SetToolTip(txtValue, _viewModel.Tooltip);
            }
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((FolderFieldModel)(object)viewModel);
        }
    }
}
