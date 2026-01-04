namespace CodeGenerator.Domain.Languages;

/// <summary>
/// TypeScript programming language
/// </summary>
public class TypeScriptLanguage : ProgrammingLanguage
{
    public override string Name => "TypeScript";
    public override string FileExtension => ".ts";
    public override string SingleLineCommentPrefix => "//";
    public override string MultiLineCommentStart => "/*";
    public override string MultiLineCommentEnd => "*/";

    private static readonly string[] _keywords = new[]
    {
        "break", "case", "catch", "class", "const", "continue", "debugger", "default", "delete",
        "do", "else", "enum", "export", "extends", "false", "finally", "for", "function", "if",
        "import", "in", "instanceof", "new", "null", "return", "super", "switch", "this", "throw",
        "true", "try", "typeof", "var", "void", "while", "with", "as", "implements", "interface",
        "let", "package", "private", "protected", "public", "static", "yield", "any", "boolean",
        "constructor", "declare", "get", "module", "require", "number", "set", "string", "symbol",
        "type", "from", "of", "namespace", "async", "await", "readonly", "keyof", "infer", "never",
        "unknown", "abstract", "override"
    };

    public override IReadOnlyList<string> Keywords => _keywords;

    /// <summary>
    /// Singleton instance
    /// </summary>
    public static TypeScriptLanguage Instance { get; } = new();
}
