using Scriban.Runtime;
using Syncfusion.Windows.Forms.Edit;
using Syncfusion.Windows.Forms.Edit.Enums;
using Syncfusion.Windows.Forms.Edit.Implementation.Config;
using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Utils.CodeSnippets;
using System.Collections;
using System.Drawing;
using System.Reflection;

namespace CodeGenerator.Presentation.WinForms.Views;

/// <summary>
/// Scriban template language configuration for Syncfusion EditControl
/// Provides syntax highlighting and IntelliSense support using Syncfusion's built-in classes
/// </summary>
public class ScribanLanguageConfig : ConfigLanguage
{
    private readonly Dictionary<string, object?> _parameters;
    private readonly Dictionary<string, Delegate> _methods;
    private readonly ScriptObject _globalFunctions;

    public ScribanLanguageConfig(
        Dictionary<string, object?> parameters,
        Dictionary<string, Delegate> methods,
        ScriptObject globalFunctions)
        : base("Scriban")
    {
        _parameters = parameters ?? new Dictionary<string, object?>();
        _methods = methods ?? new Dictionary<string, Delegate>();
        _globalFunctions = globalFunctions ?? new ScriptObject();

        // Set basic properties
        CaseInsensitive = false;
        Extensions.Add("scriban");

        InitializeFormats();
        InitializeLexems();
        InitializeContextChoices();
    }

    #region Format Initialization

    private void InitializeFormats()
    {
        // Create text formats for different token types
        CreateFormat("Text", Color.Black, Color.White, false, false);
        CreateFormat("Keyword", Color.Blue, Color.White, true, false);
        CreateFormat("Variable", Color.Purple, Color.White, false, false);
        CreateFormat("Parameter", Color.DarkCyan, Color.White, false, false);
        CreateFormat("Function", Color.DarkGreen, Color.White, false, false);
        CreateFormat("GlobalFunction", Color.Green, Color.White, false, false);
        CreateFormat("String", Color.Brown, Color.White, false, false);
        CreateFormat("Number", Color.DarkOrange, Color.White, false, false);
        CreateFormat("Comment", Color.Gray, Color.White, false, true);
        CreateFormat("Operator", Color.DarkBlue, Color.White, false, false);
        CreateFormat("Delimiter", Color.Red, Color.White, true, false);
        CreateFormat("Property", Color.Teal, Color.White, false, false);
    }

    private void CreateFormat(string name, Color foreColor, Color backColor, bool bold, bool italic)
    {
        var format = Add(name);
        format.ForeColor = foreColor;
        format.BackColor = backColor;
        if (bold || italic)
        {
            var style = FontStyle.Regular;
            if (bold) style |= FontStyle.Bold;
            if (italic) style |= FontStyle.Italic;
            format.Font = new Font("Consolas", 10, style);
        }
    }

    #endregion

    #region Lexem Initialization

    private void InitializeLexems()
    {
        // Add Scriban block delimiters as complex lexems
        AddScribanBlockLexem();

        // Add string lexems
        AddStringLexem("\"", "\"");
        AddStringLexem("'", "'");

        // Add comment lexem (Scriban uses ## for comments)
        AddCommentLexem();
    }

    private void AddScribanBlockLexem()
    {
        // Create a lexem for Scriban code blocks {{ ... }}
        var scribanLexem = new ConfigLexem("{{", "}}", FormatType.Custom, false);
        scribanLexem.FormatName = "Delimiter";

        // Add keywords as sub-lexems
        var keywords = new[]
        {
            "if", "else", "elseif", "end", "for", "in", "while", "break", "continue",
            "func", "ret", "capture", "readonly", "import", "with", "wrap", "include",
            "true", "false", "null", "empty", "blank", "this", "tablerow"
        };

        foreach (var keyword in keywords)
        {
            AddKeywordSubLexem(scribanLexem, keyword);
        }

        // Add operators
        var operators = new[] { "==", "!=", "<=", ">=", "&&", "||", "??" };
        foreach (var op in operators)
        {
            AddOperatorSubLexem(scribanLexem, op);
        }

        // Add parameters
        foreach (var param in _parameters)
        {
            AddIdentifierSubLexem(scribanLexem, param.Key, "Parameter");
        }

        // Add custom methods
        foreach (var method in _methods)
        {
            AddIdentifierSubLexem(scribanLexem, method.Key, "Function");
        }

        // Add global functions
        foreach (var func in _globalFunctions)
        {
            AddIdentifierSubLexem(scribanLexem, func.Key, "GlobalFunction");
        }

        // Add built-in functions
        var builtinFunctions = new[]
        {
            "size", "first", "last", "join", "split", "reverse", "sort", "uniq", "map", "contains",
            "upcase", "downcase", "capitalize", "strip", "lstrip", "rstrip", "slice", "truncate",
            "replace", "append", "prepend", "remove", "remove_first", "strip_html", "escape",
            "date", "math", "string", "array", "object", "regex", "timespan", "html"
        };
        foreach (var func in builtinFunctions)
        {
            if (!_globalFunctions.ContainsKey(func))
            {
                AddIdentifierSubLexem(scribanLexem, func, "Function");
            }
        }

        Lexems.Add(scribanLexem);

        // Also add the raw block delimiters {%- ... -%}
        var rawLexem = new ConfigLexem("{%-", "-%}", FormatType.Custom, false);
        rawLexem.FormatName = "Delimiter";
        Lexems.Add(rawLexem);
    }

