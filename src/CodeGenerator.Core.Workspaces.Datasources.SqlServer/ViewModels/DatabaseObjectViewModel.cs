using CodeGenerator.Core.Workspaces.Datasources.SqlServer.Services;
using CodeGenerator.Shared.ViewModels;

namespace CodeGenerator.Core.Workspaces.Datasources.SqlServer.ViewModels;

/// <summary>
/// ViewModel for a database object in the list
/// </summary>
public class DatabaseObjectViewModel : ViewModelBase
{
    public string Name { get; set; } = string.Empty;
    public string Schema { get; set; } = string.Empty;
    public DatabaseObjectType ObjectType { get; set; }
    public string DisplayName { get; set; } = string.Empty;

    public string TypeIcon => ObjectType == DatabaseObjectType.Table ? "table" : "eye";
}
