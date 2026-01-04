namespace CodeGenerator.Domain.Languages;

/// <summary>
/// C# programming language
/// </summary>
public class CSharpLanguage : ProgrammingLanguage
{
    public override string Name => "C#";
    public override string FileExtension => ".cs";
    public override string SingleLineCommentPrefix => "//";
    public override string MultiLineCommentStart => "/*";
    public override string MultiLineCommentEnd => "*/";

    private static readonly string[] _keywords = new[]
    {
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked",
        "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else",
        "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for",
        "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock",
        "long", "namespace", "new", "null", "object", "operator", "out", "override", "params",
        "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short",
        "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true",
        "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual",
        "void", "volatile", "while", "var", "dynamic", "async", "await", "nameof", "when",
        "record", "init", "required", "file", "scoped"
    };

    public override IReadOnlyList<string> Keywords => _keywords;

    /// <summary>
    /// Singleton instance
    /// </summary>
    public static CSharpLanguage Instance { get; } = new();
}
