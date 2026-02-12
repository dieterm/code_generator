using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Domain.CodeElements.Statements;
using CodeGenerator.Domain.ProgrammingLanguages;
using System.Text.RegularExpressions;

namespace CodeGenerator.Domain.ProgrammingLanguages.Python
{
    /// <summary>
    /// Code parser for Python programming language.
    /// Parses Python source code into CodeElement structures using line-based analysis.
    /// </summary>
    public class PythonCodeParser : ProgrammingLanguageCodeParser
    {
        public override ProgrammingLanguage Language => PythonLanguage.Instance;

        private string[] _lines = Array.Empty<string>();
        private int _currentLine;

        public override CodeFileElement ParseCodeFile(string sourceCode, string fileName)
        {
            var codeFile = new CodeFileElement(fileName, PythonLanguage.Instance);

            _lines = sourceCode.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            _currentLine = 0;

            // File header (leading comments)
            codeFile.FileHeader = ExtractFileHeader();

            // Parse top-level elements
            while (_currentLine < _lines.Length)
            {
                var line = _lines[_currentLine];
                var trimmed = line.TrimStart();

                if (string.IsNullOrWhiteSpace(trimmed))
                {
                    _currentLine++;
                    continue;
                }

                // Skip comments
                if (trimmed.StartsWith("#"))
                {
                    _currentLine++;
                    continue;
                }

                // Import statements
                if (trimmed.StartsWith("import ") || trimmed.StartsWith("from "))
                {
                    codeFile.Usings.Add(ParseImport(trimmed));
                    _currentLine++;
                    continue;
                }

                // Class definition
                if (trimmed.StartsWith("class "))
                {
                    var indent = GetIndentLevel(line);
                    codeFile.TopLevelTypes.Add(ParseClass(indent));
                    continue;
                }

                // Top-level function (treated as method in a default namespace)
                if (trimmed.StartsWith("def "))
                {
                    var indent = GetIndentLevel(line);
                    var method = ParseFunction(indent);
                    // Add top-level functions as methods on a module-level class
                    var moduleClass = GetOrCreateModuleClass(codeFile, fileName);
                    moduleClass.Methods.Add(method);
                    continue;
                }

                // Decorator — peek ahead
                if (trimmed.StartsWith("@"))
                {
                    var indent = GetIndentLevel(line);
                    var decorators = CollectDecorators(indent);

                    if (_currentLine < _lines.Length)
                    {
                        var nextTrimmed = _lines[_currentLine].TrimStart();
                        if (nextTrimmed.StartsWith("class "))
                        {
                            var cls = ParseClass(indent);
                            foreach (var dec in decorators)
                                cls.Attributes.Add(new AttributeElement(dec));
                            codeFile.TopLevelTypes.Add(cls);
                            continue;
                        }
                        else if (nextTrimmed.StartsWith("def "))
                        {
                            var method = ParseFunction(indent);
                            foreach (var dec in decorators)
                                method.Attributes.Add(new AttributeElement(dec));
                            var moduleClass = GetOrCreateModuleClass(codeFile, fileName);
                            moduleClass.Methods.Add(method);
                            continue;
                        }
                    }

                    _currentLine++;
                    continue;
                }

                // Top-level statements / assignments
                codeFile.TopLevelStatements.Add(trimmed);
                _currentLine++;
            }

            return codeFile;
        }

        #region File Header

