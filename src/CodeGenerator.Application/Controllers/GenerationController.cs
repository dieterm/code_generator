using CodeGenerator.Application.Services;
using CodeGenerator.Application.ViewModels;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.DomainSchema.Schema;
using CodeGenerator.Core.Generators;
using CodeGenerator.Shared.Ribbon;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Application.Controllers
{
    public class GenerationController
    {
        private readonly ILogger _logger;
        private readonly GeneratorOrchestrator _generatorOrchestrator;
        private readonly GenerationResultTreeViewModel _treeViewModel;
        private readonly IWindowManagerService _windowManagerService;
        private readonly DomainSchemaController _domainSchemaController;

        public GenerationController(DomainSchemaController domainSchemaController, IWindowManagerService windowManagerService, GeneratorOrchestrator generatorOrchestrator, GenerationResultTreeViewModel treeViewModel, ILogger<GenerationController> logger)
        {
            _generatorOrchestrator = generatorOrchestrator ?? throw new ArgumentNullException(nameof(generatorOrchestrator));
            _treeViewModel = treeViewModel ?? throw new ArgumentNullException(nameof(treeViewModel));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _windowManagerService = windowManagerService ?? throw new ArgumentNullException(nameof(windowManagerService));
            _domainSchemaController = domainSchemaController ?? throw new ArgumentNullException(nameof(domainSchemaController));

            _treeViewModel.ArtifactSelected += OnArtifactSelected;
        }

        private void OnArtifactSelected(object sender, GenerationResultTreeViewModel.ArtifactSelectedEventArgs e)
        {
            var selectedArtifact = e.SelectedArtifact;
            _windowManagerService.ShowArtifactPreview(e.SelectedArtifact);
            _treeViewModel.SelectedArtifact = selectedArtifact;
        }

        public async Task GenerateAsync(IProgress<GenerationProgress> progress, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Starting code generation process...");
                var domainSchema = _domainSchemaController.DomainSchema;
                var generationResult = await _generatorOrchestrator.GenerateAsync(domainSchema, false, progress, cancellationToken);
                _treeViewModel.GenerationResult = generationResult;
                _windowManagerService.ShowGenerationTreeView(_treeViewModel);
                _logger.LogInformation("Code generation process completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the code generation process.");
                throw;
            }
        }

        public async Task GeneratePreviewAsync(IProgress<GenerationProgress> progress, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Starting code generation preview process...");
                var domainSchema = _domainSchemaController.DomainSchema;
                var generationResult = await _generatorOrchestrator.GenerateAsync(domainSchema, true, progress, cancellationToken);
                _treeViewModel.GenerationResult = generationResult;
                _windowManagerService.ShowGenerationTreeView(_treeViewModel);
                _logger.LogInformation("Code generation preview process completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the code generation preview process.");
                throw;
            }
        }
    }
}