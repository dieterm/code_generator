using CodeGenerator.Shared.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.UserControls.ViewModels
{
    public class StringDictionaryFieldModel : FieldViewModelBase
    {
        public BindingList<StringDictionaryEntry> Items { get; } = new();

        public override object Value
        {
            get => GetValue<object>();
            set
            {
                if (SetValue(value))
                    LoadItems(value as IDictionary<string, string>);
            }
        }

        private void LoadItems(IDictionary<string, string>? source)
        {
            Items.RaiseListChangedEvents = false;
            Items.Clear();
            if (source != null)
            {
                foreach (var kvp in source)
                    Items.Add(new StringDictionaryEntry { Key = kvp.Key, Value = kvp.Value });
            }
            Items.RaiseListChangedEvents = true;
            Items.ResetBindings();
        }

        public void AddItem(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key)) return;
            var existing = Items.FirstOrDefault(i => i.Key == key);
            if (existing != null)
                existing.Value = value;
            else
                Items.Add(new StringDictionaryEntry { Key = key, Value = value });
            SyncValueFromItems();
        }

        public void UpdateItem(int index, string key, string value)
        {
            if (index < 0 || index >= Items.Count || string.IsNullOrWhiteSpace(key)) return;
            Items[index] = new StringDictionaryEntry { Key = key, Value = value };
            SyncValueFromItems();
        }

        public void RemoveItem(int index)
        {
            if (index < 0 || index >= Items.Count) return;
            Items.RemoveAt(index);
            SyncValueFromItems();
        }

        private void SyncValueFromItems()
        {
            var dict = new Dictionary<string, string>();
            foreach (var item in Items)
                dict[item.Key] = item.Value;
            SetValue<object>(dict);
        }
    }

    public class StringDictionaryEntry
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public override string ToString() => $"{Key} = {Value}";
    }
}
