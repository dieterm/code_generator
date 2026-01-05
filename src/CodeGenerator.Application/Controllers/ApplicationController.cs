using CodeGenerator.Application.Services;
using CodeGenerator.Presentation.WinForms.ViewModels;
using Microsoft.DotNet.DesignTools.ViewModels;

namespace CodeGenerator.Application.Controllers
{
    public class ApplicationController : IDisposable
    {
        private readonly MainViewModel _mainViewModel;
        private readonly IApplicationService _applicationService;
        private readonly IMessageBoxService _messageBoxService;
        private readonly IFileSystemDialogService _fileSystemDialogService;

        public ApplicationController(MainViewModel viewModel, IApplicationService applicationService, IMessageBoxService messageboxService, IFileSystemDialogService fileSystemDialogService) 
        {
            _mainViewModel = viewModel;
            _applicationService = applicationService;
            _messageBoxService = messageboxService;
            _fileSystemDialogService = fileSystemDialogService;
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

        private void OnOpenRequested(object? sender, EventArgs e)
        {
            if (!HandleUnsavedChanges())
                return;

            // Handle open document logic
            var filePath = _fileSystemDialogService.OpenFile("Json Files (*.json)|*.json|All Files (*.*)|*.*");
            if(filePath == null)
                return; // User cancelled

            // Open existing document
            _mainViewModel.IsDirty = false;
        }

        private void OnSaveRequested(object? sender, EventArgs e)
        {
            // Handle save document logic
            _mainViewModel.IsDirty = false;
        }

        private void OnGenerateRequested(object? sender, EventArgs e)
        {
            // Handle code generation logic
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
