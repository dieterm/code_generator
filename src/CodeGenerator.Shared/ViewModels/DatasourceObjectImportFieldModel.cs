using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace CodeGenerator.Shared.ViewModels
{
    /// <summary>
    /// Column definition for the DatasourceObjectImportField ListView.
    /// </summary>
    public class DatasourceObjectColumnDefinition
    {
        public string HeaderText { get; set; } = string.Empty;
        public int Width { get; set; } = 100;
    }

    /// <summary>
    /// Represents a single row in the DatasourceObjectImportField ListView.
    /// Each item has a primary text (first column) and optional sub-item texts for additional columns.
    /// </summary>
    public class DatasourceObjectItemViewModel : ViewModelBase
    {
        public string Text { get; set; } = string.Empty;
        public List<string> SubItems { get; set; } = new();
        public string? ImageKey { get; set; }
        public object? Tag { get; set; }
    }

    /// <summary>
    /// Reusable FieldModel for datasource object import: GroupBox with buttons, info label, ListView, status and error labels.
    /// </summary>
    public class DatasourceObjectImportFieldModel : FieldViewModelBase
    {
        private string _groupBoxText = "Available Objects";
        private string _loadButtonText = "Load";
        private string _addSelectedButtonText = "Add Selected";
        private string _addAllButtonText = "Add All";
        private bool _addAllButtonVisible = false;
        private string? _infoText;
        private bool _infoLabelVisible = false;
        private string? _statusText;
        private string? _errorText;
        private bool _isLoading;
        private DatasourceObjectItemViewModel? _selectedItem;

        public DatasourceObjectImportFieldModel()
        {
            Name = "ObjectImport";
            Label = "Object Import";
            Columns = new List<DatasourceObjectColumnDefinition>();
            Items = new ObservableCollection<DatasourceObjectItemViewModel>();
        }

        /// <summary>
        /// Text displayed on the GroupBox.
        /// </summary>
        public string GroupBoxText
        {
            get => _groupBoxText;
            set => SetProperty(ref _groupBoxText, value);
        }

        /// <summary>
        /// Text for the first button (load/analyze/scan).
        /// </summary>
        public string LoadButtonText
        {
            get => _loadButtonText;
            set => SetProperty(ref _loadButtonText, value);
        }

        /// <summary>
        /// Text for the second button (add selected/import).
        /// </summary>
        public string AddSelectedButtonText
        {
            get => _addSelectedButtonText;
            set => SetProperty(ref _addSelectedButtonText, value);
        }

        /// <summary>
        /// Text for the third button (add all).
        /// </summary>
        public string AddAllButtonText
        {
            get => _addAllButtonText;
            set => SetProperty(ref _addAllButtonText, value);
        }

        /// <summary>
        /// Whether the third button (Add All) is visible.
        /// </summary>
        public bool AddAllButtonVisible
        {
            get => _addAllButtonVisible;
            set => SetProperty(ref _addAllButtonVisible, value);
        }

        /// <summary>
        /// Info text displayed between the buttons and the ListView.
        /// </summary>
        public string? InfoText
        {
            get => _infoText;
            set => SetProperty(ref _infoText, value);
        }

        /// <summary>
        /// Whether the info label is visible.
        /// </summary>
        public bool InfoLabelVisible
        {
            get => _infoLabelVisible;
            set => SetProperty(ref _infoLabelVisible, value);
        }

        /// <summary>
        /// Status text displayed below the ListView.
        /// </summary>
        public string? StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        /// <summary>
        /// Error text displayed below the status.
        /// </summary>
        public string? ErrorText
        {
            get => _errorText;
            set => SetProperty(ref _errorText, value);
        }

        /// <summary>
        /// Whether a loading operation is in progress.
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        /// <summary>
        /// The currently selected item in the ListView.
        /// </summary>
        public DatasourceObjectItemViewModel? SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        /// <summary>
        /// Column definitions for the ListView.
        /// </summary>
        public List<DatasourceObjectColumnDefinition> Columns { get; set; }

        /// <summary>
        /// Items displayed in the ListView.
        /// </summary>
        public ObservableCollection<DatasourceObjectItemViewModel> Items { get; }

        /// <summary>
        /// Command for the Load button.
        /// </summary>
        public ICommand? LoadCommand { get; set; }

        /// <summary>
        /// Command for the Add Selected button.
        /// </summary>
        public ICommand? AddSelectedCommand { get; set; }

        /// <summary>
        /// Command for the Add All button.
        /// </summary>
        public ICommand? AddAllCommand { get; set; }
    }
}
