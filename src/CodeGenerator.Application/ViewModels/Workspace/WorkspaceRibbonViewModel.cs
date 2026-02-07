using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Ribbon;
using CodeGenerator.Shared.UndoRedo;
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
        private readonly UndoRedoManager _undoRedoManager;
        
        // Events
        public event EventHandler? RequestNewWorkspace;
        public event EventHandler? RequestOpenWorkspace;
        public event EventHandler? RequestSaveWorkspace;
        public event EventHandler? RequestCloseWorkspace;
        public event EventHandler? RequestShowTemplates;
        public event EventHandler? RequestShowCopilot;
        public event EventHandler? RequestUndo;
        public event EventHandler? RequestRedo;
        public event EventHandler<int>? RequestUndoMultiple;
        public event EventHandler<int>? RequestRedoMultiple;

        // Commands
        public ICommand NewWorkspaceCommand { get; }
        public ICommand OpenWorkspaceCommand { get; }
        public ICommand SaveWorkspaceCommand { get; }
        public ICommand CloseWorkspaceCommand { get; }
        public ICommand ShowTemplatesCommand { get; }
        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }
        public ICommand ShowCopilotCommand { get; }

        public WorkspaceRibbonViewModel(IWorkspaceContextProvider workspaceContextProvider, UndoRedoManager undoRedoManager)
        {
            _workspaceContextProvider = workspaceContextProvider;
            _undoRedoManager = undoRedoManager;
            _workspaceContextProvider.WorkspaceChanged += OnWorkspaceChanged;
            _workspaceContextProvider.WorkspaceHasUnsavedChangesChanged += OnWorkspaceHasUnsavedChangesChanged;
            _undoRedoManager.HistoryChanged += OnUndoRedoHistoryChanged;

            NewWorkspaceCommand = new RelayCommand((e) => RequestNewWorkspace?.Invoke(this, EventArgs.Empty), CanRequestNewWorkspace);
            OpenWorkspaceCommand = new RelayCommand((e) => RequestOpenWorkspace?.Invoke(this, EventArgs.Empty), CanRequestOpenWorkspace);
            SaveWorkspaceCommand = new RelayCommand((e) => RequestSaveWorkspace?.Invoke(this, EventArgs.Empty), CanRequestSaveWorkspace);
            CloseWorkspaceCommand = new RelayCommand((e) => RequestCloseWorkspace?.Invoke(this, EventArgs.Empty), CanRequestCloseWorkspace);
            ShowTemplatesCommand = new RelayCommand((e) => RequestShowTemplates?.Invoke(this, EventArgs.Empty), CanShowTemplates);
            ShowCopilotCommand = new RelayCommand((e) => RequestShowCopilot?.Invoke(this, EventArgs.Empty), CanShowCopilot);
            UndoCommand = new RelayCommand((e) => RequestUndo?.Invoke(this, EventArgs.Empty), _ => _undoRedoManager.CanUndo);
            RedoCommand = new RelayCommand((e) => RequestRedo?.Invoke(this, EventArgs.Empty), _ => _undoRedoManager.CanRedo);
        }



        private void OnUndoRedoHistoryChanged(object? sender, EventArgs e)
        {
            (UndoCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (RedoCommand as RelayCommand)?.RaiseCanExecuteChanged();
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
            (ShowCopilotCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
        private bool CanShowCopilot(object? arg)
        {
            return _workspaceContextProvider.CurrentWorkspace != null; // Enable if a workspace is open
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

        /// <summary>
        /// Returns the dropdown items provider for the Undo history dropdown.
        /// Each item shows an action description; clicking it undoes up to and including that action.
        /// </summary>
        public IEnumerable<RibbonDropDownItemViewModel> GetUndoDropDownItems()
        {
            var index = 0;
            foreach (var description in _undoRedoManager.GetUndoHistory())
            {
                var capturedIndex = index + 1;
                yield return new RibbonDropDownItemViewModel
                {
                    Name = $"undoItem_{index}",
                    Text = description,
                    Tag = capturedIndex,
                    ClickHandler = (item) => RequestUndoMultiple?.Invoke(this, capturedIndex)
                };
                index++;
            }
        }

        /// <summary>
        /// Returns the dropdown items provider for the Redo history dropdown.
        /// Each item shows an action description; clicking it redoes up to and including that action.
        /// </summary>
        public IEnumerable<RibbonDropDownItemViewModel> GetRedoDropDownItems()
        {
            var index = 0;
            foreach (var description in _undoRedoManager.GetRedoHistory())
            {
                var capturedIndex = index + 1;
                yield return new RibbonDropDownItemViewModel
                {
                    Name = $"redoItem_{index}",
                    Text = description,
                    Tag = capturedIndex,
                    ClickHandler = (item) => RequestRedoMultiple?.Invoke(this, capturedIndex)
                };
                index++;
            }
        }
    }
}
