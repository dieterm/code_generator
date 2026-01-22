using CodeGenerator.Core.IntellicenseSupport;
using CodeGenerator.Domain.DataTypes;
using Scriban.Functions;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.TemplateEngines.Scriban
{
    public static class ScribanIntellisenceSupport
    {
        public static TypeFormatStyle FormatText { get; } = new TypeFormatStyle(FormatType.Text, "Black");
        public static TypeFormatStyle FormatKeyword { get; } = new TypeFormatStyle(FormatType.KeyWord, "Blue") { Bold=true };
        public static TypeFormatStyle FormatString { get; } = new TypeFormatStyle(FormatType.String, "#A31515");
        public static TypeFormatStyle FormatComment { get; } = new TypeFormatStyle(FormatType.Comment, "Green") { Italic=true };
        public static TypeFormatStyle FormatOperator { get; } = new TypeFormatStyle(FormatType.Operator, "#FF6600") { Bold=true };
        public static TypeFormatStyle FormatParameter { get; } = new TypeFormatStyle("Parameter", "Purple") { Bold=true };
        public static TypeFormatStyle FormatFunction { get; } = new TypeFormatStyle("Function", "#8B4513");
        public static TypeFormatStyle FormatNumber { get; } = new TypeFormatStyle(FormatType.Number, "DarkCyan");

        public static TypeFormatStyle[] Formats { get; } = new TypeFormatStyle[]
        {
            FormatText,
            FormatKeyword,
            FormatString,
            FormatComment,
            FormatOperator,
            FormatParameter,
            FormatFunction,
            FormatNumber
        };

        public static ContextChoiseTriggerCollection Triggers { get; } = new ContextChoiseTriggerCollection();

        static ScribanIntellisenceSupport()
        {
            CreateKeywordsStatements();
            CreateForStatement();
            CreateWhileStatement();
            CreateBuildinFunctionStatements();
            CreateOperatorsStatements();
        }

        private static void CreateOperatorsStatements()
        {
            // Arithmetic expressions
            Triggers.Add(new ContextChoiseTrigger("+", "<expression1> + <expression2> - Adds two expressions. e.g 1 + 2", FormatOperator));
            Triggers.Add(new ContextChoiseTrigger("-", "<expression1> - <expression2> - Subtracts expression2 from expression1. e.g 5 - 3", FormatOperator) );
            Triggers.Add(new ContextChoiseTrigger("*", "<expression1> * <expression2> - Multiplies two expressions. e.g 4 * 2", FormatOperator));
            Triggers.Add(new ContextChoiseTrigger("/", "<expression1> / <expression2> - Divides expression1 by expression2. e.g 10 / 2", FormatOperator));
            Triggers.Add(new ContextChoiseTrigger("//", "<expression1> // <expression2> - Divides expression1 by expression2 and rounds down to the nearest integer. e.g 7 // 2 = 3", FormatOperator) { UseSplitter = true });
            Triggers.Add(new ContextChoiseTrigger("%", "<expression1> % <expression2> - Calculates the modulus of expression1 by expression2. e.g 10 % 3 = 1", FormatOperator));
            Triggers.Add(new ContextChoiseTrigger("**", "<expression1> ** <expression2> - Raises expression1 to the power of expression2. e.g 2 ** 3 = 8", FormatOperator) { UseSplitter = true });
            
            // Conditional expressions
            Triggers.Add(new ContextChoiseTrigger("==", "<expression1> == <expression2> - Checks if two expressions are equal. e.g 5 == 5", FormatOperator) { UseSplitter = true });
            Triggers.Add(new ContextChoiseTrigger("!=", "<expression1> != <expression2> - Checks if two expressions are not equal. e.g 5 != 3", FormatOperator) { UseSplitter = true });
            Triggers.Add(new ContextChoiseTrigger(">", "<expression1> > <expression2> - Checks if expression1 is greater than expression2. e.g 7 > 5", FormatOperator) { UseSplitter = true });
            Triggers.Add(new ContextChoiseTrigger("<", "<expression1> < <expression2> - Checks if expression1 is less than expression2. e.g 3 < 5", FormatOperator));
            Triggers.Add(new ContextChoiseTrigger(">=", "<expression1> >= <expression2> - Checks if expression1 is greater than or equal to expression2. e.g 5 >= 5", FormatOperator) { UseSplitter = true });
            Triggers.Add(new ContextChoiseTrigger("<=", "<expression1> <= <expression2> - Checks if expression1 is less than or equal to expression2. e.g 3 <= 5", FormatOperator) { UseSplitter = true });

            // and/or operators
            Triggers.Add(new ContextChoiseTrigger("&&", "<expression1> && <expression2> - Logical AND operation between two boolean expressions. e.g true and false", FormatOperator) { UseSplitter = true });
            Triggers.Add(new ContextChoiseTrigger("||", "<expression1> || <expression2> - Logical OR operation between two boolean expressions. e.g true or false", FormatOperator) { UseSplitter = true });

            // Unary operators
            Triggers.Add(new ContextChoiseTrigger("!", "! <expression> - Boolean negate an expression. e.g if !page", FormatOperator));
            Triggers.Add(new ContextChoiseTrigger("^", "^ <variable> - Expand an array passed to arguments of a function call (see function call).", FormatOperator));
            Triggers.Add(new ContextChoiseTrigger("@", "@ <expression> - Alias the result of an expression that would be evaluated if it was a function call.", FormatOperator));
            Triggers.Add(new ContextChoiseTrigger("++", "++ <variable> - <variable> ++ - Increments the variable. Expression is evaluated to the value before/after it is incremented.", FormatOperator) { UseSplitter=true });
            Triggers.Add(new ContextChoiseTrigger("--", "-- <variable> - <variable> -- - Decrements the variable. Expression is evaluated to the value before/after it is decremented.", FormatOperator) { UseSplitter = true });

            // Range operators
            Triggers.Add(new ContextChoiseTrigger("..", "<start> .. <end> - Creates a range from start to end inclusive. e.g 1..10", FormatOperator) { UseSplitter = true });

            // The null-coalescing operators ??, ?!
            Triggers.Add(new ContextChoiseTrigger("??", "<expression1> ?? <expression2> - Returns expression1 if it is not null, otherwise returns expression2.", FormatOperator) { UseSplitter = true });
            Triggers.Add(new ContextChoiseTrigger("?!", "<expression1> ?! <expression2> - Returns expression1 if it is null, otherwise returns expression2.", FormatOperator) { UseSplitter = true });

            // Compound Assignment Operators
            Triggers.Add(new ContextChoiseTrigger("+=", "<variable> += <expression> - Adds <expression> to <variable> and assigns the result to <variable>.", FormatOperator) { UseSplitter = true });
            Triggers.Add(new ContextChoiseTrigger("-=", "<variable> -= <expression> - Subtracts <expression> from <variable> and assigns the result to <variable>.", FormatOperator) { UseSplitter = true });
            Triggers.Add(new ContextChoiseTrigger("*=", "<variable> *= <expression> - Multiplies <variable> by <expression> and assigns the result to <variable>.", FormatOperator) { UseSplitter = true });
            Triggers.Add(new ContextChoiseTrigger("/=", "<variable> /= <expression> - Divides <variable> by <expression> and assigns the result to <variable>.", FormatOperator) { UseSplitter = true });
            Triggers.Add(new ContextChoiseTrigger("//=", "<variable> //= <expression> - divide <variable> by <expression> number and round to an integer, and assigns the result to <variable>.", FormatOperator) { UseSplitter = true });
            Triggers.Add(new ContextChoiseTrigger("%=", "<variable> %= <expression> - calculates the modulus of <variable> by <expression>, and assigns the result to <variable>.", FormatOperator) { UseSplitter = true });

            // Whitespace contol operators
            Triggers.Add(new ContextChoiseTrigger("~", "Used to trim (non-greedy) whitespace (but no newlines) in template tags. e.g {{~ variable }} or {{ variable ~}}", FormatOperator) { UseSplitter = true });
            //Triggers.Add(new ContextChoiseTrigger("{{-", "Used to trim whitespace greedy in template tags. e.g {{- variable }} or {{ variable -}}", FormatOperator) { UseSplitter = true });
            //Triggers.Add(new ContextChoiseTrigger("-}}", "Used to trim whitespace greedy in template tags. e.g {{- variable }} or {{ variable -}}", FormatOperator) { UseSplitter = true });
            //Triggers.Add(new ContextChoiseTrigger("{{~", "will remove any preceeding whitespace until it reaches a non whitespace character such as a newline or letter", FormatOperator) { UseSplitter = true });
            //Triggers.Add(new ContextChoiseTrigger("~}}", "will remove any following whitespace including the first newline until it reaches a non whitespace character or a second newline", FormatOperator) { UseSplitter = true });
        }

        private static void CreateKeywordsStatements()
        {
            Triggers.Add(new ContextChoiseTrigger("end", "Ends a control flow statement such as if, for, while, func, capture, with, tablerow", FormatKeyword));
            Triggers.Add(new ContextChoiseTrigger("break", "Exits the nearest enclosing loop (for, while) or switch statement", FormatKeyword));
            Triggers.Add(new ContextChoiseTrigger("continue", "Skips the rest of the current loop iteration and moves to the next iteration", FormatKeyword));

            // If statement keywords
            Triggers.Add(new ContextChoiseTrigger("if", "syntax: if <condition> elseif <condition> else end", FormatKeyword));
            Triggers.Add(new ContextChoiseTrigger("else", "Defines the else branch of an if statement", FormatKeyword));
            Triggers.Add(new ContextChoiseTrigger("elseif", "Defines an else-if branch of an if statement", FormatKeyword));

            // For statement keywords
            // see CreateForStatement()

            // While statement keywords
            // see CreateWhileStatement()

            // Switch statement keywords
            Triggers.Add(new ContextChoiseTrigger("switch", "syntax: switch <expression> case <value> when <condition> else end", FormatKeyword));
            Triggers.Add(new ContextChoiseTrigger("case", "Defines a case branch of a switch statement", FormatKeyword));
            Triggers.Add(new ContextChoiseTrigger("when", "Defines a condition for a case branch of a switch statement", FormatKeyword));

            // Function definition keywords
            Triggers.Add(new ContextChoiseTrigger("func", "syntax: func <function_name>(<parameters>) end - Defines a function", FormatKeyword));
            Triggers.Add(new ContextChoiseTrigger("ret", "syntax: ret <expression> - Returns a value from a function", FormatKeyword));

            // Capture keyword
            Triggers.Add(new ContextChoiseTrigger("capture", "syntax: capture <variable_name> end - Captures the output of the enclosed block into a variable", FormatKeyword));

            // readonly keyword
            Triggers.Add(new ContextChoiseTrigger("readonly", "syntax: readonly <variable_name> = <expression> - Defines a read-only variable", FormatKeyword));

            // import keyword
            Triggers.Add(new ContextChoiseTrigger("import", "syntax: import '<template_path>' - Imports another template file", FormatKeyword));
            // include keyword
            Triggers.Add(new ContextChoiseTrigger("include", "syntax: include '<template_path>' - Includes another template file", FormatKeyword));
            // with keyword
            Triggers.Add(new ContextChoiseTrigger("with", "syntax: with <expression> end - Defines a scope for the enclosed block with the given expression", FormatKeyword));
            // wrap keyword
            Triggers.Add(new ContextChoiseTrigger("wrap", "syntax: wrap <expression> end - Wraps the output of the enclosed block with the given expression", FormatKeyword));
            // boolean literals
            Triggers.Add(new ContextChoiseTrigger("true", "Boolean literal representing true", FormatKeyword));
            Triggers.Add(new ContextChoiseTrigger("false", "Boolean literal representing false", FormatKeyword));
            // null literal
            Triggers.Add(new ContextChoiseTrigger("null", "Literal representing null value", FormatKeyword));
            // empty and blank literals
            Triggers.Add(new ContextChoiseTrigger("empty", "Literal representing an empty value", FormatKeyword));
            Triggers.Add(new ContextChoiseTrigger("blank", "Literal representing a blank value", FormatKeyword));

            // this keyword
            Triggers.Add(new ContextChoiseTrigger("this", "Refers to the current context object", FormatKeyword));
            // tablerow keyword
            Triggers.Add(new ContextChoiseTrigger("tablerow", "syntax: tablerow <item> in <list> cols:<number> end - Iterates over a list and generates a table row for each item. This function generates HTML rows compatible with an HTML table. Must be wrapped in an opening <table> and closing </table> HTML tags.", FormatKeyword));
        }

        public static string GetIconKey(GenericDataType dataType)
        {
            if (dataType == GenericDataTypes.Guid)
                return "square-asterisk";
            else if (dataType == GenericDataTypes.Xml)
                return "code-xml";
            else if (dataType == GenericDataTypes.Json)
                return "braces";
            else if (dataType == GenericDataTypes.Money)
                return "dollar-sign";
            else if (GenericDataTypes.IsTextBasedType(dataType.Id))
                return "case-sensitive";
            else if (GenericDataTypes.IsNumericType(dataType.Id))
                return "hash";
            else if (GenericDataTypes.IsDateType(dataType.Id))
                return "calendar";
            else if (GenericDataTypes.IsBinaryType(dataType.Id))
                return "binary";
            else if (GenericDataTypes.IsBooleanType(dataType.Id))
                return "toggle-left";
            else
                return "circle-question-mark";
        }

        private static void CreateForStatement()
        {
            var forTrigger = new ContextChoiseTrigger("for", "syntax: for <item> in <list> limit:<number> end", FormatKeyword);

            forTrigger.Items.Add(new ContextChoiseItem("index", "The current index of the for loop", GetIconKey(GenericDataTypes.Int)));
            forTrigger.Items.Add(new ContextChoiseItem("rindex", "The current index of the for loop starting from the end of the list", GetIconKey(GenericDataTypes.Int)));
            forTrigger.Items.Add(new ContextChoiseItem("first", "A boolean indicating whether this is the first step in the loop", GetIconKey(GenericDataTypes.Boolean)));
            forTrigger.Items.Add(new ContextChoiseItem("last", "A boolean indicating whether this is the last step in the loop", GetIconKey(GenericDataTypes.Boolean)));
            forTrigger.Items.Add(new ContextChoiseItem("even", "A boolean indicating whether this is an even row in the loop", GetIconKey(GenericDataTypes.Boolean)));
            forTrigger.Items.Add(new ContextChoiseItem("odd", "A boolean indicating whether this is an odd row in the loop", GetIconKey(GenericDataTypes.Boolean)));
            forTrigger.Items.Add(new ContextChoiseItem("changed", "A boolean indicating whether a current value of this iteration changed from previous step", GetIconKey(GenericDataTypes.Boolean)));
            forTrigger.Items.Add(new ContextChoiseItem("item", "The current item in the loop", GetIconKey(GenericDataTypes.Json)));

            Triggers.Add(forTrigger);
            Triggers.Add(new ContextChoiseTrigger("in", "syntax: for <item> in <list> end - Used in for statements to iterate over a list", FormatKeyword));
            Triggers.Add(new ContextChoiseTrigger("limit", "syntax: for <item> in <list> limit:<number> end - Limits the number of iterations in a for loop", FormatKeyword));
            Triggers.Add(new ContextChoiseTrigger("reversed", "syntax: for <item> in <list> reversed end - Reverses the order of iteration in a for loop", FormatKeyword));
        }

        private static void CreateWhileStatement()
        {
            var whileTrigger = new ContextChoiseTrigger("while", "syntax: while <condition> end", FormatKeyword);
            whileTrigger.Items.Add(new ContextChoiseItem("index", "The current index of the while loop", GetIconKey(GenericDataTypes.Int)));
            whileTrigger.Items.Add(new ContextChoiseItem("first", "A boolean indicating whether this is the first step in the loop", GetIconKey(GenericDataTypes.Boolean)));
            whileTrigger.Items.Add(new ContextChoiseItem("even", "A boolean indicating whether this is an even row in the loop", GetIconKey(GenericDataTypes.Boolean)));
            whileTrigger.Items.Add(new ContextChoiseItem("odd", "A boolean indicating whether this is an odd row in the loop", GetIconKey(GenericDataTypes.Boolean)));
            Triggers.Add(whileTrigger);
        }

        private static void CreateBuildinFunctionStatements()
        {
            var _buildinfunctions = new global::Scriban.Functions.BuiltinFunctions();
            foreach(var (triggerText, scribanObject) in _buildinfunctions)
            {
                if(triggerText=="blank" || triggerText=="empty" || triggerText=="include")
                {
                    // skip these, the are already added in the Keywords section
                    continue;
                }
                var trigger = new ContextChoiseTrigger(triggerText, $"Built-in {triggerText} functions", FormatKeyword);
                AddBuildinFunctionMembers(trigger, triggerText, scribanObject);
                Triggers.Add(trigger);
            }
            
        }

        private static void AddBuildinFunctionMembers(ContextChoiseTrigger trigger, string triggerText, object? memberObj)
        {
            if (memberObj is ScriptObject scriptObj)
            {
                foreach (var (functionName, functionBody) in scriptObj)
                {
                    var funcbod = functionBody as IScriptCustomFunction;
                    if (funcbod != null)
                    {
                        string paramTooltip = CreateTooltipForScriptFunction(funcbod);
                        trigger.Items.Add(new ContextChoiseItem(functionName, $"{functionName}({paramTooltip})", "square-function"));
                    }
                    else
                    {
                        trigger.Items.Add(new ContextChoiseItem(functionName, functionName, "square-function"));
                        Debug.WriteLine($"    Not a custom function");
                    }
                }
            }
            else if (memberObj is EmptyScriptObject emptyScriptObject)
            {
                // eg: "empty", "blank"
            }
            else if (memberObj is IScriptCustomFunction includeFunction)
            {
                // eg: "include" 
                var paramTooltip = CreateTooltipForScriptFunction(includeFunction);
                trigger.Tooltip = $"{triggerText}({paramTooltip})";
            }
        }

        private static string CreateTooltipForScriptFunction(IScriptCustomFunction funcbod)
        {
            StringBuilder paramList = new StringBuilder();
            for (int i = 0; i < funcbod.ParameterCount; i++)
            {
                var paparamInfo = funcbod.GetParameterInfo(i);
                paramList.Append($"<{paparamInfo.Name}>");
                if (i < funcbod.ParameterCount - 1)
                    paramList.Append(", ");
            }
            var paramTooltip = paramList.ToString().Trim();
            return paramTooltip;
        }

        public static void CreateGlobalFunctionStatements(string[] buildinFunctions, Dictionary<string, string> buildinFunctionsTooltips)
        {
            foreach (var functionName in buildinFunctions)
            {
                var trigger = new ContextChoiseTrigger(functionName, buildinFunctionsTooltips[functionName], FormatKeyword);
                //AddBuildinFunctionMembers(trigger, functionName, null);
                Triggers.Add(trigger);
            }
        }
    }
}
