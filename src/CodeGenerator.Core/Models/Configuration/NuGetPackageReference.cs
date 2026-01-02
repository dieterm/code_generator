using CodeGenerator.Core.Enums;

namespace CodeGenerator.Core.Models.Configuration;

/// <summary>
/// NuGet package reference
/// </summary>
public class NuGetPackageReference
{
    public string PackageId { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public ArchitectureLayer[] Layers { get; set; } = Array.Empty<ArchitectureLayer>();
    public bool IncludePrerelease { get; set; } = false;
}