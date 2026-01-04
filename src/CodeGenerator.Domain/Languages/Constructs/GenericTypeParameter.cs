namespace CodeGenerator.Domain.Languages.Constructs;

/// <summary>
/// Represents a generic type parameter
/// </summary>
public class GenericTypeParameter
{
    /// <summary>
    /// Name of the type parameter (e.g., "T", "TKey")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Constraints on the type parameter
    /// </summary>
    public List<string> Constraints { get; } = new();

    /// <summary>
    /// Variance (in/out for interfaces)
    /// </summary>
    public GenericVariance Variance { get; set; } = GenericVariance.None;
}

/// <summary>
/// Generic type parameter variance
/// </summary>
public enum GenericVariance
{
    None,
    In,  // contravariant
    Out  // covariant
}
