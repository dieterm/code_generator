namespace CodeGenerator.Domain.CodeElements
{
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
