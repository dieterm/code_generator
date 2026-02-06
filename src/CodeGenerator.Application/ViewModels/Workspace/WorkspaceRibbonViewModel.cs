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
        public event EventHandler? RequestUndo;
        public event EventHandler? RequestRedo;

        // Commands
        public ICommand NewWorkspaceCommand { get; }
        public ICommand OpenWorkspaceCommand { get; }
        public ICommand SaveWorkspaceCommand { get; }
        public ICommand CloseWorkspaceCommand { get; }
        public ICommand ShowTemplatesCommand { get; }
        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }

        public WorkspaceRibbonViewModel(IWorkspaceContextProvider workspaceContextProvider)
        {
            _workspaceContextProvider = workspaceContextProvider;
            _workspaceContextProvider.WorkspaceChanged += OnWorkspaceChanged;
            _workspaceContextProvider.WorkspaceHasUnsavedChangesChanged += OnWorkspaceHasUnsavedChangesChanged;

            NewWorkspaceCommand = new RelayCommand((e) => RequestNewWorkspace?.Invoke(this, EventArgs.Empty), CanRequestNewWorkspace);
            OpenWorkspaceCommand = new RelayCommand((e) => RequestOpenWorkspace?.Invoke(this, EventArgs.Empty), CanRequestOpenWorkspace);
            SaveWorkspaceCommand = new RelayCommand((e) => RequestSaveWorkspace?.Invoke(this, EventArgs.Empty), CanRequestSaveWorkspace);
            CloseWorkspaceCommand = new RelayCommand((e) => RequestCloseWorkspace?.Invoke(this, EventArgs.Empty), CanRequestCloseWorkspace);
            ShowTemplatesCommand = new RelayCommand((e) => RequestShowTemplates?.Invoke(this, EventArgs.Empty), CanShowTemplates);
            UndoCommand = new RelayCommand((e) => RequestUndo?.Invoke(this, EventArgs.Empty));
            RedoCommand = new RelayCommand((e) => RequestRedo?.Invoke(this, EventArgs.Empty));
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
            (NewWorkspaceCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (OpenWorkspaceCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (SaveWorkspaceCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (CloseWorkspaceCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (ShowTemplatesCommand as RelayCommand)?.RaiseCanExecuteChanged();
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
