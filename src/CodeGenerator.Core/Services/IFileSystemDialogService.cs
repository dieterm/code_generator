namespace CodeGenerator.Application.Services;

/// <summary>
/// Service for file system dialogs (open file, save file, select folder)
/// </summary>
public interface IFileSystemDialogService
{
    /// <summary>
    /// Show an open file dialog
    /// </summary>
    /// <param name="filter">File filter (e.g., "Text files (*.txt)|*.txt|All files (*.*)|*.*")</param>
    /// <param name="initialDirectory">Initial directory to show</param>
    /// <returns>Selected file path, or null if cancelled</returns>
    string? OpenFile(string filter, string? initialDirectory = null);

    /// <summary>
    /// Show an open file dialog that allows selecting multiple files
    /// </summary>
    /// <param name="filter">File filter (e.g., "Text files (*.txt)|*.txt|All files (*.*)|*.*")</param>
    /// <param name="initialDirectory">Initial directory to show</param>
    /// <returns>Selected file paths, or empty array if cancelled</returns>
    string[] OpenFiles(string filter, string? initialDirectory = null);

    /// <summary>
    /// Show a save file dialog
    /// </summary>
    /// <param name="filter">File filter (e.g., "Text files (*.txt)|*.txt|All files (*.*)|*.*")</param>
    /// <param name="initialDirectory">Initial directory to show</param>
    /// <param name="defaultFileName">Default file name</param>
    /// <returns>Selected file path, or null if cancelled</returns>
    string? SaveFile(string filter, string? initialDirectory = null, string? defaultFileName = null);

    /// <summary>
    /// Show a folder browser dialog
    /// </summary>
    /// <param name="description">Description shown in the dialog</param>
    /// <param name="initialDirectory">Initial directory to show</param>
    /// <returns>Selected folder path, or null if cancelled</returns>
    string? SelectFolder(string description, string? initialDirectory = null);
}
