namespace CodeGenerator.Core.Workspaces.Datasources.Csv.ViewModels;

/// <summary>
/// Event args for property value changes
/// </summary>
public class PropertyValueChangedEventArgs : EventArgs
{
    public string PropertyName { get; }
    public object? Value { get; }

    public PropertyValueChangedEventArgs(string propertyName, object? value)
    {
        PropertyName = propertyName;
        Value = value;
    }
}
