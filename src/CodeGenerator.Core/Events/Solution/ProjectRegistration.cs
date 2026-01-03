using CodeGenerator.Core.Enums;

namespace CodeGenerator.Core.Events;

/// <summary>
/// Information about a project being registered/created
/// </summary>
public class ProjectRegistration
{
    public ArchitectureLayer? Layer { get; set; }
    /// <summary>
    /// eg. "Geoservice.Application"
    /// </summary>
    public string ProjectName { get; set; } = string.Empty;
    /// <summary>
    /// Full path of project folder
    /// </summary>
    public string ProjectPath { get; set; } = string.Empty;
    /// <summary>
    /// eg. "classlib", "winformslib"
    /// </summary>
    public string ProjectType { get; set; } = "classlib";
    /// <summary>
    /// eg. "net8.0"
    /// </summary>
    public string TargetFramework { get; set; } = "net8.0";
    public List<string> ProjectReferences { get; set; } = new();
    public List<NuGetPackageInfo> NuGetPackages { get; set; } = new();
    public string RegisteredBy { get; set; } = string.Empty;
}
