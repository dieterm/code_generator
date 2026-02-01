namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Base class for all code elements that can be used to build source code structures.
    /// </summary>
    public abstract class CodeElement
    {
        public string? RawCode { get; set; }

        /// <summary>
        /// Optional name of the code element
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Documentation/comments for this code element
        /// </summary>
        public string? Documentation { get; set; }

        /// <summary>
        /// Additional attributes/annotations applied to this element
        /// </summary>
        public List<AttributeElement> Attributes { get; set; } = new();

        /// <summary>
        /// Access modifier for this element
        /// </summary>
        public AccessModifier AccessModifier { get; set; } = AccessModifier.Public;

        /// <summary>
        /// Additional modifiers (static, abstract, virtual, etc.)
        /// </summary>
        public ElementModifiers Modifiers { get; set; } = ElementModifiers.None;
    }

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

    /// <summary>
    /// Additional modifiers that can be applied to code elements
    /// </summary>
    [Flags]
    public enum ElementModifiers
    {
        None = 0,
        Static = 1 << 0,
        Abstract = 1 << 1,
        Virtual = 1 << 2,
        Override = 1 << 3,
        Sealed = 1 << 4,
        Readonly = 1 << 5,
        Const = 1 << 6,
        Async = 1 << 7,
        Partial = 1 << 8,
        Extern = 1 << 9,
        Volatile = 1 << 10,
        New = 1 << 11,
        Required = 1 << 12
    }
}
