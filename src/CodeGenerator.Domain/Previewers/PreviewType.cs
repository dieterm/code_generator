namespace CodeGenerator.Domain.Previewers;

/// <summary>
/// Types of previews that can be generated
/// </summary>
public enum PreviewType
{
    /// <summary>
    /// Image preview (for image files)
    /// </summary>
    Image,

    /// <summary>
    /// Text/code preview
    /// </summary>
    Text,

    /// <summary>
    /// WinForms UserControl preview
    /// </summary>
    UserControl,

    /// <summary>
    /// Folder/file tree structure preview
    /// </summary>
    FolderStructure,

    /// <summary>
    /// Project structure preview
    /// </summary>
    Project,

    /// <summary>
    /// Solution structure preview
    /// </summary>
    Solution
}
