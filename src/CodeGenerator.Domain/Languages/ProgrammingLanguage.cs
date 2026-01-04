namespace CodeGenerator.Domain.Languages;

/// <summary>
/// Base class for programming languages
/// </summary>
public abstract class ProgrammingLanguage
{
    /// <summary>
    /// Language name
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// File extension for source files (including dot)
    /// </summary>
    public abstract string FileExtension { get; }

    /// <summary>
    /// Single-line comment prefix
    /// </summary>
    public abstract string SingleLineCommentPrefix { get; }

    /// <summary>
    /// Multi-line comment start
    /// </summary>
    public abstract string MultiLineCommentStart { get; }

    /// <summary>
    /// Multi-line comment end
    /// </summary>
    public abstract string MultiLineCommentEnd { get; }

    /// <summary>
    /// Whether the language is case-sensitive
    /// </summary>
    public virtual bool IsCaseSensitive => true;

    /// <summary>
    /// Reserved keywords in the language
    /// </summary>
    public abstract IReadOnlyList<string> Keywords { get; }
}