    private void AddKeywordSubLexem(ConfigLexem parent, string keyword)
    {
        // Use keyword as both begin and end for single-word matching
        var subLexem = new ConfigLexem(keyword, "", FormatType.Custom, true);
        subLexem.FormatName = "Keyword";
        parent.SubLexems.Add(subLexem);
    }

    private void AddOperatorSubLexem(ConfigLexem parent, string op)
    {
        var subLexem = new ConfigLexem(op, "", FormatType.Custom, true);
        subLexem.FormatName = "Operator";
        parent.SubLexems.Add(subLexem);
    }

    private void AddIdentifierSubLexem(ConfigLexem parent, string identifier, string formatName)
    {
        var subLexem = new ConfigLexem(identifier, "", FormatType.Custom, true);
        subLexem.FormatName = formatName;
        parent.SubLexems.Add(subLexem);
    }

    private void AddStringLexem(string begin, string end)
    {
        var stringLexem = new ConfigLexem(begin, end, FormatType.String, false);
        stringLexem.FormatName = "String";
        Lexems.Add(stringLexem);
    }

    private void AddCommentLexem()
    {
        // Scriban uses ## for line comments inside blocks
        var commentLexem = new ConfigLexem("##", "\n", FormatType.Comment, false);
        commentLexem.FormatName = "Comment";
        Lexems.Add(commentLexem);
    }

    #endregion

    #region Context Choices (IntelliSense)

    private void InitializeContextChoices()
    {
        // Add code snippets for IntelliSense/auto-complete
        AddScribanSnippets();
        AddParameterSnippets();
        AddFunctionSnippets();
    }

    private void AddScribanSnippets()
    {
        // Control flow snippets
        AddSnippet("if", "{{ if $condition$ }}\n\t$cursor$\n{{ end }}");
        AddSnippet("ifelse", "{{ if $condition$ }}\n\t$cursor$\n{{ else }}\n\t\n{{ end }}");
        AddSnippet("for", "{{ for $item$ in $collection$ }}\n\t{{ $item$ }}\n{{ end }}");
        AddSnippet("forindex", "{{ for $item$ in $collection$ }}\n\t{{ for.index }}: {{ $item$ }}\n{{ end }}");
        AddSnippet("capture", "{{ capture $variable$ }}\n\t$cursor$\n{{ end }}");
        AddSnippet("include", "{{ include '$template_name$' }}");
        AddSnippet("func", "{{ func $name$($params$) }}\n\t$cursor$\n{{ end }}");
    }

    private void AddParameterSnippets()
    {
        foreach (var param in _parameters)
        {
            AddSnippet(param.Key, $"{{{{ {param.Key} }}}}");

            // Add property snippets for complex objects
            if (param.Value != null)
            {
                AddPropertySnippets(param.Key, param.Value.GetType(), param.Value, 0);
            }
        }
    }

    private void AddPropertySnippets(string prefix, Type type, object? value, int depth)
    {
        if (depth > 2 || type == null) return;
        if (IsSimpleType(type)) return;

        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
            .Take(30);

        foreach (var prop in properties)
        {
            var propPath = $"{prefix}.{prop.Name}";
            AddSnippet(propPath.Replace(".", "_"), $"{{{{ {propPath} }}}}");

            if (!IsSimpleType(prop.PropertyType) && depth < 2)
            {
                object? propValue = null;
                try
                {
                    if (value != null)
                        propValue = prop.GetValue(value);
                }
                catch { }

                AddPropertySnippets(propPath, prop.PropertyType, propValue, depth + 1);
            }
        }
    }

    private void AddFunctionSnippets()
    {
        // Add custom method snippets
        foreach (var method in _methods)
        {
            var methodInfo = method.Value.Method;
            var paramNames = methodInfo.GetParameters().Select(p => $"${p.Name}$");
            var snippet = $"{{{{ {method.Key}({string.Join(", ", paramNames)}) }}}}";
            AddSnippet(method.Key, snippet);
        }

        // Add global function snippets
        foreach (var func in _globalFunctions)
        {
            if (func.Value is Delegate del)
            {
                var methodInfo = del.Method;
                var paramNames = methodInfo.GetParameters().Select(p => $"${p.Name}$");
                var snippet = $"{{{{ {func.Key}({string.Join(", ", paramNames)}) }}}}";
                AddSnippet(func.Key, snippet);
            }
            else
            {
                AddSnippet(func.Key, $"{{{{ {func.Key} }}}}");
            }
        }
    }

    private void AddSnippet(string title, string code)
    {
        try
        {
            var literals = new ArrayList();
            var snippet = new CodeSnippet(title, literals, code);
            SnippetsContainer.AddSnippet(snippet);
        }
        catch
        {
            // Ignore duplicate or invalid snippets
        }
    }

    private bool IsSimpleType(Type type)
    {
        return type.IsPrimitive || type.IsEnum || type == typeof(string) ||
               type == typeof(decimal) || type == typeof(DateTime) ||
               type == typeof(DateOnly) || type == typeof(TimeOnly) ||
               type == typeof(Guid);
    }

    #endregion
}