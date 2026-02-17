namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Variance modifiers for generic type parameters
    /// </summary>
    public enum GenericVariance
    {
        Invariant,
        Covariant,      // out
        Contravariant   // in
    }
}