        private string? ExtractFileHeader()
        {
            var headerLines = new List<string>();

            // Check for docstring header (triple quotes)
            if (_currentLine < _lines.Length)
            {
                var trimmed = _lines[_currentLine].TrimStart();
                if (trimmed.StartsWith("\"\"\"") || trimmed.StartsWith("'''"))
                {
                    var quote = trimmed.Substring(0, 3);
                    // Single-line docstring
                    if (trimmed.Length > 3 && trimmed.EndsWith(quote) && trimmed.Length > 6)
                    {
                        headerLines.Add(trimmed.Substring(3, trimmed.Length - 6));
                        _currentLine++;
                        return string.Join(Environment.NewLine, headerLines);
                    }
                    // Multi-line docstring
                    headerLines.Add(trimmed.Substring(3));
                    _currentLine++;
                    while (_currentLine < _lines.Length)
                    {
                        var line = _lines[_currentLine];
                        if (line.TrimStart().EndsWith(quote))
                        {
                            var endText = line.TrimStart();
                            endText = endText.Substring(0, endText.Length - 3);
                            if (!string.IsNullOrWhiteSpace(endText))
                                headerLines.Add(endText.Trim());
                            _currentLine++;
                            break;
                        }
                        headerLines.Add(line.Trim());
                        _currentLine++;
                    }
                    var result = string.Join(Environment.NewLine, headerLines).Trim();
                    return string.IsNullOrEmpty(result) ? null : result;
                }
            }

            // Hash-comment header
            while (_currentLine < _lines.Length)
            {
                var trimmed = _lines[_currentLine].TrimStart();
                if (trimmed.StartsWith("# "))
                    headerLines.Add(trimmed.Substring(2));
                else if (trimmed.StartsWith("#"))
                    headerLines.Add(trimmed.Substring(1));
                else
                    break;
                _currentLine++;
            }

            if (headerLines.Count == 0) return null;
            var headerResult = string.Join(Environment.NewLine, headerLines);
            return string.IsNullOrWhiteSpace(headerResult) ? null : headerResult;
        }

        #endregion

        #region Import

        private UsingElement ParseImport(string line)
        {
            var element = new UsingElement();

            // from X import Y [as Z]
            var fromMatch = Regex.Match(line, @"^from\s+(\S+)\s+import\s+(.+)$");
            if (fromMatch.Success)
            {
                var module = fromMatch.Groups[1].Value;
                var imports = fromMatch.Groups[2].Value.Trim();

                var aliasMatch = Regex.Match(imports, @"^(\S+)\s+as\s+(\S+)$");
                if (aliasMatch.Success)
                {
                    element.Namespace = $"{module}.{aliasMatch.Groups[1].Value}";
                    element.Alias = aliasMatch.Groups[2].Value;
                }
                else
                {
                    element.Namespace = $"{module}.{imports}";
                }
                return element;
            }

            // import X [as Y]
            var importMatch = Regex.Match(line, @"^import\s+(\S+)(?:\s+as\s+(\S+))?$");
            if (importMatch.Success)
            {
                element.Namespace = importMatch.Groups[1].Value;
                if (importMatch.Groups[2].Success)
                    element.Alias = importMatch.Groups[2].Value;
                return element;
            }

            element.Namespace = line.Replace("import ", "").Replace("from ", "").Trim();
            return element;
        }

        #endregion

        #region Class

