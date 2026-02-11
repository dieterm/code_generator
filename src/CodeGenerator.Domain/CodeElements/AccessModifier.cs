namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Access modifiers for code elements
    /// </summary>
    public enum AccessModifier
    {
        Public,
        Private,
        Protected,
        Internal,
        ProtectedInternal,
        PrivateProtected,
        /// <summary>For languages that don't have explicit access modifiers or default access</summary>
        Default
    }
}
