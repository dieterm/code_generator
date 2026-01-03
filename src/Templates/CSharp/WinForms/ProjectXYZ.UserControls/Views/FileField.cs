using System.ComponentModel;
using System.Diagnostics;

namespace ProjectXYZ.UserControls.Views
{
    public partial class FileField : UserControl
    {
        private ViewModels.FileFieldModel? _viewModel;
        private bool _isUpdatingFromViewModel = false;

        public FileField()
        {
            InitializeComponent();

            btnBrowse.Click += BtnBrowse_Click;
            btnOpenFolder.Click += BtnOpenFolder_Click;

            Disposed += FileField_Disposed;
        }

        private void FileField_Disposed(object? sender, EventArgs e)
        {
            ClearDataBindings();
            if (_viewModel != null)
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        private void BtnBrowse_Click(object? sender, EventArgs e)
        {
            if (_viewModel == null) return;

            if (_viewModel.SelectionMode == ViewModels.FileSelectionMode.Open)
            {
                using var dialog = new OpenFileDialog();
                dialog.Filter = _viewModel.Filter ?? "All files (*.*)|*.*";
                dialog.DefaultExt = _viewModel.DefaultExtension;
                if (!string.IsNullOrEmpty(txtValue.Text) && File.Exists(txtValue.Text))
                {
                    dialog.InitialDirectory = Path.GetDirectoryName(txtValue.Text);
                    dialog.FileName = Path.GetFileName(txtValue.Text);
                }

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _viewModel.Value = dialog.FileName;
                }
            }
            else
            {
                using var dialog = new SaveFileDialog();
                dialog.Filter = _viewModel.Filter ?? "All files (*.*)|*.*";
                dialog.DefaultExt = _viewModel.DefaultExtension;
                if (!string.IsNullOrEmpty(txtValue.Text))
                {
                    dialog.InitialDirectory = Path.GetDirectoryName(txtValue.Text) ?? string.Empty;
                    dialog.FileName = Path.GetFileName(txtValue.Text);
                }

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _viewModel.Value = dialog.FileName;
                }
            }
        }

        private void BtnOpenFolder_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtValue.Text)) return;

            var directory = Path.GetDirectoryName(txtValue.Text);
            if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
            {
                Process.Start("explorer.exe", directory);
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

        [Category("Behavior")]
        [Description("The file selection mode (Open or Save)")]
        [DefaultValue(ViewModels.FileSelectionMode.Open)]
        [Browsable(true)]
        public ViewModels.FileSelectionMode SelectionMode
        {
            get => _viewModel?.SelectionMode ?? ViewModels.FileSelectionMode.Open;
            set
            {
                if (_viewModel != null)
                    _viewModel.SelectionMode = value;
                UpdateBrowseButtonIcon();
            }
        }

        private void ClearDataBindings()
        {
            lblLabel.DataBindings.Clear();
            txtValue.DataBindings.Clear();
            lblErrorMessage.DataBindings.Clear();
        }

        public void BindViewModel(ViewModels.FileFieldModel viewModel)
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

            UpdateBrowseButtonIcon();

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_viewModel == null) return;

            if (e.PropertyName == nameof(_viewModel.SelectionMode))
            {
                UpdateBrowseButtonIcon();
            }
        }

        private void UpdateBrowseButtonIcon()
        {
            if (_viewModel?.SelectionMode == ViewModels.FileSelectionMode.Save)
            {
                // Load save icon
                var savePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "save.png");
                if (File.Exists(savePath))
                    btnBrowse.BackgroundImage = Image.FromFile(savePath);
            }
            else
            {
                // Load file icon
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "file.png");
                if (File.Exists(filePath))
                    btnBrowse.BackgroundImage = Image.FromFile(filePath);
            }
        }
    }
}
