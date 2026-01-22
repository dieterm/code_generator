namespace CodeGenerator.Core.Workspaces.Datasources.Excel.ViewModels;

/// <summary>
/// Event args for adding a database object
/// </summary>
public class AddSheetEventArgs : EventArgs
{
    public object Sheet { get; }

    public AddSheetEventArgs(object sheet)
    {
        Sheet = sheet;
    }
}
