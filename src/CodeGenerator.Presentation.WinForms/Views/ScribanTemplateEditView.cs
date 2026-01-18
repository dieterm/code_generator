using CodeGenerator.Application.ViewModels.Template;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Templates;
using CodeGenerator.Domain.NamingConventions;
using CodeGenerator.Shared;
using CodeGenerator.Shared.ExtensionMethods;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.TemplateEngines.Scriban;
using DocumentFormat.OpenXml.Spreadsheet;
using Scriban.Functions;
using Scriban.Runtime;
using Scriban.Syntax;
using Syncfusion.Windows.Forms.Edit;
using Syncfusion.Windows.Forms.Edit.Enums;
using Syncfusion.Windows.Forms.Edit.Interfaces;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Xml;

namespace CodeGenerator.Presentation.WinForms.Views;

/// <summary>
/// View for editing a Scriban template file using Syncfusion EditControl
/// </summary>
public partial class ScribanTemplateEditView : UserControl, IView<ScribanTemplateEditViewModel>
{
    private ScribanTemplateEditViewModel? _viewModel;
    private string? _tempConfigPath;
    private readonly ScriptObject _buildinfunctions = new global::Scriban.Functions.BuiltinFunctions();
    public ScribanTemplateEditView()
    {
        InitializeComponent();
        editControl.ContextChoiceBeforeOpen += EditControl_ContextChoiceBeforeOpen;
        editControl.ContextChoiceOpen += EditControl_ContextChoiceOpen;
        editControl.FilterAutoCompleteItems = false;
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    private void GetScribanBuildinFunctions()
    {
        
        var functions = new List<object>();
        foreach (var (member, memberObj) in _buildinfunctions)
        {
            Debug.WriteLine($"Member: {member} - Type: {memberObj.GetType().Name}: {memberObj.ToString()}");
            if (memberObj is ScriptObject scriptObj)
            {
                foreach (var (functionName, functionBody) in scriptObj)
                {

                    Debug.WriteLine($"  Function: {functionName} - Body Type: {functionBody.GetType().Name}");
                    var funcbod = functionBody as IScriptCustomFunction;
                    if (funcbod != null)
                    {
                        for (int i = 0; i < funcbod.ParameterCount; i++)
                        {
                            var paparamInfo = funcbod.GetParameterInfo(i);
                            Debug.WriteLine($"    Param {i}: {paparamInfo.Name} Type: {paparamInfo.ParameterType} DefaultValue: {paparamInfo.DefaultValue}");
                        }
                    }
                }
            }
            else if (memberObj is EmptyScriptObject emptyScriptObject)
            {
                // eg: "empty", "blank"
                //Debug.WriteLine($"  Custom Function: {member} - Parameter Count: {emptyScriptObject.}");

            }
            else if (memberObj is IScriptCustomFunction includeFunction)
            {
                // eg: "include" 
                Debug.WriteLine($"  Include Function: {member} - Parameter Count: {includeFunction.ParameterCount}");
                for (int i = 0; i < includeFunction.ParameterCount; i++)
                {
                    var paparamInfo = includeFunction.GetParameterInfo(i);
                    Debug.WriteLine($"    Param {i}: {paparamInfo.Name} Type: {paparamInfo.ParameterType} DefaultValue: {paparamInfo.DefaultValue}");
                }
            }
        }
    }

    private void EditControl_ContextChoiceOpen(IContextChoiceController controller)
    {
        controller.Items.Clear();
        if (controller.LexemBeforeDropper == null)
        {
            // show all parameters, functions and buildin functions
            if(_viewModel?.TemplateInstance!=null)
            {
                // parameters
                foreach(var paramKvp in _viewModel.TemplateInstance.Parameters)
                {
                    controller.Items.Add(paramKvp.Key, "Parameter", GetIcon("box"));
                }
                // functions
                foreach(var funcKvp in _viewModel.TemplateInstance.Functions)
                {
                    controller.Items.Add(funcKvp.Key, "Function", GetIcon("square-function"));
                }
            }

            // buildin functions
            foreach (var (member, memberObj) in _buildinfunctions)
            {
                controller.Items.Add(NamingConventions.Convert(member, NamingStyle.SnakeCase), "Built-in Function", GetIcon("square-function"));
            }
            return;
        }
        var triggerText = controller.LexemBeforeDropper.Text;
        if (controller.LexemBeforeDropper != null && _viewModel.TemplateInstance!=null)
        {
            if(_viewModel.TemplateInstance.Parameters.TryGetValue(triggerText, out var param))
            {
                if(param==null)
                    return;
                
                var paramType = param.GetType();
                foreach(var prop in paramType.GetProperties())
                {
                    controller.Items.Add(NamingConventions.Convert(prop.Name, NamingStyle.SnakeCase), prop.PropertyType.Name, GetIcon("box"));
                }

                foreach(var method in paramType.GetMethods().Where(m=>m.DeclaringType!=typeof(object)))
                {
                    controller.Items.Add(NamingConventions.Convert(method.Name, NamingStyle.SnakeCase)+"()", method.ReturnType.Name, GetIcon("square-function"));
                }

                return;
            }
            else if (_viewModel.TemplateInstance.Functions.ContainsKey(triggerText))
            {
                var del = _viewModel.TemplateInstance.Functions[triggerText];
                if (del == null)
                    return;
                // TODO: provide more meaningfull item
                del.Method.GetParameters().Select(p => p.Name).ToList().ForEach(p=>
                {
                    controller.Items.Add(NamingConventions.Convert(p, NamingStyle.SnakeCase), "Parameter", GetIcon("parameter"));
                });
            }
            else if (_buildinfunctions.ContainsKey(triggerText))
            {
                var memberObj = _buildinfunctions[triggerText];
                if (memberObj is ScriptObject scriptObj)
                {
                    foreach (var (functionName, functionBody) in scriptObj)
                    {
                        
                        //Debug.WriteLine($"  Function: {functionName} - Body Type: {functionBody.GetType().Name}");
                        var funcbod = functionBody as IScriptCustomFunction;
                        if (funcbod != null)
                        {
                            StringBuilder paramList = new StringBuilder();
                            for (int i = 0; i < funcbod.ParameterCount; i++)
                            {
                                var paparamInfo = funcbod.GetParameterInfo(i);
                                paramList.Append($"<{paparamInfo.Name}> ");
                                //Debug.WriteLine($"    Param {i}: {paparamInfo.Name} Type: {paparamInfo.ParameterType} DefaultValue: {paparamInfo.DefaultValue}");
                            }
                            controller.Items.Add(functionName, $"{functionName}({paramList.ToString().Trim()})", GetIcon("square-function"));
                        }
                        else
                        {
                            controller.Items.Add(functionName, functionName, GetIcon("square-function"));
                            Debug.WriteLine($"    Not a custom function");
                        }
                    }
                }
                else if (memberObj is EmptyScriptObject emptyScriptObject)
                {
                    // eg: "empty", "blank"
                    //Debug.WriteLine($"  Custom Function: {member} - Parameter Count: {emptyScriptObject.}");

                }
                else if (memberObj is IScriptCustomFunction includeFunction)
                {
                    // eg: "include" 
                    //Debug.WriteLine($"  Include Function: {member} - Parameter Count: {includeFunction.ParameterCount}");
                    //for (int i = 0; i < includeFunction.ParameterCount; i++)
                    //{
                    //    var paparamInfo = includeFunction.GetParameterInfo(i);
                    //    //Debug.WriteLine($"    Param {i}: {paparamInfo.Name} Type: {paparamInfo.ParameterType} DefaultValue: {paparamInfo.DefaultValue}");
                    //}
                }
            }
        }
        //Debug.WriteLine(controller.LexemBeforeDropper);
        //controller.Items.Add("data", "This is data");
        //controller.Items.Add("columns", "This is columns");
        //controller.Items.Add("target", "This is target");
        //controller.Items.Add("datasource", "This is datasource");
        //controller.Items.Add("Database", "This is Database", this.editControl1.ContextChoiceController.Images["Image1"]);
        //controller.Items.Add("NewFile", "This is NewFile", this.editControl1.ContextChoiceController.Images["Image2"]);
        //controller.Items.Add("Find", "This is Find", this.editControl1.ContextChoiceController.Images["Image3"]);
        //controller.Items.Add("Home", "This is Home", this.editControl1.ContextChoiceController.Images["Image4"]);
        //controller.Items.Add("PieChart", "This is PieChart", this.editControl1.ContextChoiceController.Images["Image6"]);
        //controller.Items.Add("Tools", "This is Tools", this.editControl1.ContextChoiceController.Images["Image7"]);
        
    }

    private INamedImage GetIcon(string imageKey)
    {
        var image = editControl.ContextChoiceController.Images[imageKey];
        if (image != null) return image;

        // if image not exists yet, get it from resource manager and add to imagelist
        var service = ServiceProviderHolder.GetService<ITreeNodeIconResolver<ResourceManagerTreeNodeIcon>>(); 
        var icon = service?.ResolveIcon(new ResourceManagerTreeNodeIcon(imageKey));
        if(icon == null)
            throw new InvalidOperationException($"Icon '{imageKey}' could not be resolved.");
        editControl.ContextChoiceController.Images.AddImage(imageKey, icon.ToIcon().ToBitmap());
        
        return editControl.ContextChoiceController.Images[imageKey];
    }

    private void EditControl_ContextChoiceBeforeOpen(object? sender, CancelEventArgs e)
    {
        if(_viewModel==null || _viewModel.TemplateInstance==null)
        {
            e.Cancel = true;
            return;
        }

        // Get current caret position
        Point caretPosition = editControl.CurrentPosition;
        int currentLine = caretPosition.Y;
        int currentColumn = caretPosition.X;

        // Get the lexem line for the current line
        ILexemLine lexemLine = editControl.GetLine(currentLine);

        if (lexemLine == null || lexemLine.LineLexems.Count == 0)
        {
            //e.Cancel = true;
            return;
        }

        // Find the lexem at or before the current column position
        ILexem? lexemBeforeCaret = null;
        ILexem? triggerLexem = null; // The dot or trigger character

        for (int i = 0; i < lexemLine.LineLexems.Count; i++)
        {
            ILexem lex = lexemLine.LineLexems[i] as ILexem;

            // Check if this lexem ends at or before the caret position
            int lexemEnd = lex.Column + lex.Length;

            if (lexemEnd <= currentColumn)
            {
                // This is a potential candidate - check if it's the trigger (dot)
                if (lex.Text == ".")
                {
                    triggerLexem = lex;
                    // The lexem before the dot is what we need
                    if (i > 0)
                    {
                        lexemBeforeCaret = lexemLine.LineLexems[i - 1] as ILexem;
                    }
                }
                else if (triggerLexem == null)
                {
                    // Keep track of the last non-dot lexem before caret
                    lexemBeforeCaret = lex;
                }
            }
            else
            {
                break; // We've passed the caret position
            }
        }

        // Alternative: use the last lexem before the dot
        if (triggerLexem == null)
        {
            // No dot found, cancel the context choice
            //e.Cancel = true;
            return;
        }

        if (lexemBeforeCaret == null)
        {
            //e.Cancel = true;
            return;
        }

        // Check if the lexem before the dot is a valid parameter
        if (_viewModel.TemplateInstance.Parameters.TryGetValue(lexemBeforeCaret.Text, out var param))
        {
            e.Cancel = false;
        }
        else if(_viewModel.TemplateInstance.Functions.ContainsKey(lexemBeforeCaret.Text))
        {
            e.Cancel = false;
        }
        else if (_buildinfunctions.ContainsKey(lexemBeforeCaret.Text))
        {
            e.Cancel = false;
        }
        else
        {
            //e.Cancel = true;
        }
    }
    // Returns the last index of the context choice character - '.' in the current line
    private int GetContextChoiceCharIndex(ILexemLine line)
    {
        int lastPos = -1;

        for (int i = 0; i < line.LineLexems.Count; i++)
        {
            ILexem lex = line.LineLexems[i] as ILexem;

            if (lex.Text == ".")
                lastPos = i;
        }

        return lastPos;
    }
    public void BindViewModel(ScribanTemplateEditViewModel viewModel)
    {
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        _viewModel = viewModel;

        if (_viewModel != null)
        {
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            LoadTemplateFile();
        }
    }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
    {
        BindViewModel((ScribanTemplateEditViewModel)(object)viewModel);
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (InvokeRequired)
        {
            Invoke(() => ViewModel_PropertyChanged(sender, e));
            return;
        }

        if (e.PropertyName == nameof(ScribanTemplateEditViewModel.TemplateFilePath))
        {
            LoadTemplateFile();
        }
    }

    private void LoadTemplateFile()
    {
        if (_viewModel == null || string.IsNullOrEmpty(_viewModel.TemplateFilePath))
        {
            editControl.Text = string.Empty;
            return;
        }

        if (File.Exists(_viewModel.TemplateFilePath))
        {
            try
            {
                // First apply syntax highlighting configuration
                InitializeSyntaxHighlighting();
                
                // Read the file content and set it directly (instead of LoadFile)
                // This ensures our configuration is used instead of auto-detection by extension
                var content = File.ReadAllText(_viewModel.TemplateFilePath);
                editControl.Text = content;
            }
            catch (Exception ex)
            {
                editControl.Text = $"Error loading file: {ex.Message}";
            }
        }
        else
        {
            editControl.Text = $"File not found: {_viewModel.TemplateFilePath}";
        }
    }

    private void InitializeSyntaxHighlighting()
    {
        try
        {
            // Generate XML configuration for Scriban syntax
            var xmlConfig = GenerateScribanXmlConfig();
            
            // Save to temp file
            _tempConfigPath = Path.Combine(Path.GetTempPath(), $"scriban_config_{Guid.NewGuid():N}.xml");
            Debug.WriteLine($"Scriban Syntax Config file: {_tempConfigPath}");
            File.WriteAllText(_tempConfigPath, xmlConfig);

            // Load configuration from XML
            editControl.Configurator.Open(_tempConfigPath);
            
            // Apply the Scriban configuration
            editControl.ApplyConfiguration("Scriban");
            
            // add code snippets
            var snippet = new Syncfusion.Windows.Forms.Edit.Utils.CodeSnippets.CodeSnippet();
            snippet.Literals.Add(new Syncfusion.Windows.Forms.Edit.Utils.CodeSnippets.Literal { ID = "ifdef", Default="id", ToolTip = "if condition" });
            snippet.Code = "{{ if $ifdef$ }}\n\t${cursor}\n{{ end }}$end$";
            snippet.Author = "CodeGenerator";
            snippet.Description = "Scriban if statement";
            snippet.Title = "Scriban if";
            snippet.Language = "Scriban";
            snippet.Shortcut = "sif";

            editControl.Language.SnippetsContainer.AddSnippet(snippet);
            Debug.WriteLine($"Known languages: {string.Join(", ", editControl.Configurator.KnownLanguageNames)}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing syntax highlighting: {ex.Message}");
            // Fallback: just show plain text
        }
    }

    private string GenerateScribanXmlConfig()
    {
        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        sb.AppendLine("<ArrayOfConfigLanguage>");
        sb.AppendLine("  <ConfigLanguage name=\"Scriban\">");
        
        // Formats
        sb.AppendLine("    <formats>");
        sb.AppendLine("      <format name=\"Text\" Font=\"Consolas, 10pt\" FontColor=\"Black\" />");
        sb.AppendLine("      <format name=\"KeyWord\" Font=\"Consolas, 10pt, style=Bold\" FontColor=\"Blue\" />");
        sb.AppendLine("      <format name=\"String\" Font=\"Consolas, 10pt\" FontColor=\"#A31515\" />");
        sb.AppendLine("      <format name=\"Comment\" Font=\"Consolas, 10pt, style=Italic\" FontColor=\"Green\" />");
        sb.AppendLine("      <format name=\"Operator\" Font=\"Consolas, 10pt, style=Bold\" FontColor=\"#FF6600\" />");
        sb.AppendLine("      <format name=\"Parameter\" Font=\"Consolas, 10pt\" FontColor=\"Purple\" />");
        sb.AppendLine("      <format name=\"Function\" Font=\"Consolas, 10pt\" FontColor=\"#8B4513\" />");
        sb.AppendLine("      <format name=\"Number\" Font=\"Consolas, 10pt\" FontColor=\"DarkCyan\" />");
        sb.AppendLine("    </formats>");
        
        // Extensions
        sb.AppendLine("    <extensions>");
        sb.AppendLine("      <extension>scriban</extension>");
        sb.AppendLine("    </extensions>");
        
        // Lexems
        sb.AppendLine("    <lexems>");

        // Scriban delimiters - using BeginBlock + EndBlock pattern for {{ and }}
        //sb.AppendLine("      <lexem BeginBlock=\"{{\" EndBlock=\"}}\" Type=\"Operator\" OnlyLocalSublexems=\"true\" IsComplex=\"true\">");
        //sb.AppendLine("        <SubLexems>");
        //sb.AppendLine("          <lexem BeginBlock=\"\\n\" IsBeginRegex=\"true\" />");
        //sb.AppendLine("        </SubLexems>");
        //sb.AppendLine("      </lexem>");

        sb.AppendLine("      <lexem BeginBlock=\"{\" EndBlock=\"{\" Type=\"Operator\" />");
        sb.AppendLine("      <lexem BeginBlock=\"}\" EndBlock=\"}\" Type=\"Operator\" />");
        //sb.AppendLine("      <lexem BeginBlock=\"{\" Type=\"Operator\" />");
        //sb.AppendLine("      <lexem BeginBlock=\"}\" Type=\"Operator\" />");

        //sb.AppendLine("      <lexem BeginBlock=\"{{\" Type=\"Operator\" />");
        //sb.AppendLine("      <lexem EndBlock=\"}}\" Type=\"Operator\" />");

        // Other operators
        sb.AppendLine("      <lexem BeginBlock=\"|\" Type=\"Operator\" />");
        sb.AppendLine("      <lexem BeginBlock=\".\" Type=\"Operator\" DropContextChoiceList=\"true\" />");
        sb.AppendLine("      <lexem BeginBlock=\"=\" EndBlock=\"=\" Type=\"Operator\" />");
        sb.AppendLine("      <lexem BeginBlock=\"!\" EndBlock=\"=\" Type=\"Operator\" />");
        sb.AppendLine("      <lexem BeginBlock=\"&lt;\" EndBlock=\"=\" Type=\"Operator\" />");
        sb.AppendLine("      <lexem BeginBlock=\"&gt;\" EndBlock=\"=\" Type=\"Operator\" />");
        sb.AppendLine("      <lexem BeginBlock=\"&lt;\" Type=\"Operator\" />");
        sb.AppendLine("      <lexem BeginBlock=\"&gt;\" Type=\"Operator\" />");
        sb.AppendLine("      <lexem BeginBlock=\"&amp;\" EndBlock=\"&amp;\" Type=\"Operator\" />");
        sb.AppendLine("      <lexem BeginBlock=\"|\" EndBlock=\"|\" Type=\"Operator\" />");
        sb.AppendLine("      <lexem BeginBlock=\"?\" EndBlock=\"?\" Type=\"Operator\" />");
        sb.AppendLine("      <lexem BeginBlock=\"+\" Type=\"Operator\" />");
        sb.AppendLine("      <lexem BeginBlock=\"-\" Type=\"Operator\" />");
        sb.AppendLine("      <lexem BeginBlock=\"*\" Type=\"Operator\" />");
        sb.AppendLine("      <lexem BeginBlock=\"/\" Type=\"Operator\" />");
        sb.AppendLine("      <lexem BeginBlock=\"%\" Type=\"Operator\" />");
        sb.AppendLine("      <lexem BeginBlock=\"!\" Type=\"Operator\" />");
        sb.AppendLine("      <lexem BeginBlock=\"(\" Type=\"Operator\" />");
        sb.AppendLine("      <lexem BeginBlock=\")\" Type=\"Operator\" />");
        sb.AppendLine("      <lexem BeginBlock=\"[\" Type=\"Operator\" />");
        sb.AppendLine("      <lexem BeginBlock=\"]\" Type=\"Operator\" />");
        sb.AppendLine("      <lexem BeginBlock=\",\" Type=\"Operator\" />");
        sb.AppendLine("      <lexem BeginBlock=\":\" Type=\"Operator\" />");
        sb.AppendLine("      <lexem BeginBlock=\";\" Type=\"Operator\" />");
        
        // Scriban keywords
        var keywords = new[] { 
            "if", "else", "elseif", "end", "for", "in", "while", "break", 
            "continue", "func", "ret", "capture", "readonly", "import", 
            "with", "wrap", "include", "true", "false", "null", "empty", 
            "blank", "this", "tablerow", "case", "when"
        };
        foreach (var keyword in keywords)
        {
            sb.AppendLine($"      <lexem BeginBlock=\"{keyword}\" Type=\"KeyWord\" />");
        }
        
        // Add template parameters if available
        if (_viewModel?.TemplateInstance?.Parameters != null)
        {
            foreach (var param in _viewModel.TemplateInstance.Parameters)
            {
                sb.AppendLine($"      <lexem BeginBlock=\"{EscapeXml(param.Key)}\" Type=\"Custom\" FormatName=\"Parameter\" />");
            }
        }
        
        // Add global functions if available
        var engineManager = ServiceProviderHolder.GetService<TemplateEngineManager>();
        var scribanEngine = engineManager?.TemplateEngines.OfType<ScribanTemplateEngine>().FirstOrDefault();
        if (scribanEngine?.GlobalFunctions != null)
        {
            foreach (var func in scribanEngine.GlobalFunctions)
            {
                sb.AppendLine($"      <lexem BeginBlock=\"{EscapeXml(func.Key)}\" Type=\"Custom\" FormatName=\"Function\" />");
            }
        }
        
        // Built-in Scriban functions
        var builtinFunctions = new[] { 
            "size", "first", "last", "join", "split", "reverse", "sort", "uniq", 
            "map", "contains", "upcase", "downcase", "capitalize", "strip", 
            "lstrip", "rstrip", "slice", "truncate", "truncatewords", "replace", 
            "replace_first", "remove", "remove_first", "append", "prepend", 
            "strip_html", "strip_newlines", "escape", "url_encode", "url_decode",
            "date", "math", "string", "array", "object", "regex", "html", "timespan"
        };
        foreach (var func in builtinFunctions)
        {
            sb.AppendLine($"      <lexem BeginBlock=\"{func}\" Type=\"Custom\" FormatName=\"Function\" />");
        }
        
        // Numbers
        sb.AppendLine("      <lexem BeginBlock=\"[0-9]+\" IsBeginRegex=\"true\" Type=\"Number\" />");
        sb.AppendLine("      <lexem BeginBlock=\"[0-9]+\" ContinueBlock=\".\" EndBlock=\"[0-9]+\" IsBeginRegex=\"true\" IsEndRegex=\"true\" Type=\"Number\" />");
        
        // Strings
        sb.AppendLine("      <lexem BeginBlock=\"&quot;\" EndBlock=\"&quot;\" Type=\"String\" IsComplex=\"true\" OnlyLocalSublexems=\"true\">");
        sb.AppendLine("        <SubLexems>");
        sb.AppendLine("          <lexem BeginBlock=\"\\\" EndBlock=\".\" IsEndRegex=\"true\" Type=\"String\" />");
        sb.AppendLine("        </SubLexems>");
        sb.AppendLine("      </lexem>");
        sb.AppendLine("      <lexem BeginBlock=\"'\" EndBlock=\"'\" Type=\"String\" IsComplex=\"true\" OnlyLocalSublexems=\"true\">");
        sb.AppendLine("        <SubLexems>");
        sb.AppendLine("          <lexem BeginBlock=\"\\\" EndBlock=\".\" IsEndRegex=\"true\" Type=\"String\" />");
        sb.AppendLine("        </SubLexems>");
        sb.AppendLine("      </lexem>");
        
        // Comments - Scriban uses ## for line comments
        sb.AppendLine("      <lexem BeginBlock=\"#\" EndBlock=\"#\" Type=\"Comment\" />");
        //sb.AppendLine("      <lexem BeginBlock=\"##\" EndBlock=\"\\n\" IsEndRegex=\"true\" Type=\"Comment\" IsComplex=\"true\" />");
        //sb.AppendLine("      <lexem BeginBlock=\"/*\" EndBlock=\"*/\" Type=\"Comment\" IsComplex=\"true\" IsCollapsable=\"true\" CollapseName=\"/*...*/\" />");
        //sb.AppendLine("         <SubLexems>");
        //sb.AppendLine("           <lexem BeginBlock=\"\\n\" IsBeginRegex=\"true\" />");
        //sb.AppendLine("         </SubLexems>");
        //sb.AppendLine("      </lexem>");
        sb.AppendLine("    </lexems>");
        /*
         <splits>
  <split>#include</split>
</splits>
         */
        //sb.AppendLine("     <splits>");
        //sb.AppendLine("         <split>{{</split>");
        //sb.AppendLine("         <split>}}</split>");
        //sb.AppendLine("     </splits>");
        sb.AppendLine("  </ConfigLanguage>");
        sb.AppendLine("</ArrayOfConfigLanguage>");
        
        return sb.ToString();
    }

    private void WriteScribanBlockSubLexems(StringBuilder sb)
    {
        // Temporarily disabled for testing

    }

    private string EscapeXml(string value)
    {
        return value
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&apos;");
    }

    /// <summary>
    /// Save the current content back to the file
    /// </summary>
    public void SaveFile()
    {
        if (_viewModel == null || string.IsNullOrEmpty(_viewModel.TemplateFilePath))
            return;

        try
        {
            editControl.SaveFile(_viewModel.TemplateFilePath);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving file: {ex.Message}", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Clean up temporary configuration file
    /// </summary>
    private void CleanupTempConfigFile()
    {
        if (!string.IsNullOrEmpty(_tempConfigPath) && File.Exists(_tempConfigPath))
        {
            try { File.Delete(_tempConfigPath); } catch { }
        }
    }
}