        private ClassElement ParseClass(int classIndent)
        {
            var line = _lines[_currentLine].TrimStart();
            var classMatch = Regex.Match(line, @"^class\s+(\w+)(?:\(([^)]*)\))?:");

            var element = new ClassElement();
            if (classMatch.Success)
            {
                element.Name = classMatch.Groups[1].Value;
                if (classMatch.Groups[2].Success && !string.IsNullOrWhiteSpace(classMatch.Groups[2].Value))
                {
                    var bases = classMatch.Groups[2].Value.Split(',');
                    foreach (var b in bases)
                        element.BaseTypes.Add(new TypeReference(b.Trim()));
                }
            }
            else
            {
                element.Name = line.Replace("class ", "").Replace(":", "").Trim();
            }

            element.Documentation = ExtractDocComment();
            _currentLine++;

            // Parse class body
            var bodyIndent = classIndent + 1;
            SkipBlankLines();

            // Docstring
            if (_currentLine < _lines.Length)
            {
                var docstring = TryParseDocstring(bodyIndent);
                if (docstring != null)
                    element.Documentation = docstring;
            }

            while (_currentLine < _lines.Length)
            {
                var bodyLine = _lines[_currentLine];
                if (string.IsNullOrWhiteSpace(bodyLine))
                {
                    _currentLine++;
                    continue;
                }

                var currentIndent = GetIndentLevel(bodyLine);
                if (currentIndent < bodyIndent)
                    break;

                var bodyTrimmed = bodyLine.TrimStart();

                // Skip comments
                if (bodyTrimmed.StartsWith("#"))
                {
                    _currentLine++;
                    continue;
                }

                // Collect decorators
                if (bodyTrimmed.StartsWith("@"))
                {
                    var decorators = CollectDecorators(currentIndent);

                    if (_currentLine < _lines.Length)
                    {
                        var nextTrimmed = _lines[_currentLine].TrimStart();
                        if (nextTrimmed.StartsWith("def __init__"))
                        {
                            var ctor = ParseConstructorFromInit(currentIndent, element.Name);
                            foreach (var dec in decorators)
                                ctor.Attributes.Add(new AttributeElement(dec));
                            element.Constructors.Add(ctor);
                            continue;
                        }
                        else if (nextTrimmed.StartsWith("def "))
                        {
                            bool isProperty = decorators.Any(d => d == "property" || d.EndsWith(".setter") || d.EndsWith(".getter") || d.EndsWith(".deleter"));
                            if (isProperty)
                            {
                                var prop = ParsePropertyFromDecorator(currentIndent, decorators);
                                // Merge with existing property of same name
                                var existing = element.Properties.FirstOrDefault(p => p.Name == prop.Name);
                                if (existing != null)
                                {
                                    if (prop.HasSetter) existing.HasSetter = true;
                                }
                                else
                                {
                                    element.Properties.Add(prop);
                                }
                                continue;
                            }
                            else
                            {
                                var method = ParseFunction(currentIndent);
                                foreach (var dec in decorators)
                                    method.Attributes.Add(new AttributeElement(dec));
                                if (decorators.Any(d => d == "staticmethod"))
                                    method.Modifiers |= ElementModifiers.Static;
                                element.Methods.Add(method);
                                continue;
                            }
                        }
                    }

                    _currentLine++;
                    continue;
                }

                // __init__ method ? constructor
                if (bodyTrimmed.StartsWith("def __init__"))
                {
                    element.Constructors.Add(ParseConstructorFromInit(currentIndent, element.Name));
                    continue;
                }

                // Method
                if (bodyTrimmed.StartsWith("def "))
                {
                    element.Methods.Add(ParseFunction(currentIndent));
                    continue;
                }

                // Nested class
                if (bodyTrimmed.StartsWith("class "))
                {
                    element.NestedTypes.Add(ParseClass(currentIndent));
                    continue;
                }

                // Class-level field assignment (e.g., x = 5, x: int = 5)
                var fieldMatch = Regex.Match(bodyTrimmed, @"^(\w+)\s*(?::\s*(\S+))?\s*=\s*(.+)$");
                if (fieldMatch.Success && !bodyTrimmed.StartsWith("self."))
                {
                    var field = new FieldElement();
                    field.Name = fieldMatch.Groups[1].Value;
                    field.Type = fieldMatch.Groups[2].Success
                        ? new TypeReference(fieldMatch.Groups[2].Value)
                        : new TypeReference("object");
                    field.InitialValue = fieldMatch.Groups[3].Value.Trim();
                    field.AccessModifier = field.Name.StartsWith("_") ? AccessModifier.Private : AccessModifier.Public;
                    element.Fields.Add(field);
                    _currentLine++;
                    continue;
                }

                // Anything else as raw code
                _currentLine++;
            }

            return element;
        }

        #endregion

        #region Function / Method

        private MethodElement ParseFunction(int defIndent)
        {
            var line = _lines[_currentLine].TrimStart();
            var element = new MethodElement();

            // def name(params) -> return_type:
            var defMatch = Regex.Match(line, @"^def\s+(\w+)\s*\(([^)]*)\)(?:\s*->\s*(.+?))?\s*:");
            if (defMatch.Success)
            {
                element.Name = defMatch.Groups[1].Value;

                // Parameters
                var paramStr = defMatch.Groups[2].Value.Trim();
                if (!string.IsNullOrEmpty(paramStr))
                {
                    foreach (var p in SplitParameters(paramStr))
                    {
                        var param = ParsePythonParameter(p.Trim());
                        if (param.Name == "self" || param.Name == "cls") continue;
                        element.Parameters.Add(param);
                    }
                }

                // Return type
                if (defMatch.Groups[3].Success)
                    element.ReturnType = new TypeReference(defMatch.Groups[3].Value.Trim());
            }
            else
            {
                element.Name = line.Replace("def ", "").Replace(":", "").Trim();
            }

            element.Documentation = ExtractDocComment();
            element.AccessModifier = element.Name != null && element.Name.StartsWith("_")
                ? AccessModifier.Private : AccessModifier.Public;

            _currentLine++;

            // Parse body
            var bodyIndent = defIndent + 1;
            SkipBlankLines();

            // Docstring
            if (_currentLine < _lines.Length)
            {
                var docstring = TryParseDocstring(bodyIndent);
                if (docstring != null)
                    element.Documentation = docstring;
            }

            ParseFunctionBody(element.Body, bodyIndent);

            return element;
        }

