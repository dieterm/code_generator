using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.DomainSchema.Schema;
using CodeGenerator.Core.Generators;
using CodeGenerator.Shared;
using CodeGenerator.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CodeGenerator.Application.ViewModels
{
    public class GenerationResultTreeViewModel : ViewModelBase
    {
        private GenerationResult _generationResult;
        public GenerationResult GenerationResult
        {
            get => _generationResult;
            set => SetProperty(ref _generationResult, value);
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
