namespace CodeGenerator.Domain.Artifacts.DotNet;

/// <summary>
/// Represents a NuGet package reference
/// </summary>
public class NugetPackageReference
{
    /// <summary>
    /// Package identifier
    /// </summary>
    public string PackageId { get; set; } = string.Empty;

    /// <summary>
    /// Package version (null for latest)
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// Include assets from the package
    /// </summary>
    public bool IncludeAssets { get; set; } = true;

    /// <summary>
    /// Mark assets as private (not flowing to dependents)
    /// </summary>
    public bool PrivateAssets { get; set; } = false;

    /// <summary>
    /// Specific assets to include
    /// </summary>
    public string? IncludeAssetsValue { get; set; }

    /// <summary>
    /// Specific assets to exclude
    /// </summary>
    public string? ExcludeAssetsValue { get; set; }

    public override string ToString() => $"{PackageId} {Version ?? "(latest)"}";
}
