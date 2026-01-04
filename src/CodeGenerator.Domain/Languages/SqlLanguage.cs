namespace CodeGenerator.Domain.Languages;

/// <summary>
/// SQL language
/// </summary>
public class SqlLanguage : ProgrammingLanguage
{
    public override string Name => "SQL";
    public override string FileExtension => ".sql";
    public override string SingleLineCommentPrefix => "--";
    public override string MultiLineCommentStart => "/*";
    public override string MultiLineCommentEnd => "*/";
    public override bool IsCaseSensitive => false;

    private static readonly string[] _keywords = new[]
    {
        "SELECT", "FROM", "WHERE", "INSERT", "UPDATE", "DELETE", "CREATE", "ALTER", "DROP",
        "TABLE", "INDEX", "VIEW", "PROCEDURE", "FUNCTION", "TRIGGER", "DATABASE", "SCHEMA",
        "PRIMARY", "KEY", "FOREIGN", "REFERENCES", "UNIQUE", "NOT", "NULL", "DEFAULT",
        "CHECK", "CONSTRAINT", "AND", "OR", "IN", "BETWEEN", "LIKE", "IS", "JOIN", "LEFT",
        "RIGHT", "INNER", "OUTER", "FULL", "CROSS", "ON", "AS", "ORDER", "BY", "ASC", "DESC",
        "GROUP", "HAVING", "UNION", "ALL", "DISTINCT", "TOP", "LIMIT", "OFFSET", "CASE",
        "WHEN", "THEN", "ELSE", "END", "BEGIN", "COMMIT", "ROLLBACK", "TRANSACTION", "IF",
        "WHILE", "DECLARE", "SET", "EXEC", "EXECUTE", "RETURN", "GO", "USE", "GRANT", "REVOKE"
    };

    public override IReadOnlyList<string> Keywords => _keywords;

    /// <summary>
    /// Singleton instance
    /// </summary>
    public static SqlLanguage Instance { get; } = new();
}
