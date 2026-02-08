using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Generators;
using CodeGenerator.Shared;
using CodeGenerator.Shared.ViewModels;
using System;
using System.Windows.Input;

namespace CodeGenerator.Application.ViewModels.Generation
{
    /// <summary>
    /// Top-level ViewModel for the generation result view.
    /// Contains the tree ViewModel and details ViewModel.
    /// </summary>
    public class GenerationResultTreeViewModel : ViewModelBase
    {
        private GenerationResult? _generationResult;

        public GenerationResult? GenerationResult
        {
            get => _generationResult;
            set => SetProperty(ref _generationResult, value);
        }

        private GenerationTreeViewModel? _treeViewModel;
        /// <summary>
        /// ViewModel for the artifact tree (upper panel)
        /// </summary>
        public GenerationTreeViewModel? TreeViewModel
        {
            get => _treeViewModel;
            set => SetProperty(ref _treeViewModel, value);
        }

        private GenerationDetailsViewModel? _detailsViewModel;
        /// <summary>
        /// ViewModel for the details view (lower panel)
        /// </summary>
        public GenerationDetailsViewModel? DetailsViewModel
        {
            get => _detailsViewModel;
            set => SetProperty(ref _detailsViewModel, value);
        }

        private IArtifact? _selectedArtifact;
        public IArtifact? SelectedArtifact
        {
            get => _selectedArtifact;
            set => SetProperty(ref _selectedArtifact, value);
        }

        // SelectArtifactCommand
        public ICommand SelectArtifactCommand { get; }
        public event EventHandler<ArtifactSelectedEventArgs>? ArtifactSelected;
        public GenerationResultTreeViewModel()
        {
            SelectArtifactCommand = new RelayCommand((artifact) => ArtifactSelected?.Invoke(this, new ArtifactSelectedEventArgs((IArtifact)artifact)));
        }

        public class ArtifactSelectedEventArgs : EventArgs
        {
            public IArtifact SelectedArtifact { get; }
            public ArtifactSelectedEventArgs(IArtifact selectedArtifact)
            {
                SelectedArtifact = selectedArtifact;
            }
        }
    }
}
