using CodeGenerator.Application.Services;
using CodeGenerator.Application.ViewModels;
using CodeGenerator.Shared;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using Microsoft.DotNet.DesignTools.ViewModels;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Edit;
using Syncfusion.Windows.Forms.Edit.Enums;
using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Tools.XPMenus;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeGenerator.Presentation.WinForms.Views
{
    public partial class ArtifactPreviewView : UserControl, IView<ArtifactPreviewViewModel>
    {
        private ArtifactPreviewViewModel? _viewModel;
        public ArtifactPreviewView()
        {
            InitializeComponent();
            InitializeCustomContextMenu();
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

        private string GetSaveAsFilter()
        {
            // Determine filter based on language
            if (_viewModel == null)
                return "All Files (*.*)|*.*";

            return _viewModel.TextLanguageSchema switch
            {
                ArtifactPreviewViewModel.KnownLanguages.CSharp => "C# Files (*.cs)|*.cs|All Files (*.*)|*.*",
                ArtifactPreviewViewModel.KnownLanguages.VBNET => "VB.NET Files (*.vb)|*.vb|All Files (*.*)|*.*",
                ArtifactPreviewViewModel.KnownLanguages.XML => "XML Files (*.xml)|*.xml|All Files (*.*)|*.*",
                ArtifactPreviewViewModel.KnownLanguages.HTML => "HTML Files (*.html;*.htm)|*.html;*.htm|All Files (*.*)|*.*",
                ArtifactPreviewViewModel.KnownLanguages.JScript => "JavaScript Files (*.js)|*.js|All Files (*.*)|*.*",
                //ArtifactPreviewViewModel.KnownLanguages.Python => "Python Files (*.py)|*.py|All Files (*.*)|*.*",
                ArtifactPreviewViewModel.KnownLanguages.SQL => "SQL Files (*.sql)|*.sql|All Files (*.*)|*.*",
                _ => "All Files (*.*)|*.*",
            };
        }

        private void EditControl_HandleSaveAs()
        {
            // Show SaveFileDialog to save the content
            var filePath = _viewModel?.FilePath;
            var fileService = ServiceProviderHolder.GetRequiredService<IFileSystemDialogService>();
            var initialDirectory = filePath != null ? Path.GetDirectoryName(filePath) : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var defaultFileName = _viewModel?.FileName!=null ? _viewModel.FileName : filePath != null ? Path.GetFileName(filePath) : "Untitled.txt";
            var saveToFilePath = fileService.SaveFile(GetSaveAsFilter(), initialDirectory, defaultFileName);
            if (saveToFilePath != null)
            {
                try
                {
                    File.WriteAllText(saveToFilePath, editControl.Text);
                    if (_viewModel != null) {
                        // Update ViewModel FilePath
                        _viewModel.FilePath = saveToFilePath;
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
            if(_viewModel != null && !string.IsNullOrEmpty(_viewModel.FilePath))
            {
                try
                {
                    File.WriteAllText(_viewModel.FilePath, editControl.Text);
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

        public void BindViewModel(ArtifactPreviewViewModel viewModel)
        {
            // reset controls
            editControl.Text = string.Empty;
            _viewModel = viewModel;
            if (viewModel == null) return;

            if(viewModel.IsTextContent())
            {
                editControl.Visible = true;
                imageBox.Visible = false;
                ShowTextContent(viewModel);
            }
            else if(viewModel.IsImageContent())
            {
                imageBox.Visible = true;
                editControl.Visible = false;
                imageBox.Image = viewModel.ImageContent;
                return;
            } 
            else 
            {
                imageBox.Visible = false;
                editControl.Visible = true;
                editControl.Text = $"Error: Both TextContent and FilePath and ImageContent are null.";
                //throw new InvalidOperationException("Both TextContent and FilePath are null.");
            }
        }

        private void ShowTextContent(ArtifactPreviewViewModel viewModel)
        {
            editControl.Visible = viewModel.TextContent != null || viewModel.FilePath != null;

            // default_language;XML;Pascal;C#;JScript;HTML (Light);Java;C;PowerShell;VB.NET;SQL;VBScript; (Parameter 'name')'
            // map ArtifactPreviewViewModel.KnownLanguages to Syncfusion.Windows.Forms.Edit.Enums.KnownLanguages by name
            try
            {
                KnownLanguages language;
                if (!Enum.TryParse<KnownLanguages>(viewModel.TextLanguageSchema.ToString(), out language) || language == KnownLanguages.Undefined)
                {
                    language = KnownLanguages.Text;
                }

                editControl.ApplyConfiguration(language);
            }
            catch (Exception ex)
            {

                // throw;
            }

            if (viewModel.FilePath != null)
            {
                try
                {
                    editControl.Text = File.ReadAllText(viewModel.FilePath);
                    editControl.FileName = viewModel.FilePath;// Path.GetFileName(viewModel.FilePath);
                }
                catch (Exception ex)
                {
                    // throw;
                }
            }
            else if (viewModel.TextContent != null)
            {

                editControl.Text = viewModel.TextContent ?? string.Empty;
            }
            
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((ArtifactPreviewViewModel)(object)viewModel);
        }
    }
}