        private ConstructorElement ParseConstructorFromInit(int defIndent, string? className)
        {
            var line = _lines[_currentLine].TrimStart();
            var element = new ConstructorElement();
            element.Name = className;

            var defMatch = Regex.Match(line, @"^def\s+__init__\s*\(([^)]*)\)\s*:");
            if (defMatch.Success)
            {
                var paramStr = defMatch.Groups[1].Value.Trim();
                if (!string.IsNullOrEmpty(paramStr))
                {
                    foreach (var p in SplitParameters(paramStr))
                    {
                        var param = ParsePythonParameter(p.Trim());
                        if (param.Name == "self") continue;
                        element.Parameters.Add(param);
                    }
                }
            }

            _currentLine++;

            var bodyIndent = defIndent + 1;
            SkipBlankLines();

            var docstring = TryParseDocstring(bodyIndent);
            if (docstring != null)
                element.Documentation = docstring;

            ParseFunctionBody(element.Body, bodyIndent);

            return element;
        }

        private PropertyElement ParsePropertyFromDecorator(int defIndent, List<string> decorators)
        {
            var line = _lines[_currentLine].TrimStart();
            var element = new PropertyElement();

            var defMatch = Regex.Match(line, @"^def\s+(\w+)\s*\(([^)]*)\)(?:\s*->\s*(.+?))?\s*:");
            if (defMatch.Success)
            {
                element.Name = defMatch.Groups[1].Value;
                if (defMatch.Groups[3].Success)
                    element.Type = new TypeReference(defMatch.Groups[3].Value.Trim());
            }

            element.IsAutoImplemented = false;

            // Determine getter vs setter
            bool isSetter = decorators.Any(d => d.EndsWith(".setter"));
            element.HasGetter = !isSetter;
            element.HasSetter = isSetter;

            _currentLine++;

            var bodyIndent = defIndent + 1;
            SkipBlankLines();
            TryParseDocstring(bodyIndent);

            // Skip body
            while (_currentLine < _lines.Length)
            {
                var bodyLine = _lines[_currentLine];
                if (string.IsNullOrWhiteSpace(bodyLine))
                {
                    _currentLine++;
                    continue;
                }
                if (GetIndentLevel(bodyLine) < bodyIndent)
                    break;
                _currentLine++;
            }

            return element;
        }

        #endregion

        #region Function Body / Statements

