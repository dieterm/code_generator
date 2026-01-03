using ProjectXYZ.Shared.ViewModels;

namespace ProjectXYZ.UserControls.ViewModels
{
    public enum FileSelectionMode
    {
        Open,
        Save
    }

    public class FileFieldModel : FieldViewModelBase
    {
        private FileSelectionMode _selectionMode = FileSelectionMode.Open;
        private string? _filter;
        private string? _defaultExtension;

        /// <summary>
        /// Gets or sets the file selection mode (Open or Save)
        /// </summary>
        public FileSelectionMode SelectionMode
        {
            get => _selectionMode;
            set => SetProperty(ref _selectionMode, value);
        }

        /// <summary>
        /// Gets or sets the file filter (e.g., "Text files (*.txt)|*.txt|All files (*.*)|*.*")
        /// </summary>
        public string? Filter
        {
            get => _filter;
            set => SetProperty(ref _filter, value);
        }

        /// <summary>
        /// Gets or sets the default file extension
        /// </summary>
        public string? DefaultExtension
        {
            get => _defaultExtension;
            set => SetProperty(ref _defaultExtension, value);
        }
    }
}
