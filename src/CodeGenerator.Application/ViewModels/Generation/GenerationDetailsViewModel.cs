using CodeGenerator.Core.Artifacts;
using CodeGenerator.Shared.ViewModels;

namespace CodeGenerator.Application.ViewModels.Generation
{
    /// <summary>
    /// ViewModel for the generation details view (MultiColumnTreeView showing artifact properties)
    /// </summary>
    public class GenerationDetailsViewModel : ViewModelBase
    {
        private IArtifact? _selectedArtifact;

        /// <summary>
        /// The currently selected artifact whose details are displayed
        /// </summary>
        public IArtifact? SelectedArtifact
        {
            get => _selectedArtifact;
            set => SetProperty(ref _selectedArtifact, value);
        }
    }
}
