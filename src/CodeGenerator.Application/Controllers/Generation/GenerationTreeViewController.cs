using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Services;
using CodeGenerator.Application.ViewModels.Generation;
using CodeGenerator.Core.Generators;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Operations;
using CodeGenerator.Shared.ViewModels;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Generation
{
    /// <summary>
    /// Controller for the generation result artifact tree.
    /// Coordinates between generation artifact controllers and the UI.
    /// </summary>
    public class GenerationTreeViewController : ArtifactTreeViewController<GenerationTreeViewModel>
    {
        private readonly GenerationDetailsViewModel _detailsViewModel;

        /// <summary>
        /// Event raised when the details view for a selected artifact should be shown
        /// </summary>
        public event EventHandler<ViewModelBase?>? ShowArtifactDetailsRequested;

        public GenerationTreeViewController(
            OperationExecutor operationExecutor,
            GenerationDetailsViewModel detailsViewModel,
            IWindowManagerService windowManagerService,
            IMessageBoxService messageBoxService,
            ILogger<GenerationTreeViewController> logger)
            : base(operationExecutor, windowManagerService, messageBoxService, logger)
        {
            _detailsViewModel = detailsViewModel;
        }

        protected override IEnumerable<IArtifactController> LoadArtifactControllers()
        {
            var controllers = ServiceProviderHolder.GetServices<IGenerationArtifactController>();
            return controllers;
        }

        public void Initialize()
        {
            // Future initialization logic
        }

        /// <summary>
        /// Load a generation result into the tree
        /// </summary>
        public void LoadGenerationResult(GenerationResult generationResult)
        {
            if (TreeViewModel == null) return;

            TreeViewModel.RootArtifact = generationResult?.RootArtifact;
        }

        public override void ShowArtifactDetailsView(ViewModelBase? detailsModel)
        {
            ShowArtifactDetailsRequested?.Invoke(this, detailsModel);
        }

        /// <summary>
        /// Called when an artifact is selected — update the details view model
        /// </summary>
        protected override void OnArtifactSelected(Core.Artifacts.IArtifact artifact)
        {
            base.OnArtifactSelected(artifact);
            _detailsViewModel.SelectedArtifact = artifact;
        }
    }
}
