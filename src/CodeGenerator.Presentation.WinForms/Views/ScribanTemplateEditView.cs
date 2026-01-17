using CodeGenerator.Application.ViewModels.Template;
using CodeGenerator.Core.Templates;
using CodeGenerator.Shared;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.TemplateEngines.Scriban;
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

    public ScribanTemplateEditView()
    {
        InitializeComponent();
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
        sb.AppendLine("      <lexem BeginBlock=\"{\" EndBlock=\"{\" Type=\"Operator\" />");
        sb.AppendLine("      <lexem BeginBlock=\"}\" EndBlock=\"}\" Type=\"Operator\" />");
        sb.AppendLine("      <lexem BeginBlock=\"{\" Type=\"Operator\" />");
        sb.AppendLine("      <lexem BeginBlock=\"}\" Type=\"Operator\" />");
        
        // Other operators
        sb.AppendLine("      <lexem BeginBlock=\"|\" Type=\"Operator\" />");
        sb.AppendLine("      <lexem BeginBlock=\".\" Type=\"Operator\" />");
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
        sb.AppendLine("      <lexem BeginBlock=\"##\" EndBlock=\"\\n\" IsEndRegex=\"true\" Type=\"Comment\" IsComplex=\"true\" OnlyLocalSublexems=\"true\" />");
        
        sb.AppendLine("    </lexems>");
        
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
