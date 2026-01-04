namespace CodeGenerator.Domain.Artifacts.DotNet;

/// <summary>
/// Represents a .NET framework version
/// </summary>
public class DotNetFrameworkVersion
{
    /// <summary>
    /// Version string (e.g., "net8.0", "netstandard2.1")
    /// </summary>
    public string VersionString { get; set; } = "net8.0";

    /// <summary>
    /// The framework type
    /// </summary>
    public DotNetFramework Framework { get; set; } = DotNetFramework.NET5Plus;

    /// <summary>
    /// Major version number
    /// </summary>
    public int MajorVersion { get; set; } = 8;

    /// <summary>
    /// Minor version number
    /// </summary>
    public int MinorVersion { get; set; } = 0;

    // Common framework versions
    public static DotNetFrameworkVersion Net6 => new() { VersionString = "net6.0", Framework = DotNetFramework.NET5Plus, MajorVersion = 6 };
    public static DotNetFrameworkVersion Net7 => new() { VersionString = "net7.0", Framework = DotNetFramework.NET5Plus, MajorVersion = 7 };
    public static DotNetFrameworkVersion Net8 => new() { VersionString = "net8.0", Framework = DotNetFramework.NET5Plus, MajorVersion = 8 };
    public static DotNetFrameworkVersion Net9 => new() { VersionString = "net9.0", Framework = DotNetFramework.NET5Plus, MajorVersion = 9 };
    public static DotNetFrameworkVersion Net10 => new() { VersionString = "net10.0", Framework = DotNetFramework.NET5Plus, MajorVersion = 10 };
    public static DotNetFrameworkVersion NetStandard20 => new() { VersionString = "netstandard2.0", Framework = DotNetFramework.NETStandard, MajorVersion = 2, MinorVersion = 0 };
    public static DotNetFrameworkVersion NetStandard21 => new() { VersionString = "netstandard2.1", Framework = DotNetFramework.NETStandard, MajorVersion = 2, MinorVersion = 1 };

    public override string ToString() => VersionString;
}