        private void ParseFunctionBody(CompositeStatement body, int expectedIndent)
        {
            while (_currentLine < _lines.Length)
            {
                var line = _lines[_currentLine];
                if (string.IsNullOrWhiteSpace(line))
                {
                    _currentLine++;
                    continue;
                }

                var indent = GetIndentLevel(line);
                if (indent < expectedIndent)
                    break;

                var trimmed = line.TrimStart();

                // Comments
                if (trimmed.StartsWith("#"))
                {
                    var commentText = trimmed.StartsWith("# ") ? trimmed.Substring(2) : trimmed.Substring(1);
                    body.Statements.Add(new CommentStatement(commentText));
                    _currentLine++;
                    continue;
                }

                // pass
                if (trimmed == "pass")
                {
                    _currentLine++;
                    continue;
                }

                // return
                if (trimmed.StartsWith("return"))
                {
                    var expr = trimmed.Length > 6 ? trimmed.Substring(7).Trim() : null;
                    body.Statements.Add(new ReturnStatementElement(expr));
                    _currentLine++;
                    continue;
                }

                // raise (? throw)
                if (trimmed.StartsWith("raise"))
                {
                    var expr = trimmed.Length > 5 ? trimmed.Substring(6).Trim() : null;
                    body.Statements.Add(new ThrowStatementElement(expr));
                    _currentLine++;
                    continue;
                }

                // if statement
                if (trimmed.StartsWith("if ") && trimmed.EndsWith(":"))
                {
                    ParsePythonIfStatement(body, indent);
                    continue;
                }

                // for loop
                if (trimmed.StartsWith("for ") && trimmed.EndsWith(":"))
                {
                    ParsePythonForStatement(body, indent);
                    continue;
                }

                // while loop
                if (trimmed.StartsWith("while ") && trimmed.EndsWith(":"))
                {
                    ParsePythonWhileStatement(body, indent);
                    continue;
                }

                // try/except
                if (trimmed == "try:" || trimmed.StartsWith("try:"))
                {
                    ParsePythonTryStatement(body, indent);
                    continue;
                }

                // with statement (? using)
                if (trimmed.StartsWith("with ") && trimmed.EndsWith(":"))
                {
                    ParsePythonWithStatement(body, indent);
                    continue;
                }

                // Assignment
                var assignMatch = Regex.Match(trimmed, @"^(\w[\w.]*)\s*=\s*(.+)$");
                if (assignMatch.Success && !trimmed.Contains("=="))
                {
                    body.Statements.Add(new AssignmentStatement(
                        assignMatch.Groups[1].Value,
                        assignMatch.Groups[2].Value.Trim()));
                    _currentLine++;
                    continue;
                }

                // Default: raw statement
                body.Statements.Add(new RawStatementElement(trimmed));
                _currentLine++;
            }
        }

        private void ParsePythonIfStatement(CompositeStatement parent, int ifIndent)
        {
            var line = _lines[_currentLine].TrimStart();
            var condition = line.Substring(3, line.Length - 4).Trim(); // "if X:"
            var ifElement = new IfStatementElement(condition);
            ifElement.Name = "If";
            _currentLine++;

            ParseFunctionBody(ifElement.ThenStatements, ifIndent + 1);

            // elif branches
            while (_currentLine < _lines.Length)
            {
                var nextLine = _lines[_currentLine];
                if (string.IsNullOrWhiteSpace(nextLine)) { _currentLine++; continue; }
                var nextIndent = GetIndentLevel(nextLine);
                var nextTrimmed = nextLine.TrimStart();

                if (nextIndent == ifIndent && nextTrimmed.StartsWith("elif ") && nextTrimmed.EndsWith(":"))
                {
                    var elifCondition = nextTrimmed.Substring(5, nextTrimmed.Length - 6).Trim();
                    var branch = new ElseIfBranch { Condition = elifCondition };
                    _currentLine++;
                    ParseFunctionBody(branch.Statements, ifIndent + 1);
                    ifElement.ElseIfBranches.Add(branch);
                }
                else
                {
                    break;
                }
            }

            // else block
            if (_currentLine < _lines.Length)
            {
                var nextLine = _lines[_currentLine];
                if (!string.IsNullOrWhiteSpace(nextLine))
                {
                    var nextIndent = GetIndentLevel(nextLine);
                    var nextTrimmed = nextLine.TrimStart();
                    if (nextIndent == ifIndent && (nextTrimmed == "else:" || nextTrimmed.StartsWith("else:")))
                    {
                        _currentLine++;
                        ParseFunctionBody(ifElement.ElseStatements, ifIndent + 1);
                    }
                }
            }

            parent.Statements.Add(ifElement);
        }

        private void ParsePythonForStatement(CompositeStatement parent, int forIndent)
        {
            var line = _lines[_currentLine].TrimStart();
            // for X in Y:
            var forMatch = Regex.Match(line, @"^for\s+(.+?)\s+in\s+(.+):$");
            if (forMatch.Success)
            {
                var forEachElement = new ForEachStatementElement(
                    forMatch.Groups[1].Value.Trim(),
                    forMatch.Groups[2].Value.Trim());
                forEachElement.Name = "ForEach";
                _currentLine++;
                ParseFunctionBody(forEachElement.Body, forIndent + 1);
                parent.Statements.Add(forEachElement);
            }
            else
            {
                parent.Statements.Add(new RawStatementElement(line));
                _currentLine++;
            }
        }

