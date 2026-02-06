namespace CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities
{
    /// <summary>
    /// Column type for data grid display
    /// </summary>
    public enum ListViewColumnType
    {
        /// <summary>Standard text column</summary>
        Text,
        /// <summary>Numeric column with formatting</summary>
        Numeric,
        /// <summary>Date column with formatting</summary>
        Date,
        /// <summary>DateTime column with formatting</summary>
        DateTime,
        /// <summary>Boolean column (checkbox)</summary>
        Boolean,
        /// <summary>Image column</summary>
        Image,
        /// <summary>Hyperlink column</summary>
        Hyperlink,
        /// <summary>Combobox column for inline editing</summary>
        Combobox,
        /// <summary>Template column for custom rendering</summary>
        Template
    }
}
