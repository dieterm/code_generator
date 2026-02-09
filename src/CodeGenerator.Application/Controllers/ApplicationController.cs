using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Controllers.Generation;
using CodeGenerator.Application.Controllers.Template;
using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Application.Events.Application;
using CodeGenerator.Application.Services;
using CodeGenerator.Core.Events.Application;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.MessageBus;
using CodeGenerator.Core.Settings.Application;
using CodeGenerator.Core.Templates;
using CodeGenerator.Core.Workspaces.Settings;
using CodeGenerator.Presentation.WinForms.ViewModels;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Operations;
using CodeGenerator.Shared.Ribbon;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers
{
    public class ApplicationController : CoreControllerBase
    {
        private readonly MainViewModel _mainViewModel;
        public MainViewModel MainViewModel { get { return _mainViewModel; } }
        private readonly IApplicationService _applicationService;
        private readonly WorkspaceController _workspaceController;
        private readonly GenerationController _generationController;
        private readonly SettingsController _settingsController;
        private readonly TemplateController _templateController;


        public ApplicationController(OperationExecutor operationExecutor, MainViewModel viewModel, SettingsController settingsController, WorkspaceController workspaceController, GenerationController generationController, TemplateController templateController, IWindowManagerService windowManagerService, RibbonBuilder ribbonBuilder, IApplicationService applicationService, ApplicationMessageBus messageBus, IMessageBoxService messageBoxService, IFileSystemDialogService fileSystemDialogService, ILogger<ApplicationController> logger) 
            : base(operationExecutor, windowManagerService, ribbonBuilder, messageBus, messageBoxService, fileSystemDialogService, logger)
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

            _workspaceController.Initialize();
            _templateController.Initialize();
            _generationController.Initialize();
            _settingsController.Initialize();
        }


        private void CreateRibbon()
        {
            _workspaceController.CreateRibbon();
            _generationController.CreateRibbon();
            _settingsController.CreateRibbon();
            _templateController.CreateRibbon();
        }

        private void OnSettingsRequested(object value, EventArgs e)
        {
            _settingsController.ShowSettings();
        }

        /// <summary>
        /// React to application-wide events from the message bus
        /// </summary>
        private void SubscribeToMessageBusEvents()
        {
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
            _messageBus.Subscribe<ShowArtifactPreviewEvent>((e) =>
            {
                ServiceProviderHolder.GetRequiredService<IWindowManagerService>().ShowArtifactPreview(e.ViewModel);
            });
        }

        public void StartApplication()
        {
            // Subscribe to ViewModel events
            _mainViewModel.OpenRecentFileRequested += OnOpenRecentFileRequested;
            _mainViewModel.ClosingRequested += OnClosingRequested;
            
            CreateRibbon();
            _mainViewModel.RibbonViewModel = ServiceProviderHolder.GetRequiredService<RibbonBuilder>().Build();
            
            _applicationService.RunApplication(_mainViewModel);
            // we never get here, because RunApplication is blocking in Gui-loop
        }

        private void OnClosingRequested(object? sender, EventArgs e)
        { 
            if (_workspaceController.HasUnsavedChanges)
            {
                var result = _messageBoxService.AskYesNoCancel(
                    "You have unsaved changes. Do you want to save before closing?",
                    "Unsaved Changes");

                if(result == MessageBoxResult.Yes)
                    _workspaceController.SaveBeforeApplicationCloses();

                if (result == MessageBoxResult.Cancel) 
                    return; // User cancelled, don't close
            }

            _mainViewModel.IsClosing = true;
            _workspaceController.CloseWorkspace();
            _applicationService.ExitApplication();
        }

        /// <summary>
        /// Forward to WorkspaceController to open recent file
        /// </summary>
        private async void OnOpenRecentFileRequested(object? sender, OpenRecentFileRequestedEventArgs e)
        {
            await _workspaceController.OpenRecentFile(e.FilePath);
        }

        public override void Dispose()
        {
            // Unsubscribe from events
            _mainViewModel.ClosingRequested -= OnClosingRequested;
        }
    }
}
