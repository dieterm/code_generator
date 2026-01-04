namespace CodeGenerator.Domain.Artifacts.DotNet;

/// <summary>
/// .NET framework types
/// </summary>
public enum DotNetFramework
{
    /// <summary>
    /// .NET Framework (4.x and earlier)
    /// </summary>
    NETFramework,

    /// <summary>
    /// .NET Core (1.x - 3.x)
    /// </summary>
    NETCore,

    /// <summary>
    /// .NET 5 and later (unified platform)
    /// </summary>
    NET5Plus,

    /// <summary>
    /// .NET Standard (cross-platform library target)
    /// </summary>
    NETStandard
}
