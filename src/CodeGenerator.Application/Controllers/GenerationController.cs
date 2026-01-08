using CodeGenerator.Application.MessageBus;
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
    public class GenerationController : CoreControllerBase
    {
        private readonly GeneratorOrchestrator _generatorOrchestrator;
        private readonly GenerationResultTreeViewModel _treeViewModel;
        private readonly DomainSchemaController _domainSchemaController;

        public GenerationController(DomainSchemaController domainSchemaController, GeneratorOrchestrator generatorOrchestrator, GenerationResultTreeViewModel treeViewModel, IWindowManagerService windowManagerService, RibbonBuilder ribbonBuilder, IMessageBoxService messageService, ApplicationMessageBus messageBus, IFileSystemDialogService fileSystemDialogService, ILogger<GenerationController> logger)
            : base(windowManagerService, ribbonBuilder,messageBus, messageService, fileSystemDialogService,logger)
        {
            _generatorOrchestrator = generatorOrchestrator ?? throw new ArgumentNullException(nameof(generatorOrchestrator));
            _treeViewModel = treeViewModel ?? throw new ArgumentNullException(nameof(treeViewModel));
            _domainSchemaController = domainSchemaController ?? throw new ArgumentNullException(nameof(domainSchemaController));
        }

        public override void Initialize()
        {
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

        public override void Dispose()
        {
            // Clean up resources if needed
        }


    }
}