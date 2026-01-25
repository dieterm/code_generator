using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared;
using CodeGenerator.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CodeGenerator.Application.ViewModels.Workspace
{
    public class WorkspaceRibbonViewModel : ViewModelBase
    {
        private readonly IWorkspaceContextProvider _workspaceContextProvider;
        
        // Events
        public event EventHandler? RequestNewWorkspace;
        public event EventHandler? RequestOpenWorkspace;
        public event EventHandler? RequestSaveWorkspace;
        public event EventHandler? RequestCloseWorkspace;
        public event EventHandler? RequestShowTemplates;

        // Commands
        public ICommand RequestNewWorkspaceCommand { get; }
        public ICommand RequestOpenWorkspaceCommand { get; }
        public ICommand RequestSaveWorkspaceCommand { get; }
        public ICommand RequestCloseWorkspaceCommand { get; }
        public ICommand RequestShowTemplatesCommand { get; }

        public WorkspaceRibbonViewModel(IWorkspaceContextProvider workspaceContextProvider)
        {
            _workspaceContextProvider = workspaceContextProvider;
            _workspaceContextProvider.WorkspaceChanged += OnWorkspaceChanged;
            _workspaceContextProvider.WorkspaceHasUnsavedChangesChanged += OnWorkspaceHasUnsavedChangesChanged;
            RequestNewWorkspaceCommand = new RelayCommand((e) => RequestNewWorkspace?.Invoke(this, EventArgs.Empty), CanRequestNewWorkspace);
            RequestOpenWorkspaceCommand = new RelayCommand((e) => RequestOpenWorkspace?.Invoke(this, EventArgs.Empty), CanRequestOpenWorkspace);
            RequestSaveWorkspaceCommand = new RelayCommand((e) => RequestSaveWorkspace?.Invoke(this, EventArgs.Empty), CanRequestSaveWorkspace);
            RequestCloseWorkspaceCommand = new RelayCommand((e) => RequestCloseWorkspace?.Invoke(this, EventArgs.Empty), CanRequestCloseWorkspace);
            RequestShowTemplatesCommand = new RelayCommand((e) => RequestShowTemplates?.Invoke(this, EventArgs.Empty), CanShowTemplates);
        }

        private void OnWorkspaceHasUnsavedChangesChanged(object? sender, EventArgs e)
        {
            RaiseCanExecuteChanged();
        }

        private void OnWorkspaceChanged(object? sender, WorkspaceArtifact? e)
        {
            RaiseCanExecuteChanged();
        }

        private void RaiseCanExecuteChanged()
        {
            (RequestNewWorkspaceCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (RequestOpenWorkspaceCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (RequestSaveWorkspaceCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (RequestCloseWorkspaceCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (RequestShowTemplatesCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        private bool CanRequestNewWorkspace(object? arg)
        {
            return true; // Always enabled
        }

        private bool CanRequestOpenWorkspace(object? arg)
        {
            return true; // Always enabled
        }

        private bool CanRequestSaveWorkspace(object? arg)
        {
            return _workspaceContextProvider.CurrentWorkspace != null && _workspaceContextProvider.HasUnsavedChanges;
        }

        private bool CanRequestCloseWorkspace(object? arg)
        {
            return _workspaceContextProvider.CurrentWorkspace != null;
        }

        private bool CanShowTemplates(object? arg)
        {
            return _workspaceContextProvider.CurrentWorkspace != null;
        }
    }
}
