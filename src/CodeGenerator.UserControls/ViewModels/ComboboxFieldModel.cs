using CodeGenerator.Shared.ViewModels;
using System.Collections;

namespace CodeGenerator.UserControls.ViewModels
{
    public class ComboboxFieldModel : FieldViewModelBase
    {
        private ICollection<ComboboxItem> _items = new List<ComboboxItem>();
        /// <summary>
        /// Gets or sets the list of options for the combobox
        /// </summary>
        public ICollection<ComboboxItem> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        private ComboboxItem? _selectedItem;
        public ComboboxItem? SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value); }
        }

        public virtual object Value
        {
            get => GetValue<object>();
            set {
                if (SetValue(value))
                {
                    // Update SelectedItem based on Value
                    if (value != null)
                    {
                        foreach (var item in Items)
                        {
                            if (item.Value != null && item.Value.Equals(value))
                            {
                                SelectedItem = item;
                                return;
                            }
                        }
                        SelectedItem = null; // No matching item found
                    }
                    else
                    {
                        SelectedItem = null; // Value is null
                    }
                }
            }
        }

        private string _displayMember = nameof(ComboboxItem.DisplayName);
        public string DisplayMember
        {
            get { return _displayMember; }
            set { SetProperty(ref _displayMember, value); }
        }


        //public virtual bool SetItems(ICollection<ComboboxItem> items)
        //{
        //    return SetProperty(ref _items, items, nameof(Items));
        //}
    }
}
