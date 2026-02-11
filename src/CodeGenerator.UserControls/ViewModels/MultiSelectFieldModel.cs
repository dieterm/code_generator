using CodeGenerator.Shared.ViewModels;
using System.Diagnostics;

namespace CodeGenerator.UserControls.ViewModels
{
    /// <summary>
    /// ViewModel for a multi-select field that allows selecting multiple items from a list.
    /// Supports any object type via ComboboxItem, with helper methods for Flag enums.
    /// </summary>
    public class MultiSelectFieldModel : FieldViewModelBase
    {
        private IList<ComboboxItem> _items = new List<ComboboxItem>();
        private IList<ComboboxItem> _selectedItems = new List<ComboboxItem>();
        private bool _isChangingValues;

        /// <summary>
        /// Gets or sets the list of available items for selection
        /// </summary>
        public IList<ComboboxItem> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        /// <summary>
        /// Gets or sets the list of currently selected items
        /// </summary>
        public IList<ComboboxItem> SelectedItems
        {
            get => _selectedItems;
            set
            {
                if (SetProperty(ref _selectedItems, value))
                {
                    if (_isChangingValues) return;
                    _isChangingValues = true;
                    Value = value;
                    _isChangingValues = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets the value. Expects and returns a List&lt;ComboboxItem&gt;.
        /// </summary>
        public override object Value
        {
            get => GetValue<object>();
            set
            {
                if (SetValue(value))
                {
                    if (_isChangingValues) return;
                    _isChangingValues = true;

                    if (value is IList<ComboboxItem> selectedList)
                    {
                        _selectedItems = new List<ComboboxItem>(selectedList);
                        OnPropertyChanged(nameof(SelectedItems));
                    }
                    else if (value == null)
                    {
                        _selectedItems = new List<ComboboxItem>();
                        OnPropertyChanged(nameof(SelectedItems));
                    }

                    _isChangingValues = false;
                }
            }
        }

        /// <summary>
        /// Populates Items from a Flags enum type and sets the selected items
        /// based on the provided enum value.
        /// </summary>
        /// <typeparam name="TEnum">A Flags enum type</typeparam>
        /// <param name="currentValue">The current combined enum value</param>
        public void LoadFromFlagsEnum<TEnum>(TEnum currentValue) where TEnum : struct, Enum
        {
            var items = new List<ComboboxItem>();
            foreach (var enumValue in Enum.GetValues<TEnum>())
            {
                // Skip the 'None' / zero value if present
                var intVal = Convert.ToInt64(enumValue);
                if (intVal == 0) continue;

                items.Add(new ComboboxItem
                {
                    DisplayName = enumValue.ToString(),
                    Value = enumValue
                });
            }
            Items = items;

            // Select items that are set in the current flags value
            var selected = new List<ComboboxItem>();
            var currentLong = Convert.ToInt64(currentValue);
            foreach (var item in Items)
            {
                if (item.Value is TEnum enumVal)
                {
                    var itemLong = Convert.ToInt64(enumVal);
                    if (itemLong != 0 && (currentLong & itemLong) == itemLong)
                    {
                        selected.Add(item);
                    }
                }
            }
            SelectedItems = selected;
        }

        /// <summary>
        /// Gets the combined Flags enum value from the currently selected items.
        /// </summary>
        /// <typeparam name="TEnum">A Flags enum type</typeparam>
        /// <returns>The combined enum value</returns>
        public TEnum GetFlagsEnumValue<TEnum>() where TEnum : struct, Enum
        {
            long combined = 0;
            foreach (var item in SelectedItems)
            {
                if (item.Value is TEnum enumVal)
                {
                    combined |= Convert.ToInt64(enumVal);
                }
            }
            return (TEnum)Enum.ToObject(typeof(TEnum), combined);
        }

        /// <summary>
        /// Sets the selected items based on a combined Flags enum value,
        /// without reloading the Items list.
        /// </summary>
        /// <typeparam name="TEnum">A Flags enum type</typeparam>
        /// <param name="value">The combined enum value</param>
        public void SetFlagsEnumValue<TEnum>(TEnum value) where TEnum : struct, Enum
        {
            var currentLong = Convert.ToInt64(value);
            var selected = new List<ComboboxItem>();
            foreach (var item in Items)
            {
                if (item.Value is TEnum enumVal)
                {
                    var itemLong = Convert.ToInt64(enumVal);
                    if (itemLong != 0 && (currentLong & itemLong) == itemLong)
                    {
                        selected.Add(item);
                    }
                }
            }
            SelectedItems = selected;
        }
    }
}
