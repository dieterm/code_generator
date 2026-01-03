using ProjectXYZ.Shared.ViewModels;
using System.Collections;

namespace ProjectXYZ.UserControls.ViewModels
{
    public class ComboboxFieldModel : FieldViewModelBase
    {
        private ICollection _items = new List<object>();
        /// <summary>
        /// Gets or sets the list of options for the combobox
        /// </summary>
        public ICollection Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        public virtual bool SetItems(ICollection items)
        {
            return SetProperty(ref _items, items, nameof(Items));
        }
    }
}