        private void ParsePythonWhileStatement(CompositeStatement parent, int whileIndent)
        {
            var line = _lines[_currentLine].TrimStart();
            var condition = line.Substring(6, line.Length - 7).Trim(); // "while X:"
            var whileElement = new WhileStatementElement(condition);
            whileElement.Name = "While";
            _currentLine++;
            ParseFunctionBody(whileElement.Body, whileIndent + 1);
            parent.Statements.Add(whileElement);
        }

        private void ParsePythonTryStatement(CompositeStatement parent, int tryIndent)
        {
            var tryCatchElement = new TryCatchStatementElement();
            tryCatchElement.Name = "TryCatch";
            _currentLine++;
            ParseFunctionBody(tryCatchElement.TryStatements, tryIndent + 1);

            // except blocks
            while (_currentLine < _lines.Length)
            {
                var nextLine = _lines[_currentLine];
                if (string.IsNullOrWhiteSpace(nextLine)) { _currentLine++; continue; }
                var nextIndent = GetIndentLevel(nextLine);
                var nextTrimmed = nextLine.TrimStart();

                if (nextIndent == tryIndent && nextTrimmed.StartsWith("except"))
                {
                    var catchBlock = new CatchBlock();
                    // except ExceptionType as e:
                    var exceptMatch = Regex.Match(nextTrimmed, @"^except\s+(\w[\w.]*)\s+as\s+(\w+)\s*:");
                    if (exceptMatch.Success)
                    {
                        catchBlock.ExceptionType = new TypeReference(exceptMatch.Groups[1].Value);
                        catchBlock.ExceptionVariable = exceptMatch.Groups[2].Value;
                    }
                    else
                    {
                        // except ExceptionType:
                        var exceptTypeMatch = Regex.Match(nextTrimmed, @"^except\s+(\w[\w.]*)\s*:");
                        if (exceptTypeMatch.Success)
                            catchBlock.ExceptionType = new TypeReference(exceptTypeMatch.Groups[1].Value);
                    }
                    _currentLine++;
                    ParseFunctionBody(catchBlock.Statements, tryIndent + 1);
                    tryCatchElement.CatchBlocks.Add(catchBlock);
                }
                else
                {
                    break;
                }
            }

            // finally block
            if (_currentLine < _lines.Length)
            {
                var nextLine = _lines[_currentLine];
                if (!string.IsNullOrWhiteSpace(nextLine))
                {
                    var nextIndent = GetIndentLevel(nextLine);
                    var nextTrimmed = nextLine.TrimStart();
                    if (nextIndent == tryIndent && (nextTrimmed == "finally:" || nextTrimmed.StartsWith("finally:")))
                    {
                        _currentLine++;
                        ParseFunctionBody(tryCatchElement.FinallyStatements, tryIndent + 1);
                    }
                }
            }

            parent.Statements.Add(tryCatchElement);
        }

        private void ParsePythonWithStatement(CompositeStatement parent, int withIndent)
        {
            var line = _lines[_currentLine].TrimStart();
            // with X as Y:  or  with X:
            var resource = line.Substring(5, line.Length - 6).Trim(); // "with ... :"
            var usingElement = new UsingStatementElement();
            usingElement.Name = "Using";
            usingElement.Resource = resource;
            _currentLine++;
            ParseFunctionBody(usingElement.Body, withIndent + 1);
            parent.Statements.Add(usingElement);
        }

        #endregion

        #region Helpers

        private int GetIndentLevel(string line)
        {
            int spaces = 0;
            foreach (var c in line)
            {
                if (c == ' ') spaces++;
                else if (c == '\t') spaces += 4;
                else break;
            }
            return spaces / 4;
        }

        private void SkipBlankLines()
        {
            while (_currentLine < _lines.Length && string.IsNullOrWhiteSpace(_lines[_currentLine]))
                _currentLine++;
        }

        private string? ExtractDocComment()
        {
            // Check line above for # comment
            if (_currentLine > 0)
            {
                var prevLine = _lines[_currentLine - 1].TrimStart();
                if (prevLine.StartsWith("# "))
                    return prevLine.Substring(2);
                if (prevLine.StartsWith("#"))
                    return prevLine.Substring(1);
            }
            return null;
        }

