using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Controllers.Template;
using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Application.Events.Application;
using CodeGenerator.Application.Events.DomainSchema;
using CodeGenerator.Application.Services;
using CodeGenerator.Core.Events.Application;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.MessageBus;
using CodeGenerator.Core.Settings.Application;
using CodeGenerator.Presentation.WinForms.ViewModels;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Ribbon;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers
{
    public class ApplicationController : CoreControllerBase
    {
        private readonly MainViewModel _mainViewModel;
        private readonly IApplicationService _applicationService;
        private readonly WorkspaceTreeViewController _workspaceController;
        private readonly GenerationController _generationController;
        private readonly SettingsController _settingsController;
        private readonly TemplateTreeViewController _templateController;

        public ApplicationController( MainViewModel viewModel, SettingsController settingsController, WorkspaceTreeViewController workspaceController, GenerationController generationController, TemplateTreeViewController templateController, IWindowManagerService windowManagerService, RibbonBuilder ribbonBuilder, IApplicationService applicationService, ApplicationMessageBus messageBus, IMessageBoxService messageBoxService, IFileSystemDialogService fileSystemDialogService, ILogger<ApplicationController> logger) 
            : base(windowManagerService, ribbonBuilder, messageBus, messageBoxService, fileSystemDialogService, logger)
        {
            _mainViewModel = viewModel;
            _settingsController = settingsController;
            _applicationService = applicationService;
            _workspaceController = workspaceController;
            _generationController = generationController;
            _templateController = templateController;
        }

        public override void Initialize()
        {
            SubscribeToMessageBusEvents();

            CreateRibbon();
            _settingsController.CreateRibbon();
            _templateController.Initialize();

            _generationController.Initialize();
            _settingsController.Initialize();
        }

        private void CreateRibbon()
        {
            // Build Ribbon Model
            _ribbonBuilder
                .AddTab("tabWorkspace", "Workspace")
                    .AddToolStrip("toolstripWorkspace", "Workspace")
                        .AddButton("btnNew", "New")
                            .WithSize(RibbonButtonSize.Large)
                            .WithDisplayStyle(RibbonButtonDisplayStyle.ImageAndText)
                            .WithImage("file_plus")
                            .OnClick((e) => OnNewRequested(null, e))
                        .AddButton("btnOpen", "Open")
                            .WithSize(RibbonButtonSize.Large)
                            .WithDisplayStyle(RibbonButtonDisplayStyle.ImageAndText)
                            .WithImage("folder_open")
                            .OnClick((e) => OnOpenRequested(null, e))
                        .AddButton("btnSave", "Save")
                            .WithSize(RibbonButtonSize.Large)
                            .WithDisplayStyle(RibbonButtonDisplayStyle.ImageAndText)
                            .WithImage("save")
                            .OnClick((e) => OnSaveRequested(null, e))
                        .AddButton("btnClose", "Close")
                            .WithSize(RibbonButtonSize.Large)
                            .WithDisplayStyle(RibbonButtonDisplayStyle.ImageAndText)
                            .WithImage("x")
                            .OnClick((e) => OnCloseRequested(null, e))
                            .Build();

            _ribbonBuilder.AddTab("tabGeneration", "Generation")
                    .AddToolStrip("toolstripGeneration", "Generation")
                        .AddButton("btnGenerate", "Generate")
                            .WithSize(RibbonButtonSize.Large)
                            .WithDisplayStyle(RibbonButtonDisplayStyle.ImageAndText)
                            .WithImage("scroll_text")
                            .OnClick((e) => OnGenerateRequested(null, e))
                        .AddButton("btnGeneratePreview", "Preview")
                            .WithSize(RibbonButtonSize.Large)
                            .WithDisplayStyle(RibbonButtonDisplayStyle.ImageAndText)
                            .WithImage("eye")
                            .OnClick((e) => OnGeneratePreviewRequested(null, e))
                        .AddButton("btnCancelGeneration", "Cancel")
                            .WithSize(RibbonButtonSize.Large)
                            .WithDisplayStyle(RibbonButtonDisplayStyle.ImageAndText)
                            .WithImage("x")
                            .OnClick((e) => OnCancelGenerationRequested(null, e))
                            .Build();
        }

        private void OnCloseRequested(object? sender, EventArgs e)
        {
            _workspaceController.CloseWorkspace();
        }

        private void OnSettingsRequested(object value, EventArgs e)
        {
            _settingsController.ShowSettings();
        }

        private void SubscribeToMessageBusEvents()
        {
            _messageBus.Subscribe<DomainSchemaLoadedEvent>((e) => { _mainViewModel.StatusLabel = e.FilePath ?? "New DomainSchema created"; });
            _messageBus.Subscribe<ReportApplicationStatusEvent>((e) => { _mainViewModel.StatusLabel = e.StatusMessage; });
            _messageBus.Subscribe<ReportTaskProgressEvent>((e) =>
            {
                _mainViewModel.ProgressValue = e.PercentComplete;
                if (!string.IsNullOrEmpty(e.TaskName)) { 
                    _mainViewModel.StatusLabel = e.TaskName;
                } else {
                    if (e.PercentComplete.HasValue) { 
                        if(e.PercentComplete.Value >= 100) 
                        { 
                            _mainViewModel.ProgressLabel = "Completed";
                        }
                        else 
                        { 
                            _mainViewModel.ProgressLabel = "Working...";
                        }
                    }
                }
            });
        }

        public void StartApplication()
        {
            // Subscribe to ViewModel events
            _mainViewModel.NewRequested += OnNewRequested;
            _mainViewModel.OpenRequested += OnOpenRequested;
            _mainViewModel.OpenRecentFileRequested += OnOpenRecentFileRequested;
            _mainViewModel.SaveRequested += OnSaveRequested;
            _mainViewModel.SaveAsRequested += OnSaveAsRequested;
            _mainViewModel.GenerateRequested += OnGenerateRequested;
            _mainViewModel.ClosingRequested += OnClosingRequested;
            _mainViewModel.RibbonViewModel = ServiceProviderHolder.GetRequiredService<RibbonBuilder>().Build();
            _applicationService.RunApplication(_mainViewModel);
            // we never get here, because RunApplication is blocking in Gui-loop
        }



        public const string CodeGeneratorFileDialogFilter = "CodeGenerator Workspace Files (*.codegenerator)|*.codegenerator|All Files (*.*)|*.*";
        
        private void OnSaveAsRequested(object? sender, EventArgs e)
        {
            var filePath = _fileSystemDialogService.SaveFile(CodeGeneratorFileDialogFilter, null, "myworkspace.codegenerator");
            if (filePath == null)
                return; // User cancelled
                
            // Update workspace file path and save
            if (_workspaceController.CurrentWorkspace != null)
            {
                _workspaceController.CurrentWorkspace.WorkspaceFilePath = filePath;
                OnSaveRequested(sender, e);
            }
        }

        private void OnClosingRequested(object? sender, EventArgs e)
        { 
            if (_mainViewModel.IsDirty)
            {
                var result = _messageBoxService.AskYesNoCancel(
                    "You have unsaved changes. Do you want to save before closing?",
                    "Unsaved Changes");

                if(result == MessageBoxResult.Yes)
                    OnSaveRequested(this, EventArgs.Empty);

                if (result == MessageBoxResult.Cancel) 
                    return; // User cancelled, don't close
            }

            _mainViewModel.IsClosing = true;
            _workspaceController.CloseWorkspace();
            _applicationService.ExitApplication();
        }

        private async void OnNewRequested(object? sender, EventArgs e)
        {
            if(!HandleUnsavedChanges())
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
            
            await _workspaceController.CreateWorkspaceAsync(directory, fileName);
            _mainViewModel.IsDirty = false;
            _mainViewModel.StatusLabel = $"Created new workspace: {fileName}";
        }

        private bool HandleUnsavedChanges()
        {
            if (_mainViewModel.IsDirty)
            {
                var result = _messageBoxService.AskYesNoCancel(
                    "You have unsaved changes. Do you want to save before proceeding?",
                    "Unsaved Changes");
                if (result == MessageBoxResult.Cancel)
                    return false;
                if (result == MessageBoxResult.Yes)
                {
                    OnSaveRequested(this, EventArgs.Empty);
                }
            }
            return true;
        }
        private async void OnOpenRecentFileRequested(object? sender, OpenRecentFileRequestedEventArgs e)
        {
            if (e.FilePath == null)
                throw new ArgumentNullException(nameof(e.FilePath));

            if (!HandleUnsavedChanges())
                return;

            if (!File.Exists(e.FilePath)) {
                ApplicationSettings.Instance.RecentFiles.Remove(e.FilePath);
                return;
            }

            try
            {
                await _workspaceController.LoadWorkspaceAsync(e.FilePath);
                _mainViewModel.IsDirty = false;
                _mainViewModel.StatusLabel = $"Loaded workspace: {System.IO.Path.GetFileNameWithoutExtension(e.FilePath)}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load workspace from {FilePath}", e.FilePath);
                _messageBoxService.ShowError($"Failed to load workspace: {ex.Message}", "Error");
            }
        }
        private async void OnOpenRequested(object? sender, EventArgs e)
        {
            if (!HandleUnsavedChanges())
                return;

            var filePath = _fileSystemDialogService.OpenFile(CodeGeneratorFileDialogFilter);
            if(filePath == null)
                return; // User cancelled

            try
            {
                ApplicationSettings.Instance.AddRecentFile(filePath);
                await _workspaceController.LoadWorkspaceAsync(filePath);
                _mainViewModel.IsDirty = false;
                _mainViewModel.StatusLabel = $"Loaded workspace: {System.IO.Path.GetFileNameWithoutExtension(filePath)}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load workspace from {FilePath}", filePath);
                _messageBoxService.ShowError($"Failed to load workspace: {ex.Message}", "Error");
            }
        }

        private async void OnSaveRequested(object? sender, EventArgs e)
        {
            if (_workspaceController.CurrentWorkspace == null)
            {
                _messageBoxService.ShowWarning("No workspace is open to save.", "No Workspace");
                return;
            }
            
            // If workspace has no file path, prompt for save location
            if (string.IsNullOrEmpty(_workspaceController.CurrentWorkspace.WorkspaceFilePath))
            {
                OnSaveAsRequested(sender, e);
                return;
            }

            try
            {
                await _workspaceController.SaveWorkspaceAsync();
                _mainViewModel.IsDirty = false;
                _mainViewModel.StatusLabel = $"Saved workspace: {_workspaceController.CurrentWorkspace.Name}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save workspace");
                _messageBoxService.ShowError($"Failed to save workspace: {ex.Message}", "Error");
            }
        }

        private CancellationTokenSource? _generationCancellationTokenSource;
        
        /// <summary>
        /// Handle code generation logic. <br/>
        /// Triggered when user clicks "Generate" button
        /// </summary>
        private async void OnGenerateRequested(object? sender, EventArgs e)
        {
            if(_generationCancellationTokenSource != null)
            {
                _messageBoxService.ShowWarning("Generation is already in progress.", "Generation In Progress");
                return;
            }
            _generationCancellationTokenSource = new CancellationTokenSource();
            await _generationController.GenerateAsync(_mainViewModel, _generationCancellationTokenSource.Token);
            _generationCancellationTokenSource = null;
        }

        private async void OnGeneratePreviewRequested(object? sender, EventArgs e)
        {
            if (_generationCancellationTokenSource != null)
            {
                _messageBoxService.ShowWarning("Generation is already in progress.", "Generation In Progress");
                return;
            }
            _generationCancellationTokenSource = new CancellationTokenSource();
            await _generationController.GeneratePreviewAsync(_mainViewModel, _generationCancellationTokenSource.Token);
            _generationCancellationTokenSource = null;
        }
        
        private void OnCancelGenerationRequested(object? sender, EventArgs e)
        {
            if (_generationCancellationTokenSource != null)
            {
                _generationCancellationTokenSource.Cancel();
                _generationCancellationTokenSource = null;
                _mainViewModel.StatusLabel = "Generation cancelled";
            }
        }

        public override void Dispose()
        {
            // Unsubscribe from events
            _mainViewModel.NewRequested -= OnNewRequested;
            _mainViewModel.OpenRequested -= OnOpenRequested;
            _mainViewModel.SaveRequested -= OnSaveRequested;
            _mainViewModel.SaveAsRequested -= OnSaveAsRequested;
            _mainViewModel.GenerateRequested -= OnGenerateRequested;
            _mainViewModel.ClosingRequested -= OnClosingRequested;
        }
    }
}
