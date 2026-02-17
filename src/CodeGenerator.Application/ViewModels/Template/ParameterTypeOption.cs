namespace CodeGenerator.Application.ViewModels.Template;

/// <summary>
/// Represents a parameter type option for the dropdown
/// </summary>
public class ParameterTypeOption
{
    public string TypeName { get; }
    public string DisplayName { get; }

    public ParameterTypeOption(string typeName, string displayName)
    {
        TypeName = typeName;
        DisplayName = displayName;
    }

    public override string ToString() => DisplayName;
}
