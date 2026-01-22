using CodeGenerator.Application.Services;
using CodeGenerator.Application.ViewModels.Template;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Templates;
using CodeGenerator.Domain.DataTypes;
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
using Syncfusion.Windows.Forms.Edit.Implementation.Parser;
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
    public bool IsDirty { get; set; } = false;
    public ScribanTemplateEditView()
    {
        InitializeComponent();
        InitializeCustomContextMenu();
        editControl.ContextChoiceBeforeOpen += EditControl_ContextChoiceBeforeOpen;
        editControl.ContextChoiceOpen += EditControl_ContextChoiceOpen;
        editControl.UpdateContextToolTip += EditControl_UpdateContextToolTip;
        editControl.FilterAutoCompleteItems = false;
        editControl.TextChanged += (s, e) => { IsDirty = true; };
        editControl.Closing += EditControl_Closing;
    }

    private void EditControl_Closing(object sender, StreamCloseEventArgs e)
    {
        e.Action = SaveChangesAction.Discard;
        if (IsDirty)
        {
            var messageBoxService = ServiceProviderHolder.GetRequiredService<IMessageBoxService>();
            var result = messageBoxService.AskYesNoCancel("The template has unsaved changes. Do you want to save them before closing?", "Unsaved Changes");
            if (result == MessageBoxResult.Yes)
            {
                EditControl_HandleSave();
            }
            else if (result == MessageBoxResult.Cancel)
            {
                e.Action = SaveChangesAction.Cancel;
            }

        }
            
        
    }

    /// <summary>
    /// Context menu is customized to override the 'Save' & 'Save As' commands.
    /// And to hide the Open command.
    /// </summary>
    public void InitializeCustomContextMenu()
    {
        /*
Edit
- Cut (Ctrl+X)
- Copy (Ctrl+C)
- Paste (Ctrl+V)
- Delete (Del)
- [Seperator]
- Undo (Ctrl+Z)
- Redo (Ctrl+Y)
- [Seperator]
- Find… (Ctrl+F)
- Replace…(Ctrl+H)
- Go to… (Ctrl+G)
- [Seperator]
- Select All (Ctrl+A)
- Delete All

File
- New (Ctrl+N)
- Open (Ctrl+O)
- Close
- [Seperator]
- Save (Ctrl+S)
- Save As... (Ctrl+Shift+S)
- [Seperator]
- Print Preview...
- Print (Ctrl+P)

Advanced
- Tabify Selection
- Untabify Selection
- [Seperator]
- Indent Selection
- Unindent Selection
- [Seperator]
- Comment Selection
- Uncomment Selection
- [Seperator]
- Collapse All
- Expand All

Bookmarks
- Toggle Bookmark (Ctrl+F2)
- Next Bookmark
- Previous Bookmark (Shift+F2)
- Clear Bookmarks

Options...

         */
        // Schakel default contextmenu uit
        editControl.ContextMenuEnabled = false;

        var menu = new ContextMenuStrip();

        // Edit Menu
        var editMenu = new ToolStripMenuItem("Edit");

        var cutMenuItem = new ToolStripMenuItem("Cut", null, (s, e) => editControl.Cut());
        cutMenuItem.ShortcutKeys = Keys.Control | Keys.X;
        cutMenuItem.ShowShortcutKeys = true;
        editMenu.DropDownItems.Add(cutMenuItem);

        var copyMenuItem = new ToolStripMenuItem("Copy", null, (s, e) => editControl.Copy());
        copyMenuItem.ShortcutKeys = Keys.Control | Keys.C;
        copyMenuItem.ShowShortcutKeys = true;
        editMenu.DropDownItems.Add(copyMenuItem);

        var pasteMenuItem = new ToolStripMenuItem("Paste", null, (s, e) => editControl.Paste());
        pasteMenuItem.ShortcutKeys = Keys.Control | Keys.V;
        pasteMenuItem.ShowShortcutKeys = true;
        editMenu.DropDownItems.Add(pasteMenuItem);

        var deleteMenuItem = new ToolStripMenuItem("Delete", null, (s, e) =>
        {
            if (!string.IsNullOrEmpty(editControl.SelectedText))
                editControl.SelectedText = string.Empty;
        });
        deleteMenuItem.ShortcutKeys = Keys.Delete;
        deleteMenuItem.ShowShortcutKeys = true;
        editMenu.DropDownItems.Add(deleteMenuItem);

        editMenu.DropDownItems.Add(new ToolStripSeparator());

        var undoMenuItem = new ToolStripMenuItem("Undo", null, (s, e) => editControl.Undo());
        undoMenuItem.ShortcutKeys = Keys.Control | Keys.Z;
        undoMenuItem.ShowShortcutKeys = true;
        editMenu.DropDownItems.Add(undoMenuItem);

        var redoMenuItem = new ToolStripMenuItem("Redo", null, (s, e) => editControl.Redo());
        redoMenuItem.ShortcutKeys = Keys.Control | Keys.Y;
        redoMenuItem.ShowShortcutKeys = true;
        editMenu.DropDownItems.Add(redoMenuItem);

        editMenu.DropDownItems.Add(new ToolStripSeparator());

        var findMenuItem = new ToolStripMenuItem("Find…", null, (s, e) => editControl.ShowFindDialog());
        findMenuItem.ShortcutKeys = Keys.Control | Keys.F;
        findMenuItem.ShowShortcutKeys = true;
        editMenu.DropDownItems.Add(findMenuItem);

        var replaceMenuItem = new ToolStripMenuItem("Replace…", null, (s, e) => editControl.ShowReplaceDialog());
        replaceMenuItem.ShortcutKeys = Keys.Control | Keys.H;
        replaceMenuItem.ShowShortcutKeys = true;
        editMenu.DropDownItems.Add(replaceMenuItem);

        var gotoMenuItem = new ToolStripMenuItem("Go to…", null, (s, e) => editControl.ShowGoToDialog());
        gotoMenuItem.ShortcutKeys = Keys.Control | Keys.G;
        gotoMenuItem.ShowShortcutKeys = true;
        editMenu.DropDownItems.Add(gotoMenuItem);

        editMenu.DropDownItems.Add(new ToolStripSeparator());

        var selectAllMenuItem = new ToolStripMenuItem("Select All", null, (s, e) => editControl.SelectAll());
        selectAllMenuItem.ShortcutKeys = Keys.Control | Keys.A;
        selectAllMenuItem.ShowShortcutKeys = true;
        editMenu.DropDownItems.Add(selectAllMenuItem);

        var deleteAllMenuItem = new ToolStripMenuItem("Delete All", null, (s, e) => editControl.Text = string.Empty);
        editMenu.DropDownItems.Add(deleteAllMenuItem);

        menu.Items.Add(editMenu);

        // File Menu
        var fileMenu = new ToolStripMenuItem("File");

        //var newMenuItem = new ToolStripMenuItem("New", null, (s, e) => editControl.New());
        //newMenuItem.ShortcutKeys = Keys.Control | Keys.N;
        //newMenuItem.ShowShortcutKeys = true;
        //fileMenu.DropDownItems.Add(newMenuItem);

        //var openMenuItem = new ToolStripMenuItem("Open", null, (s, e) => editControl.LoadFile());
        //openMenuItem.ShortcutKeys = Keys.Control | Keys.O;
        //openMenuItem.ShowShortcutKeys = true;
        //fileMenu.DropDownItems.Add(openMenuItem);

        //var closeMenuItem = new ToolStripMenuItem("Close", null, (s, e) => editControl.Close());
        //fileMenu.DropDownItems.Add(closeMenuItem);

        fileMenu.DropDownItems.Add(new ToolStripSeparator());

        var saveMenuItem = new ToolStripMenuItem("Save", null, (s, e) => EditControl_HandleSave());
        saveMenuItem.ShortcutKeys = Keys.Control | Keys.S;
        saveMenuItem.ShowShortcutKeys = true;
        fileMenu.DropDownItems.Add(saveMenuItem);

        var saveAsMenuItem = new ToolStripMenuItem("Save As...", null, (s, e) => EditControl_HandleSaveAs());
        saveAsMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.S;
        saveAsMenuItem.ShowShortcutKeys = true;
        fileMenu.DropDownItems.Add(saveAsMenuItem);

        fileMenu.DropDownItems.Add(new ToolStripSeparator());

        var printPreviewMenuItem = new ToolStripMenuItem("Print Preview...", null, (s, e) =>
        {
            // Print with preview
            editControl.PrintPreview();
        });
        fileMenu.DropDownItems.Add(printPreviewMenuItem);

        var printMenuItem = new ToolStripMenuItem("Print", null, (s, e) => editControl.Print());
        printMenuItem.ShortcutKeys = Keys.Control | Keys.P;
        printMenuItem.ShowShortcutKeys = true;
        fileMenu.DropDownItems.Add(printMenuItem);

        menu.Items.Add(fileMenu);

        // Advanced Menu
        var advancedMenu = new ToolStripMenuItem("Advanced");

        var tabifyMenuItem = new ToolStripMenuItem("Tabify Selection", null, (s, e) => editControl.TabifySelection());
        advancedMenu.DropDownItems.Add(tabifyMenuItem);

        var untabifyMenuItem = new ToolStripMenuItem("Untabify Selection", null, (s, e) => editControl.UntabifySelection());
        advancedMenu.DropDownItems.Add(untabifyMenuItem);

        advancedMenu.DropDownItems.Add(new ToolStripSeparator());

        var indentMenuItem = new ToolStripMenuItem("Indent Selection", null, (s, e) => editControl.IndentSelection());
        advancedMenu.DropDownItems.Add(indentMenuItem);

        var unindentMenuItem = new ToolStripMenuItem("Unindent Selection", null, (s, e) =>
        {
            ServiceProviderHolder
                .GetRequiredService<IMessageBoxService>()
                .ShowWarning("UnindentSelection is not implemented in Syncfusion EditControl. Implement custom unindent logic here.", "Unindent Selection");
            //MessageBox.Show("UnindentSelection is not implemented in Syncfusion EditControl. Implement custom unindent logic here.", "Unindent Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            // Unindent is the opposite of indent
            //var selection = editControl.Selection;
            //if (selection != null)
            //{
            //    var lines = editControl.GetLines(selection.StartLineNumber, selection.EndLineNumber);
            //    for (int i = selection.StartLineNumber; i <= selection.EndLineNumber; i++)
            //    {
            //        var line = editControl.GetLine(i);
            //        if (line.Text.StartsWith("\t"))
            //            editControl.RemoveText(new Syncfusion.Windows.Forms.Edit.Utils.CoordinatePoint(1, i), 1);
            //        else if (line.Text.StartsWith("    "))
            //            editControl.RemoveText(new Syncfusion.Windows.Forms.Edit.Utils.CoordinatePoint(1, i), 4);
            //    }
            //}
        });
        advancedMenu.DropDownItems.Add(unindentMenuItem);

        advancedMenu.DropDownItems.Add(new ToolStripSeparator());

        var commentMenuItem = new ToolStripMenuItem("Comment Selection", null, (s, e) => editControl.CommentSelection());
        advancedMenu.DropDownItems.Add(commentMenuItem);

        var uncommentMenuItem = new ToolStripMenuItem("Uncomment Selection", null, (s, e) => editControl.UncommentSelection());
        advancedMenu.DropDownItems.Add(uncommentMenuItem);

        advancedMenu.DropDownItems.Add(new ToolStripSeparator());

        var collapseAllMenuItem = new ToolStripMenuItem("Collapse All", null, (s, e) => editControl.CollapseAll());
        advancedMenu.DropDownItems.Add(collapseAllMenuItem);

        var expandAllMenuItem = new ToolStripMenuItem("Expand All", null, (s, e) => editControl.ExpandAll());
        advancedMenu.DropDownItems.Add(expandAllMenuItem);

        menu.Items.Add(advancedMenu);

        // Bookmarks Menu
        var bookmarksMenu = new ToolStripMenuItem("Bookmarks");

        var toggleBookmarkMenuItem = new ToolStripMenuItem("Toggle Bookmark", null, (s, e) =>
        {
            var currentLine = editControl.CurrentLine;
            var bookmarks = editControl.Bookmarks;
            bool hasBookmark = false;

            foreach (IBookmark bookmark in bookmarks)
            {
                if (bookmark.Line == currentLine)
                {
                    hasBookmark = true;
                    editControl.BookmarkRemove(currentLine);
                    break;
                }
            }

            if (!hasBookmark)
            {
                editControl.BookmarkAdd(currentLine);
            }
        });
        toggleBookmarkMenuItem.ShortcutKeys = Keys.Control | Keys.F2;
        toggleBookmarkMenuItem.ShowShortcutKeys = true;
        bookmarksMenu.DropDownItems.Add(toggleBookmarkMenuItem);

        var nextBookmarkMenuItem = new ToolStripMenuItem("Next Bookmark", null, (s, e) =>
        {
            var currentLine = editControl.CurrentLine;
            var bookmarks = editControl.Bookmarks;
            int? nextLine = null;

            foreach (IBookmark bookmark in bookmarks)
            {
                if (bookmark.Line > currentLine && (!nextLine.HasValue || bookmark.Line < nextLine.Value))
                {
                    nextLine = bookmark.Line;
                }
            }

            if (nextLine.HasValue)
            {
                editControl.GoTo(nextLine.Value);
                editControl.CurrentLine = nextLine.Value;
            }
        });
        nextBookmarkMenuItem.ShortcutKeys = Keys.F2;
        nextBookmarkMenuItem.ShowShortcutKeys = true;
        bookmarksMenu.DropDownItems.Add(nextBookmarkMenuItem);

        var previousBookmarkMenuItem = new ToolStripMenuItem("Previous Bookmark", null, (s, e) =>
        {
            var currentLine = editControl.CurrentLine;
            var bookmarks = editControl.Bookmarks;
            int? previousLine = null;

            foreach (IBookmark bookmark in bookmarks)
            {
                if (bookmark.Line < currentLine && (!previousLine.HasValue || bookmark.Line > previousLine.Value))
                {
                    previousLine = bookmark.Line;
                }
            }

            if (previousLine.HasValue)
            {
                editControl.GoTo(previousLine.Value);
                editControl.CurrentLine = previousLine.Value;
            }
        });
        previousBookmarkMenuItem.ShortcutKeys = Keys.Shift | Keys.F2;
        previousBookmarkMenuItem.ShowShortcutKeys = true;
        bookmarksMenu.DropDownItems.Add(previousBookmarkMenuItem);

        var clearBookmarksMenuItem = new ToolStripMenuItem("Clear Bookmarks", null, (s, e) => editControl.BookmarkClear());
        bookmarksMenu.DropDownItems.Add(clearBookmarksMenuItem);

        menu.Items.Add(bookmarksMenu);

        // Options Menu Item
        var optionsMenuItem = new ToolStripMenuItem("Options...", null, (s, e) =>
        {
            editControl.ShowFormatsCustomizationDialog();
            // Show options dialog if available
            //MessageBox.Show("Options dialog not implemented", "Options", MessageBoxButtons.OK, MessageBoxIcon.Information);
        });
        menu.Items.Add(optionsMenuItem);

        editControl.ContextMenuStrip = menu;
    }
    private void EditControl_HandleSaveAs()
    {
        // Show SaveFileDialog to save the content
        var filePath = _viewModel?.TemplateFilePath;
        var fileService = ServiceProviderHolder.GetRequiredService<IFileSystemDialogService>();
        var initialDirectory = filePath != null ? Path.GetDirectoryName(filePath) : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var defaultFileName = Path.GetFileName(filePath) ?? "Untitled.scriban";
        var saveToFilePath = fileService.SaveFile("Scriban Files (*.scriban)|*.scriban|All Files (*.*)|*.*", initialDirectory, defaultFileName);
        if (saveToFilePath != null)
        {
            try
            {
                File.WriteAllText(saveToFilePath, editControl.Text);
                IsDirty = false;
                if (_viewModel != null)
                {
                    // Update ViewModel FilePath
                    _viewModel.TemplateFilePath = saveToFilePath;
                }
            }
            catch (Exception ex)
            {
                var messageBoxService = ServiceProviderHolder.GetRequiredService<IMessageBoxService>();
                messageBoxService.ShowError($"Error saving file: {ex.Message}", "Save Error");
            }
        }


    }

    private void EditControl_HandleSave()
    {
        if (_viewModel != null && !string.IsNullOrEmpty(_viewModel.TemplateFilePath))
        {
            try
            {
                File.WriteAllText(_viewModel.TemplateFilePath, editControl.Text);
                IsDirty = false;
            }
            catch (Exception ex)
            {
                var messageBoxService = ServiceProviderHolder.GetRequiredService<IMessageBoxService>();
                messageBoxService.ShowError($"Error saving file: {ex.Message}", "Save Error");
            }
        }
        else
        {
            // If no FilePath, fallback to Save As
            EditControl_HandleSaveAs();
        }
    }
    private void EditControl_UpdateContextToolTip(object sender, UpdateTooltipEventArgs e)
    {
        if (e.Text == string.Empty)
        {
            Point pointVirtual = editControl.PointToVirtualPosition(new Point(e.X, e.Y));

            if (pointVirtual.Y > 0)
            {
                // Get the current line
                ILexemLine line = editControl.GetLine(pointVirtual.Y);

                if (line != null)
                {
                    // Get tokens from the current line
                    ILexem lexem = line.FindLexemByColumn(pointVirtual.X);

                    if (lexem != null)
                    {
                        IConfigLexem configLexem = lexem.Config as IConfigLexem;

                        if (_viewModel!=null &&_viewModel.TemplateInstance!=null && _viewModel.TemplateInstance.Parameters.ContainsKey(lexem.Text))
                        {
                            try
                            {
                                e.Text = $"Parameter: {_viewModel.TemplateInstance.Parameters[lexem.Text]?.GetType().Name}";
                            }
                            catch (Exception)
                            {

                                //throw;
                            }
                            
                            return;
                        }
                        else if (ScribanIntellisenceSupport.Triggers.TryGetValue(lexem.Text, out var configLexemInfo))
                        {
                            e.Text = configLexemInfo.Tooltip;
                        }
                        else
                        {
                            var lexemBefore = FindLexemBefore(line, lexem, true, true);
                            if (lexemBefore != null) { 
                                if (ScribanIntellisenceSupport.Triggers.TryGetValue(lexemBefore.Text, out var configLexemInfo2))
                                {
                                    var toolTip = configLexemInfo2.Items.FirstOrDefault(i => i.Text == lexem.Text)?.Tooltip;
                                    if (toolTip != null)
                                    {
                                        e.Text = toolTip;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    private ILexem? FindLexemBefore(ILexemLine line, ILexem lexem)
    {
        
        // find lexem before dot
        int lexemIndex = line.LineLexems.IndexOf(lexem);
        if (lexemIndex > 0)
        {
            return line.LineLexems[lexemIndex - 1] as ILexem;
        }
        return null;
    }
    private ILexem? FindLexemBefore(ILexemLine line, ILexem lexem, bool skipDot, bool skipWhitespace)
    {
        var foundLexem = FindLexemBefore(line, lexem);
        if (foundLexem == null) return null;

        while (foundLexem != null && ((skipDot && foundLexem.Text == ".") || (skipWhitespace && string.IsNullOrWhiteSpace(foundLexem.Text))))
        {
            foundLexem = FindLexemBefore(line, foundLexem);
        }
        return foundLexem;
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

    private string GetIconKey(GenericDataType dataType)
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

    private void EditControl_ContextChoiceOpen(IContextChoiceController controller)
    {
        controller.Items.Clear();

        if (controller.LexemBeforeDropper == null || string.IsNullOrWhiteSpace(controller.LexemBeforeDropper.Text))
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
            foreach (var trigger in ScribanIntellisenceSupport.Triggers)
            {
                controller.Items.Add(trigger.TriggerText, trigger.Tooltip??"Built-in Function", GetIcon("square-function"));
            }
            return;
        }

        var triggerText = controller.LexemBeforeDropper.Text;

        if (_viewModel?.TemplateInstance!=null)
        {
            if(_viewModel.TemplateInstance.Parameters.TryGetValue(triggerText, out var param))
            {
                if(param==null)
                    return; // weird situation, should not get here
                
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
        }

        if(ScribanIntellisenceSupport.Triggers.TryGetValue(triggerText, out var trigger2))
        {
            foreach(var item in trigger2.Items)
            {
                controller.Items.Add(item.Text, item.Tooltip??"Built-in Function", GetIcon("square-function"));
            }
        }
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
            IsDirty = false;
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
                IsDirty = false;
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
            //var parameters = new Dictionary<string, object?>();
            //var functions = new Dictionary<string, Delegate>();
            //if(_viewModel!=null && _viewModel.TemplateInstance!=null)
            //{
            //    parameters = _viewModel.TemplateInstance.Parameters;
            //    functions = _viewModel.TemplateInstance.Functions;
            //}
            //var scribanConfig = new ScribanLanguageConfig(parameters, functions, _buildinfunctions);

            //editControl.ApplyConfiguration(scribanConfig);





            //// Generate XML configuration for Scriban syntax
            //var xmlConfig = GenerateScribanXmlConfig();

            //// Save to temp file
            //_tempConfigPath = Path.Combine(Path.GetTempPath(), $"scriban_config_{Guid.NewGuid():N}.xml");
            //Debug.WriteLine($"Scriban Syntax Config file: {_tempConfigPath}");
            //File.WriteAllText(_tempConfigPath, xmlConfig);
            //editControl.Configurator.Open(_tempConfigPath);
            
            var scribanConfig = new EditControlScribanXmlConfig();
            
            var configStream = scribanConfig.GenerateScribanXmlConfigStream(_viewModel?.TemplateInstance);
            // Load configuration from XML
            editControl.Configurator.Open(configStream);

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
            Debug.WriteLine($"Known languages: {string.Join(", ", editControl.Configurator.KnownLanguageNames.OfType<string>())}");
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
        sb.AppendLine("      <lexem BeginBlock=\"##\" EndBlock=\"\\n\" IsEndRegex=\"true\" Type=\"Comment\" />");
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
    //public void SaveFile()
    //{
    //    if (_viewModel == null || string.IsNullOrEmpty(_viewModel.TemplateFilePath))
    //        return;

    //    try
    //    {
    //        editControl.SaveFile(_viewModel.TemplateFilePath);
    //    }
    //    catch (Exception ex)
    //    {
    //        MessageBox.Show($"Error saving file: {ex.Message}", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    //    }
    //}

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
