using CodeGenerator.Application.Services;
using System.Windows.Forms;

namespace CodeGenerator.Presentation.WinForms.Services;

/// <summary>
/// WinForms implementation of IFileSystemDialogService
/// </summary>
public class FileSystemDialogService : IFileSystemDialogService
{
    public string? OpenFile(string filter, string? initialDirectory = null)
    {
        using var dialog = new OpenFileDialog
        {
            Filter = filter,
            InitialDirectory = initialDirectory ?? string.Empty,
            RestoreDirectory = true
        };

        return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
    }

    public string? SaveFile(string filter, string? initialDirectory = null, string? defaultFileName = null)
    {
        using var dialog = new SaveFileDialog
        {
            Filter = filter,
            InitialDirectory = initialDirectory ?? string.Empty,
            FileName = defaultFileName ?? string.Empty,
            RestoreDirectory = true
        };

        return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
    }

    public string? SelectFolder(string description, string? initialDirectory = null)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = description,
            SelectedPath = initialDirectory ?? string.Empty,
            ShowNewFolderButton = true
        };

        return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : null;
    }
}
