using CodeGenerator.Application.Events.DomainSchema;
using CodeGenerator.Application.MessageBus;
using CodeGenerator.Application.Services;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.MessageBus;
using CodeGenerator.Presentation.WinForms.ViewModels;
using Microsoft.DotNet.DesignTools.ViewModels;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers
{
    public class ApplicationController : IDisposable
    {
        private readonly MainViewModel _mainViewModel;
        private readonly IApplicationService _applicationService;
        private readonly IMessageBoxService _messageBoxService;
        private readonly IFileSystemDialogService _fileSystemDialogService;
        private readonly ILogger<ApplicationController> _logger;
        private readonly ApplicationMessageBus _messageBus;
        private readonly DomainSchemaController _domainSchemaController;
        private readonly GeneratorOrchestrator _generatorOrchestrator;
        public ApplicationController(MainViewModel viewModel, DomainSchemaController domainSchemaController, IApplicationService applicationService, ApplicationMessageBus messageBus, IMessageBoxService messageboxService, IFileSystemDialogService fileSystemDialogService, GeneratorOrchestrator generatorOrchestrator, ILogger<ApplicationController> logger) 
        {
            _mainViewModel = viewModel;
            _applicationService = applicationService;
            _messageBus = messageBus;
            _messageBoxService = messageboxService;
            _fileSystemDialogService = fileSystemDialogService;
            _domainSchemaController = domainSchemaController;
            _generatorOrchestrator = generatorOrchestrator;
            _logger = logger;

            SubscribeToMessageBusEvents();
        }

        private void SubscribeToMessageBusEvents()
        {
            _messageBus.Subscribe<DomainSchemaLoadedEvent>((e) => _mainViewModel.StatusLabel = e.FilePath??"New DomainSchema created");
        }

        public void StartApplication()
        {
            // Subscribe to ViewModel events
            _mainViewModel.NewRequested += OnNewRequested;
            _mainViewModel.OpenRequested += OnOpenRequested;
            _mainViewModel.SaveRequested += OnSaveRequested;
            _mainViewModel.SaveAsRequested += OnSaveAsRequested;
            _mainViewModel.GenerateRequested += OnGenerateRequested;
            _mainViewModel.ClosingRequested += OnClosingRequested;

            _applicationService.RunApplication(_mainViewModel);
            // we never get here, because RunApplication is blocking in Gui-loop
        }

        private void OnSaveAsRequested(object? sender, EventArgs e)
        {
            _fileSystemDialogService.SaveFile("Json Files (*.json)|*.json|All Files (*.*)|*.*", null, "MonkeyBusiness.json");

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

                if (result != MessageBoxResult.No) 
                    return; // if user clicks yes or cancel, don't close the application
            }

            _mainViewModel.IsClosing = true;
            _applicationService.ExitApplication();
        }

        private void OnNewRequested(object? sender, EventArgs e)
        {
            if(!HandleUnsavedChanges())
                return;

            // Create new document
            _mainViewModel.IsDirty = false;
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

        private async void OnOpenRequested(object? sender, EventArgs e)
        {
            if (!HandleUnsavedChanges())
                return;

            // Handle open document logic
            var filePath = _fileSystemDialogService.OpenFile("Json Files (*.json)|*.json|All Files (*.*)|*.*");
            if(filePath == null)
                return; // User cancelled

            await _domainSchemaController.LoadDomainSchemaAsync(filePath);

            // Open existing document
            _mainViewModel.IsDirty = false;
        }

        private void OnSaveRequested(object? sender, EventArgs e)
        {
            // Handle save document logic
            _mainViewModel.IsDirty = false;
        }

        /// <summary>
        /// Handle code generation logic. <br/>
        /// Triggered when user clicks "Generate" button
        /// </summary>
        private async void OnGenerateRequested(object? sender, EventArgs e)
        {
            await _generatorOrchestrator.GenerateAsync(_domainSchemaController.DomainSchema, false, _mainViewModel);
        }

        public void Dispose()
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
