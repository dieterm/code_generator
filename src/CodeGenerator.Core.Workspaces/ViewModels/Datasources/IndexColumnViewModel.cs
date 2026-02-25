using CodeGenerator.Shared.ViewModels;

namespace CodeGenerator.Core.Workspaces.ViewModels.Datasources
{
    /// <summary>
    /// ViewModel for a column in the index editor
    /// </summary>
    public class IndexColumnViewModel : ViewModelBase
    {
        public string ColumnName { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;

        public string DisplayText => $"{ColumnName} ({DataType})";
    }
}
