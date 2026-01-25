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

namespace CodeGenerator.Application.ViewModels.Generation
{
    public class GenerationRibbonViewModel : ViewModelBase
    {
        private readonly IWorkspaceContextProvider _workspaceContextProvider;
        
        // Events
        public event EventHandler? StartGenerationRequested;
        public event EventHandler? StartPreviewRequested;
        public event EventHandler? CancelGenerationRequested;

        // Properties
        private bool _isGenerating;
        public bool IsGenerating
        {
            get { return _isGenerating; }
            set
            {
                if (SetProperty(ref _isGenerating, value))
                {
                    RaiseCanExecuteChanged();
                }
            }
        }

        // Commands
        public ICommand StartGenerationCommand { get; }
        public ICommand StartPreviewCommand { get; }
        public ICommand CancelGenerationCommand { get; }


        public GenerationRibbonViewModel(IWorkspaceContextProvider workspaceContextProvider)
        {
            _workspaceContextProvider = workspaceContextProvider;
            _workspaceContextProvider.WorkspaceChanged += OnWorkspaceChanged;

            StartGenerationCommand = new RelayCommand((e) => StartGenerationRequested?.Invoke(this, EventArgs.Empty), CanStartGeneration);
            StartPreviewCommand = new RelayCommand((e) => StartPreviewRequested?.Invoke(this, EventArgs.Empty), CanStartPreview);
            CancelGenerationCommand = new RelayCommand((e) => CancelGenerationRequested?.Invoke(this, EventArgs.Empty), CanCancelGeneration);
        }

        private void OnWorkspaceChanged(object? sender, WorkspaceArtifact? e)
        {
            RaiseCanExecuteChanged();
        }

        private void RaiseCanExecuteChanged()
        {
            (StartGenerationCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (StartPreviewCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (CancelGenerationCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        private bool CanCancelGeneration(object? arg)
        {
            return _workspaceContextProvider.CurrentWorkspace != null && IsGenerating;
        }

        private bool CanStartPreview(object? arg)
        {
            return _workspaceContextProvider.CurrentWorkspace != null && !IsGenerating;
        }

        private bool CanStartGeneration(object? arg)
        {
            return _workspaceContextProvider.CurrentWorkspace != null && !IsGenerating;
        }
    }
}