        private string? TryParseDocstring(int expectedIndent)
        {
            if (_currentLine >= _lines.Length) return null;

            var line = _lines[_currentLine];
            var indent = GetIndentLevel(line);
            if (indent < expectedIndent) return null;

            var trimmed = line.TrimStart();
            if (!trimmed.StartsWith("\"\"\"") && !trimmed.StartsWith("'''"))
                return null;

            var quote = trimmed.Substring(0, 3);

            // Single-line docstring
            if (trimmed.Length > 6 && trimmed.EndsWith(quote))
            {
                _currentLine++;
                return trimmed.Substring(3, trimmed.Length - 6).Trim();
            }

            // Multi-line docstring
            var docLines = new List<string>();
            var firstLine = trimmed.Substring(3).Trim();
            if (!string.IsNullOrEmpty(firstLine))
                docLines.Add(firstLine);
            _currentLine++;

            while (_currentLine < _lines.Length)
            {
                var docLine = _lines[_currentLine];
                var docTrimmed = docLine.TrimStart();
                if (docTrimmed.Contains(quote))
                {
                    var endText = docTrimmed.Replace(quote, "").Trim();
                    if (!string.IsNullOrEmpty(endText))
                        docLines.Add(endText);
                    _currentLine++;
                    break;
                }
                docLines.Add(docTrimmed);
                _currentLine++;
            }

            var result = string.Join(Environment.NewLine, docLines).Trim();
            return string.IsNullOrEmpty(result) ? null : result;
        }

        private ParameterElement ParsePythonParameter(string paramText)
        {
            var element = new ParameterElement();

            // *args
            if (paramText.StartsWith("**"))
            {
                element.Modifier = ParameterModifier.KeywordArgs;
                paramText = paramText.Substring(2);
            }
            else if (paramText.StartsWith("*"))
            {
                element.Modifier = ParameterModifier.VarArgs;
                paramText = paramText.Substring(1);
            }

            // name: type = default
            var match = Regex.Match(paramText, @"^(\w+)(?:\s*:\s*(.+?))?(?:\s*=\s*(.+))?$");
            if (match.Success)
            {
                element.Name = match.Groups[1].Value;
                element.Type = match.Groups[2].Success
                    ? new TypeReference(match.Groups[2].Value.Trim())
                    : new TypeReference("object");
                if (match.Groups[3].Success)
                    element.DefaultValue = match.Groups[3].Value.Trim();
            }
            else
            {
                element.Name = paramText;
                element.Type = new TypeReference("object");
            }

            return element;
        }

        private List<string> SplitParameters(string paramStr)
        {
            var result = new List<string>();
            int depth = 0;
            int start = 0;
            for (int i = 0; i < paramStr.Length; i++)
            {
                if (paramStr[i] == '(' || paramStr[i] == '[') depth++;
                else if (paramStr[i] == ')' || paramStr[i] == ']') depth--;
                else if (paramStr[i] == ',' && depth == 0)
                {
                    result.Add(paramStr.Substring(start, i - start));
                    start = i + 1;
                }
            }
            result.Add(paramStr.Substring(start));
            return result;
        }

        private List<string> CollectDecorators(int indent)
        {
            var decorators = new List<string>();
            while (_currentLine < _lines.Length)
            {
                var line = _lines[_currentLine];
                if (string.IsNullOrWhiteSpace(line)) { _currentLine++; continue; }
                var currentIndent = GetIndentLevel(line);
                var trimmed = line.TrimStart();
                if (currentIndent == indent && trimmed.StartsWith("@"))
                {
                    decorators.Add(trimmed.Substring(1).Trim());
                    _currentLine++;
                }
                else
                {
                    break;
                }
            }
            return decorators;
        }

        private ClassElement GetOrCreateModuleClass(CodeFileElement codeFile, string fileName)
        {
            var moduleClass = codeFile.TopLevelTypes.OfType<ClassElement>()
                .FirstOrDefault(c => c.Name == fileName);
            if (moduleClass == null)
            {
                moduleClass = new ClassElement(fileName);
                moduleClass.Documentation = "Module-level functions";
                moduleClass.Modifiers = ElementModifiers.Static;
                codeFile.TopLevelTypes.Add(moduleClass);
            }
            return moduleClass;
        }

        #endregion
    }
}
