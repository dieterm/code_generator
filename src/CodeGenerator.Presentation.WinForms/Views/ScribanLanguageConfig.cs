using Scriban.Runtime;
using Syncfusion.Windows.Forms.Edit.Enums;
using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Utils.CodeSnippets;
using System.Collections;

namespace CodeGenerator.Presentation.WinForms.Views
{
    public class ScribanLanguageConfig : IConfigLanguage
    {
        public readonly Dictionary<string, object?> _parameters;
        private readonly Dictionary<string, Delegate> _methods;
        private readonly ScriptObject _globalFunctions;

        public ScribanLanguageConfig(Dictionary<string, object?> parameters, Dictionary<string, Delegate> methods, Scriban.Runtime.ScriptObject globalFunctions)
        {
            _parameters = parameters;
            _methods = methods;
            _globalFunctions = globalFunctions;
        }

        public ISnippetFormat this[string name] => throw new NotImplementedException();

        public ISnippetFormat this[FormatType type] => throw new NotImplementedException();

        public ISnippetFormat this[int index] => throw new NotImplementedException();

        public string Language => throw new NotImplementedException();

        public string OneCharTokenSplits { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ArrayList Lexems => throw new NotImplementedException();

        public CodeSnippetsContainer SnippetsContainer => throw new NotImplementedException();

        public ArrayList AutoReplaceTriggers => throw new NotImplementedException();

        public ArrayList Splits { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ArrayList KnownFormats => throw new NotImplementedException();

        public ArrayList Extensions { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool CaseInsensitive { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string StartComment { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string EndComment { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool Cached { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public char[] TriggersActivators => throw new NotImplementedException();

        public string TriggersActivatorsString => throw new NotImplementedException();

        public int MaxLineHeight => throw new NotImplementedException();

        public int MinLineHeight => throw new NotImplementedException();

        public int MaxCharWidth => throw new NotImplementedException();

        public int MinCharWidth => throw new NotImplementedException();

        public string TabReplaceString => throw new NotImplementedException();

        public bool ShowWhiteSpaces { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ISnippetFormat Add(string formatName)
        {
            throw new NotImplementedException();
        }

        public ISnippetFormat Add(string formatName, ISnippetFormat source)
        {
            throw new NotImplementedException();
        }

        public ISnippetFormat Add(string formatName, string sourceName)
        {
            throw new NotImplementedException();
        }

        public void AddCodeSnippet(string title, ArrayList literals, string code)
        {
            throw new NotImplementedException();
        }

        public void AddCodeSnippet(CodeSnippet snippet)
        {
            throw new NotImplementedException();
        }

        public void AddCodeSnippetsContainer(CodeSnippetsContainer container)
        {
            throw new NotImplementedException();
        }

        public IConfigLexem FindConfig(int iConfigID)
        {
            throw new NotImplementedException();
        }

        public void Remove(ISnippetFormat format)
        {
            throw new NotImplementedException();
        }

        public void Remove(string formatName)
        {
            throw new NotImplementedException();
        }

        public void ResetCaches()
        {
            throw new NotImplementedException();
        }
    }
}