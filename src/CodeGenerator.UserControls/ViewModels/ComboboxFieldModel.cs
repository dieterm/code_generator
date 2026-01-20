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
            set {
                if (SetProperty(ref _selectedItem, value)) { 
                    if(_isChangingValues)
                        return;
                    _isChangingValues = true;

                    if (value == null)
                    {
                        Value = null;
                        Tooltip = string.Empty;
                    }
                    else
                    {
                        Value = value.Value;
                        Tooltip = value.Tooltip;
                    }

                    _isChangingValues = false;
                }
            }
        }
        bool _isChangingValues = false;
        public override object Value
        {
            get => GetValue<object>();
            set {
                if (SetValue(value))
                {
                    if (_isChangingValues)
                        return;
                    _isChangingValues = true;
                    
                    // Update SelectedItem based on Value
                    if (value != null && Items?.Count>0)
                    {
                        foreach (var item in Items)
                        {
                            if (item.Value != null && item.Value.Equals(value))
                            {
                                SelectedItem = item;
                                return;
                            }
                        }
                        throw new InvalidOperationException($"Value {value} not found in Items collection.");
                    }
                    else
                    {
                        SelectedItem = null; // Value is null
                    }
                    _isChangingValues = false;
                }
            }
        }

        private string _displayMember = nameof(ComboboxItem.DisplayName);
        public string DisplayMember
        {
            get { return _displayMember; }
            set { SetProperty(ref _displayMember, value); }
        }
    }
}
