using ProjectXYZ.Shared.ViewModels;

namespace ProjectXYZ.UserControls.ViewModels
{
    public class FolderFieldModel : FieldViewModelBase
    {
        private string? _description;

        /// <summary>
        /// Gets or sets the description shown in the folder browser dialog
        /// </summary>
        public string? Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }
    }
}
