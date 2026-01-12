namespace CodeGenerator.UserControls.ViewModels
{
    /// <summary>
    /// Represents an item in a combobox
    /// </summary>
    public class ComboboxItem
    {
        /// <summary>
        /// Display name shown in the combobox
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Value of the item
        /// </summary>
        public object? Value { get; set; }

        public override string ToString() => DisplayName;
    }
}
