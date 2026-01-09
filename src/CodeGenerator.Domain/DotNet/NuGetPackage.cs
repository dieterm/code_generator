namespace CodeGenerator.Domain.DotNet;

/// <summary>
/// NuGet package information
/// </summary>
public class NuGetPackage
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
