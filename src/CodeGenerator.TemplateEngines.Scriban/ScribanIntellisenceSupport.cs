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
        public static ContextChoiseTriggerCollection Triggers { get; } = new ContextChoiseTriggerCollection();

        static ScribanIntellisenceSupport()
        {
            //CreateKeywordsStatements();
            CreateForStatement();
            CreateWhileStatement();
            CreateBuildinFunctionStatements();
        }

        //private static void CreateKeywordsStatements()
        //{
        //    // Scriban keywords
        //    var keywords = new[] {
        //        "if", "else", "elseif", "end",  "in", "while", "break", 
        //        //"for", "while",-> handled seperatly
        //        "continue", "func", "ret", "capture", "readonly", "import",
        //        "with", "wrap", "include", "true", "false", "null", "empty",
        //        "blank", "this", "tablerow", "case", "when"
        //    };
        //    foreach (var keyword in keywords)
        //    {
        //        var trigger = new ContextChoiseTrigger(keyword, "Scriban keyword");
        //        Triggers.Add(trigger);
        //    }
        //}

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
            var forTrigger = new ContextChoiseTrigger("for", "syntax: for <item> in <list>");

            forTrigger.Items.Add(new ContextChoiseItem("index", "The current index of the for loop", GetIconKey(GenericDataTypes.Int)));
            forTrigger.Items.Add(new ContextChoiseItem("rindex", "The current index of the for loop starting from the end of the list", GetIconKey(GenericDataTypes.Int)));
            forTrigger.Items.Add(new ContextChoiseItem("first", "A boolean indicating whether this is the first step in the loop", GetIconKey(GenericDataTypes.Boolean)));
            forTrigger.Items.Add(new ContextChoiseItem("last", "A boolean indicating whether this is the last step in the loop", GetIconKey(GenericDataTypes.Boolean)));
            forTrigger.Items.Add(new ContextChoiseItem("even", "A boolean indicating whether this is an even row in the loop", GetIconKey(GenericDataTypes.Boolean)));
            forTrigger.Items.Add(new ContextChoiseItem("odd", "A boolean indicating whether this is an odd row in the loop", GetIconKey(GenericDataTypes.Boolean)));
            forTrigger.Items.Add(new ContextChoiseItem("changed", "A boolean indicating whether a current value of this iteration changed from previous step", GetIconKey(GenericDataTypes.Boolean)));
            forTrigger.Items.Add(new ContextChoiseItem("item", "The current item in the loop", GetIconKey(GenericDataTypes.Json)));

            Triggers.Add(forTrigger);
        }

        private static void CreateWhileStatement()
        {
            var whileTrigger = new ContextChoiseTrigger("while", "syntax: while <condition>");
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
                var trigger = new ContextChoiseTrigger(triggerText, $"Built-in {triggerText} functions");
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
                var trigger = new ContextChoiseTrigger(functionName, buildinFunctionsTooltips[functionName]);
                //AddBuildinFunctionMembers(trigger, functionName, null);
                Triggers.Add(trigger);
            }
        }
    }
}
