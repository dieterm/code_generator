using CodeGenerator.Shared.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.UserControls.ViewModels
{
    public class StringListFieldModel : FieldViewModelBase
    {
        public BindingList<string> Items { get; } = new();

        public override object Value
        {
            get => GetValue<object>();
            set
            {
                if (SetValue(value))
                    LoadItems(value as IEnumerable<string>);
            }
        }

        private void LoadItems(IEnumerable<string>? source)
        {
            Items.RaiseListChangedEvents = false;
            Items.Clear();
            if (source != null)
            {
                foreach (var item in source)
                    Items.Add(item);
            }
            Items.RaiseListChangedEvents = true;
            Items.ResetBindings();
        }

        public void AddItem(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            Items.Add(value);
            SyncValueFromItems();
        }

        public void UpdateItem(int index, string newValue)
        {
            if (index < 0 || index >= Items.Count || string.IsNullOrWhiteSpace(newValue)) return;
            Items[index] = newValue;
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
            SetValue<object>(Items.ToList());
        }
    }
}
