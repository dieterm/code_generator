
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

    /// <summary>
    /// Compares if this package version is higher than the existing package version
    /// eg. "8.0.11" is higher than "8.0.10"
    /// </summary>
    public bool IsHigherVersionThan(NuGetPackage existingPackage)
    {
        if (existingPackage == null) throw new ArgumentNullException(nameof(existingPackage));
        if(Version == null) 
            if(existingPackage.Version== null) 
                return false; // beide null dus gelijk
            else 
                return false; // deze is null, dus niet hoger
        else if(existingPackage.Version == null) 
            return true; // deze is niet null, dus wel hoger

        return IsVersionGreater(this.Version, existingPackage.Version);
    }
    public bool IsHigherVersionThan(string? version)
    {
        if (version == null) return false;
        return IsVersionGreater(this.Version, version);
    }
    /*
     * IsVersionGreater("8.11.1", "8.1.1");   // true
     * IsVersionGreater("8.0.10", "8.0.2");   // true
     * IsVersionGreater("8.1", "8.1.0");      // false
     * IsVersionGreater("7.9.9", "8.0.0");    // false
     */
    public static bool IsVersionGreater(string versionA, string versionB)
    {
        if (versionA == null) throw new ArgumentNullException(nameof(versionA));
        if (versionB == null) throw new ArgumentNullException(nameof(versionB));

        var partsA = versionA.Split('.');
        var partsB = versionB.Split('.');

        int maxLength = Math.Max(partsA.Length, partsB.Length);

        for (int i = 0; i < maxLength; i++)
        {
            int a = i < partsA.Length ? int.Parse(partsA[i]) : 0;
            int b = i < partsB.Length ? int.Parse(partsB[i]) : 0;

            if (a > b) return true;
            if (a < b) return false;
        }

        // Volledig gelijk
        return false;
    }

}
