namespace CodeGenerator.Core.Events;

/// <summary>
/// NuGet package information
/// </summary>
public class NuGetPackageInfo
{
    /// <summary>
    /// eg. "Microsoft.EntityFrameworkCore.Relational"
    /// </summary>
    public string PackageId { get; set; } = string.Empty;
    /// <summary>
    /// eg. "8.0.11"
    /// </summary>
    public string? Version { get; set; }
}
