namespace CodeGenerator.Core.Events;

/// <summary>
/// Information about a solution being created
/// </summary>
public class SolutionInfo
{
    public string SolutionName { get; set; } = string.Empty;
    public string SolutionPath { get; set; } = string.Empty;
    public string TargetFramework { get; set; } = "net8.0";
}
