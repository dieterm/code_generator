using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Controllers.Template;
using CodeGenerator.Application.Events.Application;
using CodeGenerator.Application.Services;
using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Core.MessageBus;
using CodeGenerator.Core.Settings.Application;
using CodeGenerator.Core.Templates;
using CodeGenerator.Core.Workspaces.MessageBus;
using CodeGenerator.Domain.DotNet;
using CodeGenerator.Domain.DotNet.ProjectType;
using CodeGenerator.Presentation.WinForms.ViewModels;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Ribbon;
using CodeGenerator.TemplateEngines.DotNetProject.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Application.Controllers.Workspace
{
    public class WorkspaceController : CoreControllerBase
    {
        public const string CodeGeneratorFileDialogFilter = "CodeGenerator Workspace Files (*.codegenerator)|*.codegenerator|All Files (*.*)|*.*";

        private readonly WorkspaceRibbonViewModel _workspaceRibbonViewModel;
        private readonly WorkspaceTreeViewController _workspaceTreeViewController;
        private readonly WorkspaceMessageBus _workspaceMessageBus;
        public bool HasUnsavedChanges { get {return _workspaceTreeViewController.HasUnsavedChanges; } }

        public WorkspaceController(WorkspaceMessageBus workspaceMessageBus, WorkspaceTreeViewController workspaceTreeViewController, WorkspaceRibbonViewModel workspaceRibbonViewModel, IWindowManagerService windowManagerService, RibbonBuilder ribbonBuilder, ApplicationMessageBus messageBus, IMessageBoxService messageboxService, IFileSystemDialogService fileSystemDialogService, ILogger<WorkspaceController> logger) 
            : base(windowManagerService, ribbonBuilder, messageBus, messageboxService, fileSystemDialogService, logger)
        {
            _workspaceMessageBus = workspaceMessageBus;
            _workspaceRibbonViewModel = workspaceRibbonViewModel;
            _workspaceTreeViewController = workspaceTreeViewController;
        }

        public override void Initialize()
        {
            _workspaceMessageBus.Initialize();
            _workspaceTreeViewController.Initialize();
            _workspaceRibbonViewModel.RequestCloseWorkspace += OnRequestCloseWorkspace;
            _workspaceRibbonViewModel.RequestSaveWorkspace += OnRequestSaveWorkspace;
            _workspaceRibbonViewModel.RequestOpenWorkspace += OnRequestOpenWorkspace;
            _workspaceRibbonViewModel.RequestNewWorkspace += OnRequestNewWorkspace;
            _workspaceRibbonViewModel.RequestShowTemplates += OnRequestShowTemplates;
            _workspaceRibbonViewModel.RequestUndo += OnRequestUndo;
            _workspaceRibbonViewModel.RequestRedo += OnRequestRedo;
        }

        private async void OnRequestRedo(object? sender, EventArgs e)
        {
            if(_workspaceTreeViewController.CurrentWorkspace==null)
                return;
            var dotNetProjectService = ServiceProviderHolder.GetRequiredService<DotNetProjectService>();
            var workspaceOutputDirectory = _workspaceTreeViewController.CurrentWorkspace.OutputDirectory;

            var supportedLanguages = new Dictionary<string, List<DotNetLanguage>>();
            var supportedFrameworks = new Dictionary<string, List<TargetFramework>>();
            var totalCombinations = DotNetProjectTypes.AllTypes.Length * TargetFrameworks.AllFrameworks.Length;// * DotNetLanguages.AllLanguages.Count;
            var counter = 0;
            foreach (var projectType in DotNetProjectTypes.AllTypes)
            {
                supportedLanguages.Add(projectType.Id, new List<DotNetLanguage>());
                supportedFrameworks.Add(projectType.Id, new List<TargetFramework>());
                
                foreach (var targetFramework in projectType.SupportedFrameworks)
                {
                    counter++;
                    _logger.LogInformation("Creating project {Counter}/{Total} : {ProjectType} {TargetFramework}", counter, totalCombinations, projectType.Id, targetFramework);
                    var language = DotNetLanguages.CSharp;
                    
                    var folderName = $"{projectType.Id}.{targetFramework.Id}.{language.Id}";
                    var projectDirectory = System.IO.Path.Combine(workspaceOutputDirectory, folderName);
                    try
                    {
                        _logger.LogInformation(projectDirectory);
                        Directory.CreateDirectory(projectDirectory);
                        await dotNetProjectService.CreateProjectAsync("ProjectXYZ", projectDirectory, projectType.Id, targetFramework.DotNetCommandLineArgument, language.DotNetCommandLineArgument);

                        supportedLanguages[projectType.Id].Add(language);
                        supportedFrameworks[projectType.Id].Add(targetFramework);
                    }
                    catch (Exception ex)
                    {
                        if(Directory.Exists(projectDirectory))
                            Directory.Delete(projectDirectory);

                        _logger.LogError(ex, "Failed to create project {ProjectType} {TargetFramework} {Language}", projectType.Id, targetFramework, language.DotNetCommandLineArgument);
                    }
                }
            }

            var languageLookup = new Dictionary<DotNetLanguage, string>
            {
                { DotNetLanguages.CSharp, "DotNetLanguages.CSharp" },
                { DotNetLanguages.FSharp, "DotNetLanguages.FSharp" },
                { DotNetLanguages.VisualBasic, "DotNetLanguages.VisualBasic" }
            };

            var frameworkLookup = new Dictionary<TargetFramework, string>
            {
                { TargetFrameworks.Net10, "TargetFrameworks.Net10" },
                { TargetFrameworks.Net9, "TargetFrameworks.Net9" },
                { TargetFrameworks.Net8, "TargetFrameworks.Net8" },
                { TargetFrameworks.Net7, "TargetFrameworks.Net7" },
                { TargetFrameworks.Net6, "TargetFrameworks.Net6" },
                { TargetFrameworks.Net5, "TargetFrameworks.Net5" },
                { TargetFrameworks.NetStandard21, "TargetFrameworks.NetStandard21" },
                { TargetFrameworks.NetStandard20, "TargetFrameworks.NetStandard20" },
                { TargetFrameworks.NetCore31, "TargetFrameworks.NetCore31" },
                { TargetFrameworks.Net48, "TargetFrameworks.Net48" },
                { TargetFrameworks.Net472, "TargetFrameworks.Net472" }
            };

            StringBuilder sb = new StringBuilder();
            foreach (var projectType in supportedLanguages.Keys)
            {
                sb.AppendLine("ProjectTypeSupportedLanguages[ClassLib] = new[] {");
               
                foreach (var language in supportedLanguages[projectType].Distinct())
                {
                    sb.AppendLine (languageLookup[language] + ",");
                }
                sb.AppendLine("};");
            }
            foreach (var projectType in supportedFrameworks.Keys)
            {
                sb.AppendLine("ProjectTypeSupportedFrameworks[ClassLib] = new[] {");
                foreach (var framework in supportedFrameworks[projectType].Distinct())
                {
                    sb.AppendLine (frameworkLookup[framework] + ",");
                }
                sb.AppendLine("};");
            }
            Debug.WriteLine(sb.ToString());
        }

        private void OnRequestUndo(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnRequestShowTemplates(object? sender, EventArgs e)
        {
            ServiceProviderHolder.GetRequiredService<TemplateTreeViewController>().ShowTemplateTreeView(TargetTemplateFolder.Workspace);
        }

        private async void OnRequestNewWorkspace(object? sender, EventArgs e)
        {
            if (!HandleUnsavedChanges())
                return;

            var filePath = _fileSystemDialogService.SaveFile(CodeGeneratorFileDialogFilter, null, "myworkspace.codegenerator");
            if (filePath == null)
                return; // User cancelled

            var directory = System.IO.Path.GetDirectoryName(filePath);
            var fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);

            if (string.IsNullOrEmpty(directory))
            {
                _messageBoxService.ShowError("Invalid directory path.", "Error");
                return;
            }

            await _workspaceTreeViewController.CreateWorkspaceAsync(directory, fileName);
            _messageBus.ReportApplicationStatus($"Created new workspace: {fileName}");
        }

        private async void OnRequestOpenWorkspace(object? sender, EventArgs e)
        {
            if (!HandleUnsavedChanges())
                return;

            var filePath = _fileSystemDialogService.OpenFile(CodeGeneratorFileDialogFilter);
            if (filePath == null)
                return; // User cancelled

            try
            {
                ApplicationSettings.Instance.AddRecentFile(filePath);
                await _workspaceTreeViewController.LoadWorkspaceAsync(filePath);
                
                _messageBus.ReportApplicationStatus($"Loaded workspace: {System.IO.Path.GetFileNameWithoutExtension(filePath)}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load workspace from {FilePath}", filePath);
                _messageBoxService.ShowError($"Failed to load workspace: {ex.Message}", "Error");
                throw;
            }
        }

        private async void OnRequestSaveWorkspace(object? sender, EventArgs e)
        {
            if (_workspaceTreeViewController.CurrentWorkspace == null)
            {
                _messageBoxService.ShowWarning("No workspace is open to save.", "No Workspace");
                return;
            }

            // If workspace has no file path, prompt for save location
            if (string.IsNullOrEmpty(_workspaceTreeViewController.CurrentWorkspace.WorkspaceFilePath))
            {
                OnSaveAsRequested(sender, e);
                return;
            }

            try
            {
                await _workspaceTreeViewController.SaveWorkspaceAsync();
                _messageBus.ReportApplicationStatus($"Saved workspace: {_workspaceTreeViewController.CurrentWorkspace.Name}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save workspace");
                _messageBoxService.ShowError($"Failed to save workspace: {ex.Message}", "Error");
            }
        }

        private void OnRequestCloseWorkspace(object? sender, EventArgs e)
        {
            if (_workspaceTreeViewController.HasUnsavedChanges)
            {
                var result = _messageBoxService.AskYesNoCancel(
                    "You have unsaved changes. Do you want to save before closing the workspace?",
                    "Unsaved Changes");
                if (result == MessageBoxResult.Cancel)
                    return;
                if (result == MessageBoxResult.Yes)
                {
                    OnRequestSaveWorkspace(sender, e);
                }
            }
            _workspaceTreeViewController.CloseWorkspace();
        }

        private void OnSaveAsRequested(object? sender, EventArgs e)
        {
            var filePath = _fileSystemDialogService.SaveFile(CodeGeneratorFileDialogFilter, null, "myworkspace.codegenerator");
            if (filePath == null)
                return; // User cancelled

            // Update workspace file path and save
            if (_workspaceTreeViewController.CurrentWorkspace != null)
            {
                _workspaceTreeViewController.CurrentWorkspace.WorkspaceFilePath = filePath;
                OnRequestSaveWorkspace(sender, e);
            }
        }

        public bool HandleUnsavedChanges()
        {
            if (_workspaceTreeViewController.HasUnsavedChanges)
            {
                var result = _messageBoxService.AskYesNoCancel(
                    "You have unsaved changes. Do you want to save before proceeding?",
                    "Unsaved Changes");
                if (result == MessageBoxResult.Cancel)
                    return false;
                if (result == MessageBoxResult.Yes)
                {
                    OnRequestSaveWorkspace(this, EventArgs.Empty);
                }
            }
            return true;
        }
     
        public void SaveBeforeApplicationCloses()
        {
            OnRequestSaveWorkspace(this, EventArgs.Empty);
        }


        public async Task OpenRecentFile(string filePath)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            if (!HandleUnsavedChanges())
                return;

            if (!File.Exists(filePath))
            {
                ApplicationSettings.Instance.RecentFiles.Remove(filePath);
                return;
            }

            try
            {
                await _workspaceTreeViewController.LoadWorkspaceAsync(filePath);
                _messageBus.ReportApplicationStatus($"Loaded workspace: {System.IO.Path.GetFileNameWithoutExtension(filePath)}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load workspace from {FilePath}", filePath);
                _messageBoxService.ShowError($"Failed to load workspace: {ex.Message}", "Error");
            }
        }

        public void CreateRibbon()
        {
            // Build Ribbon Model
            var workspaceTabBuilder = _ribbonBuilder
                .AddTab("tabWorkspace", "Workspace");

            workspaceTabBuilder.AddToolStrip("toolstripWorkspace", "Workspace")
                        .AddButton("btnNew", "New")
                            .WithSize(RibbonButtonSize.Large)
                            .WithDisplayStyle(RibbonButtonDisplayStyle.ImageAndText)
                            .WithImage("file_plus")
                            .WithCommand(_workspaceRibbonViewModel.RequestNewWorkspaceCommand)
                        .AddButton("btnOpen", "Open")
                            .WithSize(RibbonButtonSize.Large)
                            .WithDisplayStyle(RibbonButtonDisplayStyle.ImageAndText)
                            .WithImage("folder_open")
                            .WithCommand(_workspaceRibbonViewModel.RequestOpenWorkspaceCommand)
                        .AddButton("btnSave", "Save")
                            .WithSize(RibbonButtonSize.Large)
                            .WithDisplayStyle(RibbonButtonDisplayStyle.ImageAndText)
                            .WithImage("save")
                            .WithCommand(_workspaceRibbonViewModel.RequestSaveWorkspaceCommand)
                        .AddButton("btnClose", "Close")
                            .WithSize(RibbonButtonSize.Large)
                            .WithDisplayStyle(RibbonButtonDisplayStyle.ImageAndText)
                            .WithImage("x")
                            .WithCommand(_workspaceRibbonViewModel.RequestCloseWorkspaceCommand)
                        .Build();

            workspaceTabBuilder.AddToolStrip("toolstripWorkspaceTemplates", "Templates")
                    .AddButton("btnShowTemplates", "Templates")
                        .WithSize(RibbonButtonSize.Large)
                        .WithDisplayStyle(RibbonButtonDisplayStyle.ImageAndText)
                        .WithImage("file_code")
                        .WithCommand(_workspaceRibbonViewModel.RequestShowTemplatesCommand)
                    .Build();

            workspaceTabBuilder.AddToolStrip("toolstripWorkspaceUndoRedo", "History")
                    .AddButton("btnUndo", "Undo")
                        .WithSize(RibbonButtonSize.Large)
                        .WithDisplayStyle(RibbonButtonDisplayStyle.ImageAndText)
                        .WithImage("undo")
                        .WithCommand(_workspaceRibbonViewModel.RequestUndoCommand)
                    .AddButton("btnRedo", "Redo")
                        .WithSize(RibbonButtonSize.Large)
                        .WithDisplayStyle(RibbonButtonDisplayStyle.ImageAndText)
                        .WithImage("redo")
                        .WithCommand(_workspaceRibbonViewModel.RequestRedoCommand)
                    .Build();

            workspaceTabBuilder.Build();
        }

        public override void Dispose()
        {
            _workspaceRibbonViewModel.RequestCloseWorkspace -= OnRequestCloseWorkspace;
            _workspaceRibbonViewModel.RequestSaveWorkspace -= OnRequestSaveWorkspace;
            _workspaceRibbonViewModel.RequestOpenWorkspace -= OnRequestOpenWorkspace;
            _workspaceRibbonViewModel.RequestNewWorkspace -= OnRequestNewWorkspace;
            _workspaceRibbonViewModel.RequestShowTemplates -= OnRequestShowTemplates;
        }

        public void CloseWorkspace()
        {
            _workspaceTreeViewController.CloseWorkspace();
        }

        public async Task LoadWorkspaceAsync(string filePath)
        {
            await _workspaceTreeViewController.LoadWorkspaceAsync(filePath);
        }
    }
}
