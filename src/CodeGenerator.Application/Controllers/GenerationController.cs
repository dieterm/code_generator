using CodeGenerator.Core.MessageBus;
using CodeGenerator.Application.Services;
using CodeGenerator.Application.ViewModels;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.DomainSchema.Schema;
using CodeGenerator.Core.Generators;
using CodeGenerator.Shared;
using CodeGenerator.Shared.ExtensionMethods;
using CodeGenerator.Shared.Ribbon;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeGenerator.Application.Controllers.Base;

namespace CodeGenerator.Application.Controllers
{
    public class GenerationController : CoreControllerBase
    {
        private readonly GenerationResultTreeViewModel _treeViewModel;
        private readonly ArtifactPreviewViewModel _artifactPreviewViewModel;
        private readonly DomainSchemaController _domainSchemaController;

        public GenerationController(DomainSchemaController domainSchemaController, ArtifactPreviewViewModel artifactPreviewViewModel, GenerationResultTreeViewModel treeViewModel, IWindowManagerService windowManagerService, RibbonBuilder ribbonBuilder, IMessageBoxService messageService, ApplicationMessageBus messageBus, IFileSystemDialogService fileSystemDialogService, ILogger<GenerationController> logger)
            : base(windowManagerService, ribbonBuilder,messageBus, messageService, fileSystemDialogService,logger)
        {
            _treeViewModel = treeViewModel ?? throw new ArgumentNullException(nameof(treeViewModel));
            _artifactPreviewViewModel = artifactPreviewViewModel ?? throw new ArgumentNullException(nameof(artifactPreviewViewModel));
            _domainSchemaController = domainSchemaController ?? throw new ArgumentNullException(nameof(domainSchemaController));
        }

        public override void Initialize()
        {
            _treeViewModel.ArtifactSelected += OnArtifactSelected;
        }

        private void OnArtifactSelected(object sender, GenerationResultTreeViewModel.ArtifactSelectedEventArgs e)
        {
            var selectedArtifact = e.SelectedArtifact;
            _treeViewModel.SelectedArtifact = selectedArtifact;

            // Artifact Preview
            if (selectedArtifact != null)
            {
                if(selectedArtifact.CanPreview)
                {
                    object? previewOutput = selectedArtifact.CreatePreview();

                    if (previewOutput is string) { 
                        _artifactPreviewViewModel.TextContent = previewOutput as string;
                        var fileExtension = selectedArtifact.GetDecoratorOfType<FileArtifactDecorator>()?.FileName.GetFileExtension();
                        switch (fileExtension)
                        {
                            case ".cs":
                                _artifactPreviewViewModel.TextLanguageSchema = ArtifactPreviewViewModel.KnownLanguages.CSharp;
                                break;
                            case ".js":
                                _artifactPreviewViewModel.TextLanguageSchema = ArtifactPreviewViewModel.KnownLanguages.JScript;
                                break;
                            case ".ts":
                                _artifactPreviewViewModel.TextLanguageSchema = ArtifactPreviewViewModel.KnownLanguages.CSharp;
                                break;
                            case ".html":
                            case ".htm":
                                _artifactPreviewViewModel.TextLanguageSchema = ArtifactPreviewViewModel.KnownLanguages.HTML;
                                break;
                            case ".xml":
                            case ".csproj":
                            case ".sln":
                            case ".targets":
                                _artifactPreviewViewModel.TextLanguageSchema = ArtifactPreviewViewModel.KnownLanguages.XML;
                                break;
                            case ".json":
                                _artifactPreviewViewModel.TextLanguageSchema = ArtifactPreviewViewModel.KnownLanguages.Undefined;
                                break;
                            case ".sql":
                                _artifactPreviewViewModel.TextLanguageSchema = ArtifactPreviewViewModel.KnownLanguages.SQL;
                                break;
                            case ".java":
                                _artifactPreviewViewModel.TextLanguageSchema = ArtifactPreviewViewModel.KnownLanguages.Java;
                                break;
                            case ".py":
                                _artifactPreviewViewModel.TextLanguageSchema = ArtifactPreviewViewModel.KnownLanguages.Undefined;
                                break;
                            default:
                                _artifactPreviewViewModel.TextLanguageSchema = ArtifactPreviewViewModel.KnownLanguages.Text;
                                break;
                        }
                    }
                    else
                        throw new NotImplementedException("Cannot preview Artifact");
                }
            }

            _windowManagerService.ShowArtifactPreview(_artifactPreviewViewModel);
        }

        public async Task GenerateAsync(IProgress<GenerationProgress> progress, CancellationToken cancellationToken)
        {
            try
            {
                var generatorOrchestrator = ServiceProviderHolder.GetRequiredService<GeneratorOrchestrator>();
                _logger.LogInformation("Starting code generation process...");
                var domainSchema = _domainSchemaController.DomainSchema;
                var generationResult = await generatorOrchestrator.GenerateAsync(domainSchema, false, progress, cancellationToken);
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
                var generatorOrchestrator = ServiceProviderHolder.GetRequiredService<GeneratorOrchestrator>();
                _logger.LogInformation("Starting code generation preview process...");
                var domainSchema = _domainSchemaController.DomainSchema;
                var generationResult = await generatorOrchestrator.GenerateAsync(domainSchema, true, progress, cancellationToken);
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