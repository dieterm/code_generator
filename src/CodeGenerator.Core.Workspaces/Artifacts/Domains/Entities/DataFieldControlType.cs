namespace CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities
{
    /// <summary>
    /// Available data field control types for edit views
    /// </summary>
    public enum DataFieldControlType
    {
        /// <summary>Single line text input</summary>
        SingleLineTextField,
        /// <summary>Multi-line text input</summary>
        MultiLineTextField,
        /// <summary>Integer number input</summary>
        IntegerField,
        /// <summary>Decimal number input</summary>
        DecimalField,
        /// <summary>Boolean checkbox</summary>
        BooleanField,
        /// <summary>Date picker</summary>
        DateField,
        /// <summary>Date and time picker</summary>
        DateTimeField,
        /// <summary>Time picker</summary>
        TimeField,
        /// <summary>Combobox/dropdown selection</summary>
        ComboboxField,
        /// <summary>Radio button group</summary>
        RadioButtonField,
        /// <summary>File upload/picker</summary>
        FileField,
        /// <summary>Image upload/display</summary>
        ImageField,
        /// <summary>Rich text editor</summary>
        RichTextField,
        /// <summary>Color picker</summary>
        ColorField,
        /// <summary>Password input</summary>
        PasswordField,
        /// <summary>Email input with validation</summary>
        EmailField,
        /// <summary>Phone number input</summary>
        PhoneField,
        /// <summary>URL input with validation</summary>
        UrlField
    }
}
