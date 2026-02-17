namespace CodeGenerator.Domain.CodeElements
{
    /// <summary>
    /// Special generic constraint kinds
    /// </summary>
    [Flags]
    public enum GenericConstraintKind
    {
        None = 0,
        Class = 1 << 0,         // where T : class
        Struct = 1 << 1,        // where T : struct
        New = 1 << 2,           // where T : new()
        NotNull = 1 << 3,       // where T : notnull
        Unmanaged = 1 << 4,     // where T : unmanaged
        Default = 1 << 5        // where T : default (C# 9)
    }
}
